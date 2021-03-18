// -----------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Joseph Pasek
//
//  Created: 4/22/2016 2:56:14 PM
// -----------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace SG.Core
{
    public interface IMaterialSceneHandle
    {
        void Draw(Transform transform, MaterialProperty property);
    }

    [CustomEditor(typeof(Material))]
    public class MaterialEditor : UnityEditor.MaterialEditor
    {
        Material m { get { return (Material)target; } }

        const string FORMAT_WARNING = "A Custom Render Queue of {0} is Defined.";

        public Dictionary<string, IMaterialSceneHandle> propertySceneHandles = new Dictionary<string, IMaterialSceneHandle>();
        public List<string> dependencyProperties = new List<string>();

#pragma warning disable 0414 // disabled because this may be used outside of this module
        Color[] backgroundColors = new Color[4] { Color.white, Color.white, Color.white, Color.white };
        public Color[] BackgroundColors { get { return backgroundColors; } }
#pragma warning restore 0414

        System.Type StylesType;
        MethodInfo CheckSetupMethod;
        MethodInfo DetectShaderChangedMethod;
        MethodInfo HasMultipleMixedShaderValuesMethod;
        MethodInfo GetAssociatedRenderFromInspectorStaticMethod;
        FieldInfo m_ShaderField;
        FieldInfo m_CustomShaderGUIField;
        FieldInfo m_InsidePropertiesGUIField;
        FieldInfo m_RendererForAnimationModeField;
        FieldInfo m_PropertyBlockField;
        FieldInfo m_InfoMessageField;
        FieldInfo s_ControlHashField;

        public override void OnEnable()
        {
            // Get all of the fields and methods we'll be wrapping

            StylesType = typeof(UnityEditor.MaterialEditor).GetNestedType("Styles", BindingFlags.NonPublic | BindingFlags.Static);

            CheckSetupMethod = typeof(UnityEditor.MaterialEditor).GetMethod("CheckSetup", BindingFlags.NonPublic | BindingFlags.Instance);
            DetectShaderChangedMethod = typeof(UnityEditor.MaterialEditor).GetMethod("DetectShaderChanged", BindingFlags.NonPublic | BindingFlags.Instance);
            HasMultipleMixedShaderValuesMethod = typeof(UnityEditor.MaterialEditor).GetMethod("HasMultipleMixedShaderValues", BindingFlags.NonPublic | BindingFlags.Instance);

            GetAssociatedRenderFromInspectorStaticMethod = typeof(UnityEditor.MaterialEditor).GetMethod("GetAssociatedRenderFromInspector", BindingFlags.NonPublic | BindingFlags.Static);

            m_ShaderField = typeof(UnityEditor.MaterialEditor).GetField("m_Shader", BindingFlags.NonPublic | BindingFlags.Instance);
            m_CustomShaderGUIField = typeof(UnityEditor.MaterialEditor).GetField("m_CustomShaderGUI", BindingFlags.NonPublic | BindingFlags.Instance);
            m_InsidePropertiesGUIField = typeof(UnityEditor.MaterialEditor).GetField("m_InsidePropertiesGUI", BindingFlags.NonPublic | BindingFlags.Instance);
            m_RendererForAnimationModeField = typeof(UnityEditor.MaterialEditor).GetField("m_RendererForAnimationMode", BindingFlags.NonPublic | BindingFlags.Instance);
            m_PropertyBlockField = typeof(UnityEditor.MaterialEditor).GetField("m_PropertyBlock", BindingFlags.NonPublic | BindingFlags.Instance);
            m_InfoMessageField = typeof(UnityEditor.MaterialEditor).GetField("m_InfoMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            s_ControlHashField = typeof(UnityEditor.MaterialEditor).GetField("s_ControlHash", BindingFlags.NonPublic | BindingFlags.Static);

            base.OnEnable();
            SceneView.duringSceneGui += DrawSceneGUI;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneView.duringSceneGui -= DrawSceneGUI;
        }

        /// <summary>
        /// Adds the handle for the given property name to be drawn by this MaterialEditor.
        /// </summary>
        /// <param name="propertyName">The name of the property for which the handle will be drawn for.</param>
        /// <param name="handle">The executing handle.</param>
        public void AddSceneHandle(string propertyName, IMaterialSceneHandle handle)
        {
            if (!propertySceneHandles.ContainsKey(propertyName))
                propertySceneHandles.Add(propertyName, handle);
        }

        /// <summary>
        /// Adds a dependency property 
        /// </summary>
        /// <param name="propertyName"></param>
        public void AddDependency(string propertyName)
        {
            if (!dependencyProperties.Contains(propertyName))
                dependencyProperties.Add(propertyName);
        }

        public void SetBackgroundColorAtIndex(int index, Color color)
        {
            backgroundColors[index] = color;
        }

        public override void OnInspectorGUI() 
        {
            base.serializedObject.Update();
            CheckSetupMethod.Invoke(this,null);
            if (DetectShaderChangedMethod != null)
                DetectShaderChangedMethod.Invoke(this, null);

            bool hasMultipleMixedShaderValues = (bool) HasMultipleMixedShaderValuesMethod.Invoke(this, null);

            bool hasCustomShaderGUI = m_CustomShaderGUIField.GetValue(this) != null;
            if (hasCustomShaderGUI)
                GUILayout.Label("Custom ShaderGUI Present.");

            if (this.isVisible && m_ShaderField.GetValue(this) != null && hasMultipleMixedShaderValues == false && _PropertiesGUI())
            {
                this.PropertiesChanged();
            }
        }

        [MenuItem("CONTEXT/Material/Create Scene Instance")]
        static void CreateSceneInstance(UnityEditor.MenuCommand c)
        {
            Material m = (Material)c.context;
            foreach (GameObject go in Selection.gameObjects)
            {
                Renderer r = go.GetComponent<Renderer>();
                if (r == null) continue;
                Material[] sharedMats = r.sharedMaterials;
                for (int j = 0; j < sharedMats.Length; j++)
                {
                    if (sharedMats[j] == m)
                    {
                        sharedMats[j] = new Material(sharedMats[j]);
                        sharedMats[j].name += " (Instance)";
                        r.sharedMaterials = sharedMats;
                    }
                }
            }
        }

        public void _PropertiesDefaultGUI(MaterialProperty[] props)
        {
            this.SetDefaultGUIWidths();

            string infoMessage = (string)m_InfoMessageField.GetValue(this);

            if (infoMessage != null)
            {
                EditorGUILayout.HelpBox(infoMessage, MessageType.Info);
            }
            else
            {
                GUIUtility.GetControlID((int) s_ControlHashField.GetValue(null), FocusType.Passive, new Rect(0f, 0f, 0f, 0f));
            }
            for (int i = 0; i < props.Length; i++)
            {
                // Cache States
                Color bgColor = GUI.backgroundColor;
                bool guiEnabled = GUI.enabled;
                
                if ((props[i].flags & (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) == MaterialProperty.PropFlags.None)
                {
                    float propertyHeight = this.GetPropertyHeight(props[i], props[i].displayName);
                    Rect controlRect = EditorGUILayout.GetControlRect(true, propertyHeight, EditorStyles.layerMaskField, new GUILayoutOption[0]);
                    this.ShaderProperty(controlRect, props[i], props[i].displayName);
                }
                
                // Clear States
                if (dependencyProperties.Contains(props[i].name))
                {
                    dependencyProperties.Remove(props[i].name);
                    GUI.enabled = guiEnabled;
                }
                GUI.backgroundColor = bgColor;
                backgroundColors = new Color[4] { bgColor, bgColor, bgColor, bgColor };
            }
        }

        public bool _PropertiesGUI()
        {
            bool insidePropertiesGUI = (bool)m_InsidePropertiesGUIField.GetValue(this);

            if (insidePropertiesGUI)
            {
                Debug.LogWarning("PropertiesGUI() is being called recursivly. If you want to render the default gui for shader properties then call PropertiesDefaultGUI() instead");
                return false;
            }
            EditorGUI.BeginChangeCheck();
            MaterialProperty[] materialProperties = MaterialEditor.GetMaterialProperties(base.targets);
            Renderer r = MaterialEditor.PrepareMaterialPropertiesForAnimationMode(materialProperties, GUI.enabled);
            m_RendererForAnimationModeField.SetValue(this,r);

            bool enabled = GUI.enabled;
            if (r != null)
            {
                GUI.enabled = true;
            }

            insidePropertiesGUI = true;

            try
            {
                ShaderGUI customShaderGUI = (ShaderGUI) m_CustomShaderGUIField.GetValue(this);
                if (customShaderGUI != null)
                {
                    customShaderGUI.OnGUI(this, materialProperties);
                }
                else
                {
                    _PropertiesDefaultGUI(materialProperties);
                }
                Renderer associatedRenderFromInspector = (Renderer) GetAssociatedRenderFromInspectorStaticMethod.Invoke(null,null);
                if (associatedRenderFromInspector != null)
                {
                    MaterialPropertyBlock mpb = (MaterialPropertyBlock)m_PropertyBlockField.GetValue(this);

                    if (Event.current.type == EventType.Layout)
                    {
                        associatedRenderFromInspector.GetPropertyBlock(mpb);
                    }
                    if (mpb != null && !mpb.isEmpty)
                    {
                        EditorGUILayout.HelpBox((string) StylesType.GetField("propBlockWarning", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null), MessageType.Warning);
                    }
                }
            }
            catch (System.Exception)
            {
                GUI.enabled = enabled;
                insidePropertiesGUI = false;
                r = null;
                throw;
            }
            GUI.enabled = enabled;
            insidePropertiesGUI = false;
            r = null;
            return EditorGUI.EndChangeCheck();
        }

        void DrawSceneGUI(SceneView sceneView)
        {
            bool hasMultipleMixedShaderValues = (bool)HasMultipleMixedShaderValuesMethod.Invoke(this, null);
            if (hasMultipleMixedShaderValues)
                return;

            GameObject go = Selection.activeGameObject;
            if (go == null) return;
            Renderer r = go.GetComponent<Renderer>();
            if (r == null) return;
            if (!r.sharedMaterials.Contains<Material>(m))
                return;

            Transform targetTransform = go.transform;

            foreach(MaterialProperty property in GetMaterialProperties(targets))
            {
                IMaterialSceneHandle handle;
                if (propertySceneHandles.TryGetValue(property.name, out handle))
                    handle.Draw(targetTransform, property);
            }
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            serializedObject.Update();
            SerializedProperty customRenderQueue = serializedObject.FindProperty("m_CustomRenderQueue");
            
            // Only draw the control when there is an override and it does not conflict with the Shader's render queue. 
            // This eliminates false-positives when Unity decides to set the custom queue to the Shader's automatically.
            if (customRenderQueue.intValue != -1 && customRenderQueue.intValue != m.shader.renderQueue)
            {
                EditorGUILayout.BeginVertical(UnityEngine.GUI.skin.box);
                {
                    EditorGUILayout.HelpBox(
                        string.Format(
                            FORMAT_WARNING,
                            customRenderQueue.intValue),
                            MessageType.Warning,
                            true);
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(customRenderQueue);
                        if (GUILayout.Button("Clear", GUILayout.Width(50)))
                            customRenderQueue.intValue = -1;
                    }
                    EditorGUILayout.EndHorizontal();
                    serializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.EndVertical();
            }
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);

            serializedObject.Update();
            SerializedProperty customRenderQueue = serializedObject.FindProperty("m_CustomRenderQueue");

            if (customRenderQueue.intValue != -1 && customRenderQueue.intValue != m.shader.renderQueue)
            {

                Rect rect = new Rect(new Vector2(r.x - 5, r.y), new Vector2(20, 20));

                GUIContent gc = EditorGUIUtility.IconContent(
                            "console.warnicon",
                            string.Format(
                                FORMAT_WARNING,
                                customRenderQueue.intValue));
                UnityEngine.GUI.Label(rect, gc);
            }
        }
    }
}