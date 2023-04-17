using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Megumin.Binding;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Megumin.GameFramework.AI.Editor;
using System.ComponentModel;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class TaskGeneraotr : ScriptableObject
    {
        public UnityEngine.Object OutputFolder;

        [Editor]
        public void Test()
        {
            CodeGeneratorExtension_B2F1FF890B2949E1B0431530F1D90322.Test();
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            var list = VariableCreator.AllCreator;
            foreach (var item in list)
            {
                if (item.IsSeparator)
                {
                    continue;
                }
                var v = item.Create();
                variableTemplate.Add(v);
            }

            List<Type> types = new()
            {
                typeof(NavMeshAgent),
                typeof(Animator),
            };

            foreach (var item in types)
            {
                GenerateType(item);
            }

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
                //Debug.Log(m.ToStringReflection());

                if (m.DeclaringType != type)
                {
                    continue;
                }

                if (m.IsSpecialName)
                {
                    continue;
                }

                if (m.IsGenericMethod)
                {
                    if (m.ContainsGenericParameters == false || m.ReturnType.IsGenericParameter)
                    {
                        //忽略泛型方法
                        continue;
                    }
                }

                var ob = m.GetCustomAttribute<ObsoleteAttribute>();
                if (ob != null)
                {
                    continue;
                }

                GenerateMethod(type, m);
            }
        }

        public void GenerateMethod(Type type, MethodInfo method)
        {
            var className = $"{type.Name}_{method.Name}";
            var fileName = $"{type.Name}_{method.Name}.cs";
            var dir = AssetDatabase.GetAssetPath(OutputFolder);

            dir = Path.Combine(dir, type.Name);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string filePath = Path.Combine(dir, fileName);
            var path = Path.GetFullPath(filePath);

            //检查现有类型是不是在目标位置，如果不是在目标位置表示节点是手动编写的，应该跳过生成。
            if (Megumin.Reflection.TypeCache.TryGetType($"Megumin.GameFramework.AI.BehaviorTree.{className}", out var oldType))
            {
                var script = Megumin.GameFramework.AI.Editor.Utility.GetMonoScript(oldType).Result;
                if (script != null)
                {
                    var oldPath = AssetDatabase.GetAssetPath(script);
                    oldPath = Path.GetFullPath(oldPath);
                    if (oldPath != path)
                    {
                        Debug.Log($"发现已有脚本文件，跳过生成。 {oldPath}");
                        return;
                    }
                }
            }


            CSCodeGenerator codeGenerator = new CSCodeGenerator();
            var success = GenerateCode(type, method, codeGenerator);

            if (success)
            {

                codeGenerator.Generate(path);
            }
        }

        public bool GenerateCode(Type type, MethodInfo method, CSCodeGenerator generator)
        {
            if (method.ReturnType == typeof(bool))
            {
                //返回值是bool，生成条件装饰器节点。
                return GenerateConditionDecorator(type, method, generator);
            }
            else
            {
                return GeneraoteBTActionNode(type, method, generator);
            }
        }

        public bool GenerateConditionDecorator(Type type, MethodInfo method, CSCodeGenerator generator)
        {
            GenerateUsing(generator);

            generator.Push($"namespace Megumin.GameFramework.AI.BehaviorTree");
            using (generator.NewScope)
            {
                GenerateAttribute(type, generator);
                generator.Push($"public class $(ClassName) : ConditionDecorator<$(ComponentName)>");
                using (generator.NewScope)
                {
                    //generator.Push($"public string Title => \"$(Title)\";");

                    //声明参数
                    var @params = method.GetParameters();
                    foreach (var param in @params)
                    {
                        if (TryGetParamType(param, out var paramType))
                        {
                            generator.Push($"public {paramType.ToCodeString()} {param.Name};");
                        }
                        else
                        {
                            //参数类型不支持这个方法，不能生成节点。
                            return false;
                        }
                    }

                    generator.PushBlankLines();
                    generator.Push($"public override bool CheckCondition(object options = null)");
                    using (generator.NewScope)
                    {
                        //MyAgent.CalculatePath(targetPosition, path);
                        var callString = $"return MyAgent.{method.Name}(";
                        for (int i = 0; i < @params.Length; i++)
                        {
                            if (i != 0)
                            {
                                callString += ", ";
                            }

                            var param = @params[i];
                            if (param.IsOut)
                            {
                                callString += $"out var {param.Name}";
                            }
                            else
                            {
                                callString += $"{param.Name}";
                            }
                        }
                        callString += ");";

                        generator.Push(callString);
                    }
                }
            }

            generator.PushBlankLines(4);
            AddMacro(type, method, generator);
            return true;
        }

        public void GenerateAttribute(Type type, CSCodeGenerator generator)
        {
            generator.Push($"[DisplayName(\"$(DisplayName)\")]");
            generator.Push($"[Category(\"Unity/{type.Name}\")]");
            generator.Push($"[AddComponentMenu(\"$(MethodName)\")]");
        }

        public void GenerateUsing(CSCodeGenerator generator)
        {
            generator.Push($"using System.Collections;");
            generator.Push($"using System.Collections.Generic;");
            generator.Push($"using System.ComponentModel;");
            generator.Push($"using UnityEngine;");
            generator.PushBlankLines();
        }

        public void AddMacro(Type type, MethodInfo method, CSCodeGenerator generator)
        {
            var className = $"{type.Name}_{method.Name}";
            generator.Macro["$(ClassName)"] = className;
            generator.Macro["$(ComponentName)"] = type.FullName;
            generator.Macro["$(MethodName)"] = className;
            generator.Macro["$(DisplayName)"] = className;
        }

        public bool GeneraoteBTActionNode(Type type, MethodInfo method, CSCodeGenerator generator)
        {
            GenerateUsing(generator);

            generator.Push($"namespace Megumin.GameFramework.AI.BehaviorTree");
            using (generator.NewScope)
            {
                GenerateAttribute(type, generator);
                generator.Push($"public class $(ClassName) : BTActionNode<$(ComponentName)>");
                using (generator.NewScope)
                {
                    //generator.Push($"public string Title => \"$(Title)\";");

                    //声明参数
                    var @params = method.GetParameters();
                    foreach (var param in @params)
                    {
                        if (TryGetParamType(param, out var paramType))
                        {
                            generator.Push($"public {paramType.ToCodeString()} {param.Name};");
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
                            if (i != 0)
                            {
                                callString += ", ";
                            }

                            var param = @params[i];
                            if (param.IsOut)
                            {
                                callString += $"out var {param.Name}";
                            }
                            else
                            {
                                callString += $"{param.Name}";
                            }
                        }
                        callString += ");";

                        generator.Push(callString);
                        generator.Push($"return Status.Succeeded;");
                    }
                }
            }

            generator.PushBlankLines(4);
            AddMacro(type, method, generator);
            return true;
        }

        List<object> variableTemplate = new();
        public bool TryGetParamType(ParameterInfo param, out Type type)
        {
            type = param.ParameterType;

            foreach (var item in variableTemplate)
            {
                if (item is IVariableSpecializedType variableSpecialized)
                {
                    if (variableSpecialized.SpecializedType == param.ParameterType)
                    {
                        type = item.GetType();
                        return true;
                    }
                }
            }

            if (param.ParameterType.IsEnum || param.ParameterType.IsValueType)
            {
                type = typeof(RefVar<>).MakeGenericType(param.ParameterType);
                return true;
            }

            Debug.Log($"{param.Member.Name} 不支持参数 {param.Name} {param.ParameterType}");
            return false;
        }
    }
}
