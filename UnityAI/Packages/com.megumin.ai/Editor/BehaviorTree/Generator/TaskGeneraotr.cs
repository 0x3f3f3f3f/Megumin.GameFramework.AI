using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Unity.VisualScripting.YamlDotNet.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class TaskGeneraotr : ScriptableObject
    {
        public UnityEngine.Object OutputFolder;

        [ContextMenu("Generate")]
        public void Generate()
        {
            Type type = typeof(NavMeshAgent);
            var mes = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var m in mes)
            {
                Debug.Log(m.ToStringReflection());

                if (m.DeclaringType != type)
                {
                    continue;
                }

                if (m.IsSpecialName)
                {
                    continue;
                }



                CSCodeGenerator codeGenerator = new CSCodeGenerator();
                string className = GenerateCode(type, m, codeGenerator);

                var fileName = $"{className}.cs";
                var dir = AssetDatabase.GetAssetPath(OutputFolder);
                string filePath = Path.Combine(dir, fileName);
                var path = Path.GetFullPath(filePath);
                codeGenerator.Generate(path);
            }

            AssetDatabase.Refresh();
        }

        private string GenerateCode(Type type, MethodInfo m, CSCodeGenerator generator)
        {
            generator.Push($"using System.Collections;");
            generator.Push($"using System.Collections.Generic;");
            generator.Push($"using System.ComponentModel;");
            generator.Push($"using UnityEngine;");
            generator.PushBlankLines();

            generator.Push($"namespace Megumin.GameFramework.AI.BehaviorTree");
            using (generator.NewScope)
            {
                generator.Push($"public class $(ClassName) : BTActionNode<$(ComponentName)>");
                using (generator.NewScope)
                {

                }
            }

            generator.PushBlankLines(4);

            var className = $"{type.Name}_{m.Name}";
            generator.Macro["$(ClassName)"] = className;
            generator.Macro["$(ComponentName)"] = type.FullName;
            return className;
        }

    }
}
