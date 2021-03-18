// ------------------------------------------------------------------------------
//  Copyright © 2016 Schell Games, LLC. All Rights Reserved.
//
//  Contact: Tim Sweeney
//
//  Created: 4/27/2016 3:47:17 PM
// ------------------------------------------------------------------------------

using System;
using SG.Core;
using UnityEditor;
using UnityEngine;

namespace SG.Dynamics
{
    [CreateAssetMenu(fileName = "DynamicSoGenerator.asset", menuName = "Framework/Dynamics/Dynamic So Generator")]
    public class DynamicSoGenerator : ScriptableObject
    {
        [SerializeField]
        private MonoScript[] _monoScripts;

        [SerializeField]
        private string[] _types;

        [SerializeField]
        private string _contactName = "Contact Name";

        [SerializeField]
        private string _directory = "Assets/Code/Dynamics/Editor/Generated/";

        [SerializeField]
        private string _namespace = "SG.Dynamics";

        [ContextMenu("Generate Dynamics")]
        public void GenerateDynamics()
        {
            AssetDatabase.StartAssetEditing();
            DynamicSoDefinition generator = new DynamicSoDefinition();

            foreach (MonoScript monoScript in _monoScripts)
            {
                if (!monoScript)
                    continue;

                Type parameter = monoScript.GetClass();
                if (parameter == null)
                    continue;

                Debug.Log("Generating DynamicSo for " + parameter.FullName);
                generator.Generate(name, _contactName, _directory, _namespace, parameter);
            }

            foreach (string typename in _types)
            {
                Type parameter = Type.GetType(typename);
                if (parameter == null)
                    continue;

                Debug.Log("Generating DynamicSo for " + parameter.FullName);
                generator.Generate(name, _contactName, _directory, _namespace, parameter);
            }
            AssetDatabase.StopAssetEditing();
        }
    }

    public class DynamicSoDefinition : CodeGenerationDefinition
    {
        public void Generate(string source, string contactName, string directory, string destNamespace, Type typeParameter)
        {
            GeneratedScript = null;
            ContactName = contactName;
            Directory = directory;
            Namespace = destNamespace;
            TypeName = "Dynamic" + CleanForIdentifier(typeParameter.FullName);

            string contents = string.Format(CONTENTS, Namespace, TypeName, typeParameter.FullName);
            GenerateCode(source, contents);
        }

        // string.Format Parameter semantics
        // 0 : namespace
        // 1 : typename
        // 2 : templated class
        private const string CONTENTS =
@"namespace {0}
{{
    public class {1} : SG.Dynamics.DynamicSo<{2}> {{}}
}}";
    }
}
