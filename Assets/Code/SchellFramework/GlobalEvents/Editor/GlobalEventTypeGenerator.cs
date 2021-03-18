// ----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved. 
// 
//  Author: Ryan Hipple
//  Date:   07/19/2016
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SG.Core.Inspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace SG.GlobalEvents.Editor
{
    // ToDo: header for generated files.
    
    // TODO: function to regenerate all types. Scans through generated directory and regenerates each one based on name

    public class GlobalEventTypeGenerator
    {
        #region -- Built-in Available Global Event Types ----------------------
        public static readonly Type[] BuiltInTypes =
        {
            typeof(void),
            typeof(GameObject),
            typeof(Transform),
            typeof(int),
            typeof(float),
            typeof(bool),
            typeof(string),
            typeof(AudioMixer),
            typeof(AudioMixerGroup),
            typeof(AudioMixerSnapshot),
            typeof(AudioClip),
            typeof(CanvasGroup),
            typeof(CanvasRenderer),
            typeof(Collider),
            typeof(BoxCollider),
            typeof(CapsuleCollider),
            typeof(CharacterController),
            typeof(MeshCollider),
            typeof(SphereCollider),
            typeof(TerrainCollider),
            typeof(WheelCollider),
            typeof(Joint),
            typeof(CharacterJoint),
            typeof(ConfigurableJoint),
            typeof(FixedJoint),
            typeof(HingeJoint),
            typeof(SpringJoint),
            typeof(MeshFilter),
            typeof(UnityEngine.AI.OffMeshLink),
            typeof(ParticleSystem),
            typeof(Renderer),
            typeof(BillboardRenderer),
            typeof(LineRenderer),
            typeof(MeshRenderer),
            typeof(ParticleSystemRenderer),
            typeof(SkinnedMeshRenderer),
            typeof(SpriteRenderer),
            typeof(TrailRenderer),
            typeof(Rigidbody),
            typeof(Rigidbody2D),
            typeof(RectTransform),
            typeof(Material),
            typeof(Mesh),
            typeof(PhysicMaterial),
            typeof(PhysicsMaterial2D),
            typeof(Shader),
            typeof(Sprite),
            typeof(TextAsset),
            typeof(Texture),
            typeof(Cubemap),
            //typeof(MovieTexture),
            //typeof(ProceduralTexture),
            typeof(RenderTexture),
            typeof(SparseTexture),
            typeof(Texture2D),
            typeof(Texture3D),
            typeof(Color),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Rect)
        };
        #endregion -- Built-in Available Global Event Types -------------------

        private static readonly List<Type> potentialDataTypes = new List<Type>();
        private static int existingCount;

        public static Type FindGlobalEventTypeForPassingType(Type passingType)
        {
            if (passingType == typeof(void))
                return typeof(GlobalEvent);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].ManifestModule.Name == "Assembly-CSharp.dll")
                {
                    Type[] types = assemblies[i].GetTypes();
                    for (int t = 0; t < types.Length; t++)
                    {
                        if ((typeof(BaseGlobalEvent)).IsAssignableFrom(types[t]) && !types[t].IsAbstract)
                        {
                            Type[] generics = types[t].BaseType.GetGenericArguments();
                            for (int g = 0; g < generics.Length; g++)
                            {
                                if (generics[g] == passingType)
                                {
                                    return types[t];
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static List<Type> PotentialDataTypes
        {
            get
            {
                if (potentialDataTypes.Count == 0)
                    GenerateTypeList();
                return potentialDataTypes;
            }
        }

        public static int ExistingGlobalEventTypeCount
        {
            get
            {
                if (potentialDataTypes.Count == 0)
                    GenerateTypeList();
                return existingCount;
            }
        }

        public static void GenerateTypeList()
        {
            existingCount = 0;
            potentialDataTypes.Clear();
            for (int i = 0; i < BuiltInTypes.Length; i++)
            {
                if (FindGlobalEventTypeForPassingType(BuiltInTypes[i]) != null)
                {
                    existingCount++;
                    potentialDataTypes.Insert(0, BuiltInTypes[i]);
                }
                else
                {
                    potentialDataTypes.Add(BuiltInTypes[i]);
                }
            }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                // There has to be a better way than this...
                if (assemblies[i].FullName.ToLower().Contains("editor"))
                    continue;
                Type[] types = assemblies[i].GetTypes();
                for (int t = 0; t < types.Length; t++)
                {
                    if (typeof(BaseGlobalEvent).IsAssignableFrom(types[t]))
                        continue;
                    if (typeof(UnityEventBase).IsAssignableFrom(types[t]))
                        continue;
                    object[] serializables = new object[0];
                    if (typeof(BaseGlobalEvent).Assembly == assemblies[i])
                        serializables = types[t].GetCustomAttributes(typeof(SerializableAttribute), false);
                    if (serializables.Length > 0 ||
                        (types[t].IsSubclassOf(typeof(MonoBehaviour)) ||
                        types[t].IsSubclassOf(typeof(ScriptableObject))))
                    {
                        if (FindGlobalEventTypeForPassingType(types[t]) != null)
                        {
                            existingCount++;
                            potentialDataTypes.Insert(0, types[t]);
                        }
                        else
                        {
                            potentialDataTypes.Add((types[t]));
                        }
                    }
                }
            }
        }

        #region -- Code Generation Templates ----------------------------------
        /// <summary>
        /// Template string used to generate event classes. 
        /// 0 - Type name (MyType)
        /// 1 - Full type name (SG.MyGame.MyType)
        /// 2 - Color for Icon
        /// </summary>
        private const string EventTemplate =
@"namespace SG.GlobalEvents.Generated
{{
    [Core.AssetIcons.GenerateAssetIcon(""{2}"", DEFAULT_ASSET_ICON_REPLACE_KEY, DEFAULT_ASSET_ICON)]
    public class {0}GlobalEvent : GenericGlobalEvent<{1}> {{ }}

    [System.Serializable]
    public class {0}UnityEvent : UnityEngine.Events.UnityEvent<{1}> {{ }}
}}";

        /// <summary>
        /// Template string used to generate event listener classes.
        /// 0 - Type name (MyType)
        /// 1 - Full type name (SG.MyGame.MyType)
        /// </summary>
        private const string ListenerTemplate =
@"namespace SG.GlobalEvents.Generated
{{
    [UnityEngine.AddComponentMenu("""")]
    public class {0}GlobalEventListener : GenericGlobalEventListener<{0}GlobalEvent, {0}UnityEvent, {1}> {{ }}
}}";

        //public class MaterialGlobalEventListener : GenericGlobalEventListener<
        //MaterialGlobalEvent, MaterialUnityEvent, UnityEngine.Material, MaterialGlobalEventListener>

        /// <summary>
        /// Template string used to generate event listener classes.
        /// 0 - Type name (MyType)
        /// 1 - Full type name (SG.MyGame.MyType)
        /// </summary>
        private const string RangeListenerTemplate =
@"namespace SG.GlobalEvents.Generated
{{
    [UnityEngine.AddComponentMenu("""")]
    public class {0}GlobalEventListener : GenericRangeGlobalEventListener<{0}GlobalEvent, {0}UnityEvent, {1}, {2}, {3}, {4}> {{ }}
}}";

        //public class Int32GlobalEventListener : GenericRangeGlobalEventListener<
        //Int32GlobalEvent, Int32UnityEvent, System.Int32, 2-IntRangeSet, 3-IntRange, 4-IntRangeComparison, Int32GlobalEventListener>

        public const string GENERATED_DIRECTORY_NAME = "Generated";
        public const string GENERATED_EVENT_SUFFIX = "GlobalEvent.cs";
        public const string GENERATED_LISTENER_SUFFIX = "GlobalEventListener.cs";
        #endregion -- Code Generation Templates -------------------------------

        #region -- Type Generation --------------------------------------------
        /// <summary>
        /// Gets the root directory for all generated GlobalEvent classes.
        /// </summary>
        /// <returns>Fully qualified directory path.</returns>
        public static string GetGeneratedClassRoot()
        {
            string thisFile = new StackTrace(true).GetFrame(0).GetFileName();
            string geRoot = Path.GetDirectoryName(Path.GetDirectoryName(thisFile));
            return Path.Combine(geRoot, GENERATED_DIRECTORY_NAME);
        }

        /// <summary> Gets a path for a newly generated type. </summary>
        /// <param name="typeName">
        /// Name of the file for the type, for example "MyTypeGlobalEvent.cs".
        /// </param>
        /// <returns>Fully qualified path for the class file.</returns>
        public static string GetGeneratedClassPath(string typeName)
        {
            string generatedRoot = GetGeneratedClassRoot();
            if (!Directory.Exists(generatedRoot))
                Directory.CreateDirectory(generatedRoot);
            return Path.Combine(generatedRoot, typeName);
        }

        /// <summary>
        /// Create a file containing the class for a generated GlobalEvent.
        /// </summary>
        /// <param name="type">Type that the global event passes.</param>
        public static void CreateEventFile(Type type)
        {
            string text = GetClassText(type, EventTemplate);
            string filePath = GetGeneratedClassPath(type.Name + GENERATED_EVENT_SUFFIX);
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// Create a file containing the class for a generated 
        /// GlobalEventListener.
        /// </summary>
        /// <param name="type">
        /// Type that the global event listener handles.
        /// </param>
        public static void CreateListenerFile(Type type)
        {
            //string text = GetClassText(type, ListenerTemplate);

            // 2-IntRangeSet, 3-IntRange, 4-IntRangeComparison
            Type rangeType = null;
            Type rangeSetType = null;
            Type comparisonType = null;
            Type[] allTypes = typeof(BaseRange).Assembly.GetTypes();
            for (int i = 0; i < allTypes.Length; i++)
            {
                if (rangeType == null)
                {
                    if (typeof (BaseRange).IsAssignableFrom(allTypes[i]))
                    {
                        Type[] ts = allTypes[i].BaseType.GetGenericArguments();
                        if (ts.Length > 0 &&
                            ts[0] == type)
                        {
                            UnityEngine.Debug.Log("Found rangeType " + type.Name + " " + allTypes[i].Name);
                            rangeType = allTypes[i];
                        }
                    }
                }
            }

            for (int i = 0; i < allTypes.Length; i++)
            {
                if (comparisonType == null)
                {
                    if (typeof(BaseValueComparison).IsAssignableFrom(allTypes[i]))
                    {
                        Type[] ts = allTypes[i].BaseType.GetGenericArguments();
                        if (ts.Length > 1 && ts[0] == rangeType && ts[1] == type)
                        {
                            UnityEngine.Debug.Log("Found comparisonType " + type.Name + " " + allTypes[i].Name);
                            comparisonType = allTypes[i];
                        }
                    }
                }
            }

            for (int i = 0; i < allTypes.Length; i++)
            {
                if (rangeSetType == null && comparisonType != null)
                {
                    if (typeof (BaseValueRangeSet).IsAssignableFrom(allTypes[i]))
                    {
                        Type[] ts = allTypes[i].BaseType.GetGenericArguments();
                        if (ts.Length > 0 &&
                            ts[0] == comparisonType)
                        {
                            UnityEngine.Debug.Log("Found rangeSetType " + type.Name + " " + allTypes[i].Name);
                            rangeSetType = allTypes[i];
                        }
                    }
                }
            }
            
            string typeName = type.Name;
            string qualifiedTypeName = type.FullName;
            string text = rangeSetType != null ? 
                string.Format(RangeListenerTemplate, typeName, qualifiedTypeName, rangeSetType, rangeType, comparisonType) :
                string.Format(ListenerTemplate, typeName, qualifiedTypeName);

            string filePath = GetGeneratedClassPath(type.Name + GENERATED_LISTENER_SUFFIX);
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// Get the text for a generated event class using the given template.
        /// </summary>
        /// <param name="referencedType">
        /// The type that the generated event class will use.
        /// </param>
        /// <param name="template">
        /// Template describing how to build the class text.
        /// </param>
        /// <returns>Text to be used as a class file.</returns>
        public static string GetClassText(Type referencedType, string template)
        {
            Random.InitState(referencedType.FullName.GetHashCode());
            Color c = Random.ColorHSV(0.0f, 1.0f, 0.75f, 0.75f, 0.75f, 0.75f);
            string typeName = referencedType.Name;
            string qualifiedTypeName = referencedType.FullName;
            return string.Format(template, typeName, qualifiedTypeName, ColorUtility.ToHtmlStringRGBA(c));
        }

        #endregion -- Type Generation -----------------------------------------
    }
}