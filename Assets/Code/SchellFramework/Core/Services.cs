// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 6/17/2016 3:06:12 PM
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace SG.Core
{
    /// <summary>
    /// Simple Service locater class which can be used by types that it doesn't know about.
    /// 
    /// When registering ScriptableObjects or MonoBehaviors, call Register in OnEnable or after
    /// whatever initialization functions which are necessary are run.
    /// 
    /// If the class is normally simply created on demand (the Instance == _instance or new instance pattern)
    /// then you can add the ServiceOnDemand attribute to the class and the Services Locate command will
    /// instantiate it for you when on first Locate.
    /// 
    /// For some test examples, view Editor/Tests/ServicesTests.cs
    /// 
    /// Example usage: Define an IService interface.
    /// This is just any interface you care to write which implements IService, which currently has no content.
    /// 
    /// public interface ITitleService : IService
    /// {
    ///     string GetTitle(string value);
    /// }
    /// 
    /// Example usage: Define the class that will implement the interface.
    /// This is your "singleton" class, to some degree, but does not need to follow the singleton pattern.
    /// 
    /// It can either be a class which can be instantated normally, in which case you can either instantiate it and
    /// register it yourself.
    /// 
    /// Example usage: Instantiate and register manally.
    /// 
    /// public TitleService : ITitleService
    /// {
    ///     public string GetTitle(string value) { / * implementation * / }
    /// }
    /// 
    /// public TitleManager MonoBehaviour
    /// {
    ///     public void Awake()
    ///     {
    ///         // Create the service
    ///         ITitleService titling = new TitleService;
    ///         // Register it with the locator
    ///         Services.Register<ITitleService>(titling);
    ///     }
    /// }
    /// 
    /// --- or add the ServiceOnDemand attribute, which informs the Services locator that should
    /// someone request this service but it is not registered, that it should simply instantiate the class itself.
    /// 
    /// Example usage: ServiceOnDemand attribute to declare the implementation as safe to construct on demand.
    /// 
    /// [ServiceOnDemand(ServiceType = ITitleService)]
    /// /// public AutoTitleService : ITitleService
    /// {
    ///     public string GetTitle(string value) { / * implementation * / }
    /// }
    /// 
    /// If it is a Unity ScriptableObject, then you can register it with inside of the OnAfterDeserialize or OnEnable
    /// routines, or whenever you need to depending on your specific use case.
    /// 
    /// If it is a Unity MonoBehavior, you can register it as soon as its initialization is finished (typically at the
    /// end of the Awake method).
    /// 
    /// To support a flexibile initialization ordering, the Services locator supports an InvokeWhenRegistered callback.
    /// If you require using a Service while that Service may not be initialized, you can add your method using
    /// InvokeWhenRegistered. Once the Service is fully initialized and registers with the Service locator, the callback
    /// will be invoked.
    /// - Likewise, make sure your Service is ready to be used as soon as the Register method is called, as anything
    /// waiting for your service to inisitalize will likely make an immediate request in response.
    /// - Obviously, if you deadlock the produce by making two Services initialization dependent on each other, that's
    /// no good, so watch out for such a thing.
    /// 
    /// public TitleDisplay : MonoBehaviour
    /// {
    ///     public Text MonitorTexts;
    /// 
    ///     public void Start()
    ///     {
    ///         Services.InvokeWhenRegistered<ITitleService>(SetupTitles);
    ///         // if ITitleService is already registered or has an ServiceOnDemand attribute, this will be invoked
    ///         // immediately. Otherwise, it'll be invoked when the 
    ///     }
    /// }
    /// </summary>
    public static class Services
    {
        //private static readonly Notify Log = NotifyManager.GetInstance("SG.Core");

        private static readonly Dictionary<Type, IService> _registeredServices = new Dictionary<Type, IService>();

        private static readonly Dictionary<Type, List<Action>> _whenRegisteredCallbacks = new Dictionary<Type, List<Action>>();

        [NotNull]
        public static TService Locate<TService>() where TService : class, IService
        {
            TService service = LocateOrCreateInternal<TService>();
            if (service != null)
                return service;

            throw new Exception("No registered or ServiceOnDemand type found for " + typeof(TService).Name);
        }

        private static TService LocateOrCreateInternal<TService>() where TService : class, IService
        {
            // Manually registered or previously created
            Type serviceType = typeof(TService);
            IService found;
            if (_registeredServices.TryGetValue(serviceType, out found))
            {
                TService foundTService = found as TService;
                if (foundTService != null)
                    return foundTService;
            }

            // OnDemand
            Type instanceType;
            if (!ServiceOnDemandAttribute.TryGetOnDemandType(serviceType, out instanceType))
                return null;

            TService newTService = Activator.CreateInstance(instanceType) as TService;
            if (newTService == null)
                return null;

            _registeredServices[serviceType] = newTService;
            return newTService;
        }

        public static void Register<TService>([NotNull] TService service) where TService : class, IService
        {
            Type serviceType = typeof(TService);
            _registeredServices[serviceType] = service;

            List<Action> callbacksForServiceType;
            if (!_whenRegisteredCallbacks.TryGetValue(serviceType, out callbacksForServiceType))
                return;

            _whenRegisteredCallbacks.Remove(serviceType);

            for (int i = 0; i < callbacksForServiceType.Count; i++)
                callbacksForServiceType[i].Invoke();
            callbacksForServiceType.Clear();
        }

        /// <summary>
        /// Add a callback to a list of callbacks keyed by service type.
        /// After that service is registered, the callback will be called.
        /// 
        /// If the service is already registered, the callback will be invoked immediately.
        /// If the service is not registered at all, the callback will never be called.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="callback"></param>
        public static void InvokeWhenRegistered<TService>(Action callback) where TService : class, IService
        {
            TService alreadyRegistered = LocateOrCreateInternal<TService>();
            if (alreadyRegistered != null)
            {
                callback.Invoke();
                return;
            }
            List<Action> callbacksForServiceType;
            if (!_whenRegisteredCallbacks.TryGetValue(typeof(TService), out callbacksForServiceType))
                _whenRegisteredCallbacks[typeof(TService)] = callbacksForServiceType = new List<Action>();
            callbacksForServiceType.Add(callback);
        }
    }

    public interface IService { }

    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceOnDemandAttribute : Attribute
    {
        private static readonly Notify Log = NotifyManager.GetInstance("SG.Core");

        public Type ServiceType;

        private static Dictionary<Type, Type> _serviceTypeToOnDemandConcreteType;

        public static bool TryGetOnDemandType(Type serviceType, out Type found)
        {
            if (_serviceTypeToOnDemandConcreteType != null)
                return _serviceTypeToOnDemandConcreteType.TryGetValue(serviceType, out found);

            _serviceTypeToOnDemandConcreteType = new Dictionary<Type, Type>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int asm = 0; asm < assemblies.Length; asm++)
            {
                Type[] types = AssemblyUtility.GetTypes(assemblies[asm]);
                for (int t = 0; t < types.Length; t++)
                {
                    object[] onDemandAttributes = types[t].GetCustomAttributes(typeof(ServiceOnDemandAttribute),
                        false);
                    for (int a = 0; a < onDemandAttributes.Length; a++)
                    {
                        ServiceOnDemandAttribute att = (ServiceOnDemandAttribute)onDemandAttributes[a];

                        if (!typeof(IService).IsAssignableFrom(att.ServiceType))
                        {
                            Log.Error("ServiceOnDemand attribute present on type " + types[t].Name +
                                      " which does not implement IService");
                            continue;
                        }

                        bool foundExisting = _serviceTypeToOnDemandConcreteType.TryGetValue(att.ServiceType,
                            out found);
                        if (foundExisting)
                            Log.Error("Duplicate ServiceOnDemand attributes for type " + serviceType + " " +
                                      found.Name + " & " + types[t].Name);
                        else
                            _serviceTypeToOnDemandConcreteType.Add(att.ServiceType, types[t]);
                    }
                }
            }
            return _serviceTypeToOnDemandConcreteType.TryGetValue(serviceType, out found);
        }
    }
}
