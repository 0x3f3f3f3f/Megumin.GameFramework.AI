using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            GenerateType(type);

            AssetDatabase.Refresh();
        }

        public void GenerateType(Type type)
        {
            if (!type.IsSubclassOf(typeof(UnityEngine.Component)))
            {
                return;
            }

            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).ToList();

            foreach (var m in methods)
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

                var ob = m.GetCustomAttribute<ObsoleteAttribute>();
                if (ob != null)
                {
                    continue;
                }

                CSCodeGenerator codeGenerator = new CSCodeGenerator();
                var success = GenerateCode(type, m, codeGenerator);

                if (success)
                {
                    var fileName = $"{type.Name}_{m.Name}.cs";
                    var dir = AssetDatabase.GetAssetPath(OutputFolder);

                    dir = Path.Combine(dir, type.Name);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    string filePath = Path.Combine(dir, fileName);
                    var path = Path.GetFullPath(filePath);
                    codeGenerator.Generate(path);
                }
            }
        }

        private bool GenerateCode(Type type, MethodInfo method, CSCodeGenerator generator)
        {
            generator.Push($"using System.Collections;");
            generator.Push($"using System.Collections.Generic;");
            generator.Push($"using System.ComponentModel;");
            generator.Push($"using UnityEngine;");
            generator.PushBlankLines();

            generator.Push($"namespace Megumin.GameFramework.AI.BehaviorTree");
            using (generator.NewScope)
            {
                generator.Push($"[Category(\"Unity/{type.Name}\")]");
                generator.Push($"public class $(ClassName) : BTActionNode<$(ComponentName)>");
                using (generator.NewScope)
                {
                    //声明参数
                    var @params = method.GetParameters();
                    foreach (var param in @params)
                    {
                        if (TryGetParamType(param, out var paramType))
                        {
                            generator.Push($"public {paramType.FullName} {param.Name};");
                        }
                        else
                        {
                            //参数类型不支持这个方法，不能生成节点。
                            return false;
                        }
                    }

                    generator.PushBlankLines();
                    generator.Push($"protected override Status OnTick(BTNode from, object options = null)");
                    using (generator.NewScope)
                    {
                        //MyAgent.CalculatePath(targetPosition, path);
                        var callString = $"MyAgent.{method.Name}(";
                        for (int i = 0; i < @params.Length; i++)
                        {
                            callString += $"{@params[i].Name}";
                            if (i < @params.Length - 1)
                            {
                                callString += ", ";
                            }
                        }
                        callString += ");";

                        generator.Push(callString);
                        generator.Push($"return Status.Succeeded;");
                    }
                }
            }

            generator.PushBlankLines(4);

            var className = $"{type.Name}_{method.Name}";
            generator.Macro["$(ClassName)"] = className;
            generator.Macro["$(ComponentName)"] = type.FullName;
            return true;
        }

        private bool TryGetParamType(ParameterInfo param, out Type type)
        {
            type = param.ParameterType;
            if (param.IsOut)
            {
                return false;
            }
            return true;
        }
    }
}
