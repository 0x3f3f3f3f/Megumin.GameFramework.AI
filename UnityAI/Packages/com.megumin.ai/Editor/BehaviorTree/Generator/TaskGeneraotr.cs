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
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class TaskGeneraotr : ScriptableObject
    {
        public UnityEngine.Object OutputFolder;

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

            List<(Type type, MethodInfo method)> all = new();
            foreach (var item in types)
            {
                ClollectMethod(item, all);
            }

            Generate(all);
        }

        Dictionary<string, int> permethodCount = new Dictionary<string, int>();
        public void ClollectMethod(Type type, List<(Type type, MethodInfo method)> all)
        {
            if (!type.IsSubclassOf(typeof(UnityEngine.Component)))
            {
                return;
            }

            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).ToList();

            for (int i = 0; i < methods.Count; i++)
            {
                var method = methods[i];
                //Debug.Log(m.ToStringReflection());

                if (method.DeclaringType != type)
                {
                    continue;
                }

                if (method.IsSpecialName)
                {
                    continue;
                }

                if (method.IsGenericMethod)
                {
                    //忽略泛型方法
                    continue;
                    //if (method.ContainsGenericParameters == false || method.ReturnType.IsGenericParameter)
                    //{
                        
                    //}
                }

                var ob = method.GetCustomAttribute<ObsoleteAttribute>();
                if (ob != null)
                {
                    continue;
                }

                all.Add((type, method));
                var className = $"{type.Name}_{method.Name}";
                if (permethodCount.ContainsKey(className))
                {
                    permethodCount[className] += 1;
                }
                else
                {
                    permethodCount[className] = 1;
                }
            }
        }

        public async void Generate(List<(Type type, MethodInfo method)> all)
        {
            List<Task> alltask = new();
            int index = 0;
            foreach (var (type, method) in all)
            {
                await GenerateMethod(type, method);
                EditorUtility.DisplayProgressBar("GenerateCode", $"{type.FullName} {method.Name}", (float)index / all.Count);
                //alltask.Add(task);
                index++;
            }

            Task.WaitAll(alltask.ToArray());

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public Task GenerateMethod(Type type, MethodInfo method)
        {
            string className = GetClassName(type, method);

            var fileName = $"{className}.cs";
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
                        return Task.CompletedTask;
                    }
                }
            }

            return Task.Run(() => { GenerateCode(type, method, path); });
        }

        public string GetClassName(Type type, MethodInfo method)
        {
            var className = $"{type.Name}_{method.Name}";
            var count = permethodCount[className];
            if (count > 1)
            {
                var @params = method.GetParameters();
                for (int i = 0; i < @params.Length; i++)
                {
                    className += $"_{@params[i].ParameterType.Name}";
                }

                Debug.LogError(className);
            }

            return className;
        }

        public void GenerateCode(Type type, MethodInfo method, string path)
        {
            CSCodeGenerator codeGenerator = new CSCodeGenerator();
            var success = false;

            if (method.ReturnType == typeof(bool))
            {
                //返回值是bool，生成条件装饰器节点。
                success = GenerateConditionDecorator(type, method, codeGenerator);
            }
            else
            {
                success = GeneraoteBTActionNode(type, method, codeGenerator);
            }

            if (success)
            {
                codeGenerator.Generate(path);
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
            generator.Push($"[AddComponentMenu(\"$(MenuName)\")]");
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
            generator.Macro["$(ClassName)"] = GetClassName(type, method); ;
            generator.Macro["$(ComponentName)"] = type.FullName;
            generator.Macro["$(MenuName)"] = $"{type.Name}_{method.ToString()}";
            generator.Macro["$(DisplayName)"] = $"{type.Name}_{method.Name}";
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
