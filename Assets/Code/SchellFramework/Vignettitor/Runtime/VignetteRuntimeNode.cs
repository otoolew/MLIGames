// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 3/1/2016 10:39:19 AM
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using SG.Core;
using SG.Vignettitor.Runtime;
using SG.Vignettitor.VignetteData;
using UnityEngine;

namespace SG.Vignettitor
{
    public delegate VignetteRuntimeNode VignetteGeneratorDelegate(VignetteNode node, VignetteRuntimeGraph runtimeGraph);

    [Serializable]
    public abstract class VignetteRuntimeNode
    {
        protected static readonly Notify Log = NotifyManager.GetInstance<VignetteRuntimeNode>();

        protected VignetteRuntimeNode(VignetteNode node, VignetteRuntimeGraph runtimeGraph)
        {
            _runtime = runtimeGraph;
            _source = node;
        }

        [SerializeField]
        protected VignetteRuntimeGraph _runtime;

        [SerializeField]
        private VignetteNode _source;
        public VignetteNode Source { get { return _source; } }

        /// <summary>
        /// Gets a value indicating whether this instance is hard wired.
        /// If true, call SelectChild on the output specified by
        /// <see cref="overrideOutput"/>
        /// </summary>
        public bool IsHardwired { get { return OverrideOutput >= 0; } }

        /// <summary>
        /// When debugging, this specfies an index of the child that this node 
        /// should pass control to regardless of any other conditions. A value 
        /// of -1 indicates that the decision should be made as normal.
        /// </summary>
        public int OverrideOutput = -1;

        #region -- Node State Control -----------------------------------------

        public virtual void OnBeforeGraphStart() { }

        public virtual void OnBeforeGraphEnd() { }

        /// <summary>
        /// Skips to the next child node immediately if possible. Nodes that 
        /// require a user choice are not skippable.
        /// </summary>
        /// <returns>True if this node can be skipped.</returns>
        public virtual bool Skip()
        {
            if (_source.Children == null || _source.Children.Length < 1)
                EndVignette();
            else if (_source.Children.Length == 1)
            {
                SelectChild(0);
                return true;
            }
            return false;
        }

        /// <summary>Begins control of this node.</summary>
        /// <param name="input">An object that may be used by any node.</param>
        public virtual void Enter(object input) { }

        /// <summary>
        /// Completes the execution of this node. Be sure that "SetNextNode"
        /// is called before this.
        /// </summary>
        /// <returns>An object that will be passed to the next node.</returns>
        public virtual object Exit()
        {
            return null;
        }

        /// <summary>
        /// Called every frame that the node is active.
        /// </summary>
        public virtual void Update()
        { }

        /// <summary>
        /// Selects a child node as the next node in a Vignette.
        /// </summary>
        /// <param name="index">
        /// The index of the child node to set as the next node.
        /// </param>
        protected void SelectChild(int index)
        {
            if (_runtime == null || !_runtime.Valid)
                return;

            if (_source.Children == null || index < 0 || index >= _source.Children.Length)
            {
                throw new IndexOutOfRangeException("Unable to select child "
                    + index + " of vignette runtime node " + _source.NodeID);
            }
            _runtime.SetNode(_source.Children[index].NodeID);
        }

        protected virtual void SelectChildOrExit(int index)
        {
            VignetteNode child = _source.Children.SafeGet(index);
            if (child)
                _runtime.SetNode(child.NodeID);
            else
                EndVignette();
        }

        /// <summary>
        /// Indicates that an expected completion has occured.
        /// </summary>
        protected virtual void EndVignette()
        {
            if (_runtime == null || !_runtime.Valid)
                return;

            _runtime.SetNode(-1);
        }
        #endregion -- Node State Control --------------------------------------

        /// <summary>
        /// Traverse forward through the graph, seaching for nodes of the 
        /// given type. May throw an exception if this node or other nodes are
        ///  not first checked with SupportsLookahead.
        /// </summary>
        /// <typeparam name="T">Type of node to look for.</typeparam>
        /// <returns>
        /// Returns null if no destination node was found at the end of the 
        /// path chain before a node which does not support that type of 
        /// lookahead is found. For example: SetBool followed by a standard 
        /// ChoiceInteractionNode - the SetBool supports lookahead, but the 
        /// CIN does not
        /// 
        /// Returns a node of type T if one is found at the end of the path 
        /// chain.
        /// </returns>
        public virtual T Lookahead<T>() where T : VignetteNode
        {
            throw new NotImplementedException(string.Format("{0} Lookahead not supported for {1} node", typeof(T).Name, GetType().Name));
        }

        public List<T> GetAllChildrenOfType<T>() where T : VignetteRuntimeNode
        {
            List<T> results = new List<T>();

            int childCount = _source.Children.SafeLength();
            for (int i = 0; i < childCount; i++)
            {
                int childId = _source.Children[i] ? _source.Children[i].NodeID : -1;
                T current = _runtime[childId] as T;
                if (current == null)
                    continue;

                results.Add(current);
            }

            return results;
        }
    }

    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class VignetteGeneratorAttribute : Attribute
    {
        public readonly Type DelegateType;
        public readonly Type NodeType;

        public VignetteGeneratorAttribute(Type delegateType, Type nodeType)
        {
            DelegateType = delegateType;
            NodeType = nodeType;
        }

        private static Dictionary<Type, Dictionary<Type, Delegate>> _delegateTypeToNodeTypeToGenerator;

        [Pure]
        [CanBeNull]
        public static TDelegate GetGeneratorDelegate<TDelegate>(VignetteNode node) where TDelegate : class
        {
            if (_delegateTypeToNodeTypeToGenerator == null)
                _delegateTypeToNodeTypeToGenerator = GatherGeneratorDelegates();

            // Retrieve lookup table for this delegate type
            Dictionary<Type, Delegate> nodeTypeToGenerator;
            if (!_delegateTypeToNodeTypeToGenerator.TryGetValue(typeof(TDelegate), out nodeTypeToGenerator))
                return default(TDelegate);

            Delegate generator;
            if (!nodeTypeToGenerator.TryGetValue(node.GetType(), out generator))
                return default(TDelegate);
            return generator as TDelegate;
        }

        [Pure]
        private static Dictionary<Type, Dictionary<Type, Delegate>> GatherGeneratorDelegates()
        {
            Dictionary<Type, Dictionary<Type, Delegate>> delegateTypeToNodeTypeToGenerator =
                new Dictionary<Type, Dictionary<Type, Delegate>>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int asm = 0; asm < assemblies.Length; asm++)
            {
                Type[] types = AssemblyUtility.GetTypes(assemblies[asm]);
                for (int t = 0; t < types.Length; t++)
                {
                    MethodInfo[] methods = types[t].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    for (int m = 0; m < methods.Length; m++)
                    {
                        object[] generatorAttributes = methods[m].GetCustomAttributes(typeof(VignetteGeneratorAttribute), false);
                        for (int a = 0; a < generatorAttributes.Length; a++)
                        {
                            VignetteGeneratorAttribute att = (VignetteGeneratorAttribute)generatorAttributes[a];

                            // Retrieve or create lookup table for this delegate type
                            Dictionary<Type, Delegate> nodeTypeToGenerator;
                            if (!delegateTypeToNodeTypeToGenerator.TryGetValue(att.DelegateType, out nodeTypeToGenerator))
                                delegateTypeToNodeTypeToGenerator[att.DelegateType] = nodeTypeToGenerator = new Dictionary<Type, Delegate>();

                            // Create and store delegate using this method info with this delegate type and node type
                            if (nodeTypeToGenerator.ContainsKey(att.NodeType))
                                throw new Exception(string.Concat("Duplicate VignetteRuntimeGenerator declared for Delegate ",
                                    att.DelegateType.Name, " Node ", att.NodeType.Name));

                            nodeTypeToGenerator[att.NodeType] = Delegate.CreateDelegate(att.DelegateType, methods[m], true);
                        }
                    }
                }
            }
            return delegateTypeToNodeTypeToGenerator;
        }
    }
}
