using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Megumin.GameFramework.AI.Editor
{
    public class ParamDataGenerator : ScriptableObject
    {
        public UnityEngine.Object Folder;

        List<string> Types = new()
        {
            "int",
            "float",
            "double",
            "long",
            "string",
            "UnityEngine.Object",
        };

        [Editor]
        public void Generate()
        {
            CSCodeGenerator generator = new();

            generator.Push($"using System;");
            generator.PushBlankLines();

            generator.Push($"namespace Megumin.Serialization");
            using (generator.NewScope)
            {
                foreach (string type in Types)
                {
                    var typeP = type.Substring(0, 1).ToUpper() + type.Substring(1);
                    typeP = typeP.Replace(".", "");

                    generator.Push($"[Serializable]");
                    generator.Push($"public class {typeP}ParameterData : GenericSerializationData<{type}> {{ }}");
                    generator.PushBlankLines();
                }
            }

            var fileName = "GenericSpecializationParameterData.cs";
            var dir = AssetDatabase.GetAssetPath(Folder);
            string filePath = Path.Combine(dir, fileName);
            var path = Path.GetFullPath(filePath);
            Debug.Log(path);
            generator.Generate(path);

            //Open
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
            AssetDatabase.OpenAsset(script);
        }
    }
}
