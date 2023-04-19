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
        [Space]
        public bool MultiThreading = true;
        public Enableable<string> Define;
        public List<Enableable<string>> Types = new();

        public List<string> IgnoreMethods = new();

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

            HashSet<Type> types = new();

            foreach (var item in Types)
            {
                if (item.Enabled)
                {
                    if (Megumin.Reflection.TypeCache.TryGetType(item, out var type))
                    {
                        types.Add(type);
                    }
                }
            }

            List<(Type type, MethodInfo method)> all = new();
            foreach (var item in types)
            {
                typeIcon[item] = AssetPreview.GetMiniTypeThumbnail(item)?.name;
                ClollectMethod(item, all);
            }

            Generate(all);
        }

        Dictionary<Type, string> typeIcon = new();
        ///// <summary>
        ///// 重载函数个数
        ///// </summary>
        //Dictionary<string, int> permethodCount = new();
        public void ClollectMethod(Type type, List<(Type type, MethodInfo method)> all)
        {
            //TODO Static。
            var methods = type.GetMethods(
                System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance
                | BindingFlags.Static).ToList();

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

                //检查参数是否支持转化为节点
                var @params = method.GetParameters();
                bool supportParams = true;
                if (method.ReturnType != typeof(void) && TryGetParamType(method.ReturnType, out var _) == false)
                {
                    supportParams &= false;
                }

                foreach (var item in @params)
                {
                    if (TryGetParamType(item, out var _) == false)
                    {
                        supportParams &= false;
                        break;
                    }
                }

                if (supportParams == false)
                {
                    continue;
                }

                //忽略指定方法
                var className = GetClassName(type, method);
                if (IgnoreMethods.Contains(className))
                {
                    continue;
                }

                all.Add((type, method));
                //var className = $"{type.Name}_{method.Name}";
                //if (permethodCount.ContainsKey(className))
                //{
                //    permethodCount[className] += 1;
                //}
                //else
                //{
                //    permethodCount[className] = 1;
                //}
            }
        }

        public async void Generate(List<(Type type, MethodInfo method)> all)
        {
            List<Task> alltask = new();
            int index = 0;
            foreach (var (type, method) in all)
            {
                if (MultiThreading)
                {
                    var task = GenerateMethod(type, method);
                    EditorUtility.DisplayProgressBar("GenerateCode", $"{type.FullName} {method.Name}", (float)index / all.Count);
                    alltask.Add(task);
                    index++;
                }
                else
                {
                    await GenerateMethod(type, method);
                    EditorUtility.DisplayProgressBar("GenerateCode", $"{type.FullName} {method.Name}", (float)index / all.Count);
                    //alltask.Add(task);
                    index++;
                }
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

            if (MultiThreading)
            {
                return Task.Run(() => { GenerateCode(type, method, path); });
            }
            else
            {
                GenerateCode(type, method, path);
                return Task.CompletedTask;
            }
        }

        public string GetClassName(Type type, MethodInfo method)
        {
            var className = $"{type.Name}_{method.Name}";
            var @params = method.GetParameters();
            if (@params.Length > 0)
            {
                for (int i = 0; i < @params.Length; i++)
                {
                    Type parameterType = @params[i].ParameterType;
                    className += $"_{parameterType.ToValidVariableName()}";
                }
                //Debug.LogError(className);
            }
            className = className.Replace("[]", "Array");
            return className;
        }

        public string GetMenuName(Type type, MethodInfo method)
        {
            var result = method.Name;
            var @params = method.GetParameters();
            if (@params.Count() > 0)
            {
                result += "(";
                for (int i = 0; i < @params.Length; i++)
                {
                    if (i != 0)
                    {
                        result += ", ";
                    }
                    result += $"{@params[i].ParameterType.Name}";
                }
                result += ")";
                //Debug.LogError(className);
            }

            return result;
        }

        public void GenerateAttribute(Type type, CSCodeGenerator generator)
        {
            if (typeIcon.TryGetValue(type, out var iconName))
            {
                generator.Push($"[Icon(\"{iconName}\")]");
            }
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
            generator.Macro["$(MenuName)"] = GetMenuName(type, method);
            generator.Macro["$(DisplayName)"] = $"{type.Name}_{method.Name}";
        }

        public bool GenerateDeclaringMember(MethodInfo method, CSCodeGenerator generator)
        {
            //声明参数
            var @params = method.GetParameters();
            if (@params.Length > 0)
            {
                generator.Push($"[Space]");
            }

            foreach (var param in @params)
            {
                if (TryGetParamType(param, out var paramType))
                {
                    generator.Push($"public {paramType.ToCodeString()} {param.Name};");
                }
                else
                {
                    generator.Push($"public {param.ParameterType.ToCodeString()} {param.Name};");
                }
            }

            bool saveResult = false;
            if (method.ReturnType != null && method.ReturnType != typeof(void))
            {
                //存在返回值 。储存返回值
                if (TryGetParamType(method.ReturnType, out var returnType))
                {
                    saveResult = true;
                    generator.PushBlankLines();
                    generator.Push($"public {returnType.ToCodeString()} Result;");
                }
            }

            return saveResult;
        }

        public void GenerateCode(Type type, MethodInfo method, string path)
        {
            CSCodeGenerator codeGenerator = new CSCodeGenerator();

            if (Define.Enabled)
            {
                codeGenerator.Push($"#if {Define.Value}");
                codeGenerator.PushBlankLines();
            }
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

            if (Define.Enabled)
            {
                codeGenerator.PushBlankLines();
                codeGenerator.Push($"#endif");
            }

            codeGenerator.PushBlankLines(4);

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

                if (method.IsStatic)
                {
                    generator.Push($"public sealed class $(ClassName) : ConditionDecorator");
                }
                else
                {
                    generator.Push($"public sealed class $(ClassName) : ConditionDecorator<$(ComponentName)>");
                }

                using (generator.NewScope)
                {
                    //generator.Push($"public string Title => \"$(Title)\";");
                    bool saveResult = GenerateDeclaringMember(method, generator);

                    generator.PushBlankLines();
                    generator.Push($"public override bool CheckCondition(object options = null)");
                    using (generator.NewScope)
                    {
                        //MyAgent.CalculatePath(targetPosition, path);
                        var callString = "";
                        callString += "var result = ";

                        if (method.IsStatic)
                        {
                            callString += $"{type.FullName}.{method.Name}(";
                        }
                        else
                        {
                            callString += $"MyAgent.{method.Name}(";
                        }

                        var @params = method.GetParameters();
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

                        if (saveResult)
                        {
                            generator.PushBlankLines();
                            generator.Push($"if (Result != null)");
                            using (generator.NewScope)
                            {
                                generator.Push($"Result.Value = result;");
                            }
                            generator.PushBlankLines();
                        }

                        generator.Push($"return result;");
                    }
                }
            }

            AddMacro(type, method, generator);
            return true;
        }

        public bool GeneraoteBTActionNode(Type type, MethodInfo method, CSCodeGenerator generator)
        {
            GenerateUsing(generator);

            generator.Push($"namespace Megumin.GameFramework.AI.BehaviorTree");
            using (generator.NewScope)
            {
                GenerateAttribute(type, generator);

                if (method.IsStatic)
                {
                    generator.Push($"public sealed class $(ClassName) : BTActionNode");
                }
                else
                {
                    generator.Push($"public sealed class $(ClassName) : BTActionNode<$(ComponentName)>");
                }

                using (generator.NewScope)
                {
                    //generator.Push($"public string Title => \"$(Title)\";");
                    bool saveResult = GenerateDeclaringMember(method, generator);

                    generator.PushBlankLines();
                    generator.Push($"protected override Status OnTick(BTNode from, object options = null)");
                    using (generator.NewScope)
                    {
                        //MyAgent.CalculatePath(targetPosition, path);
                        var callString = "";
                        if (saveResult)
                        {
                            callString += "var result = ";
                        }

                        if (method.IsStatic)
                        {
                            callString += $"{type.FullName}.{method.Name}(";
                        }
                        else
                        {
                            callString += $"MyAgent.{method.Name}(";
                        }

                        var @params = method.GetParameters();
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

                        if (saveResult)
                        {
                            generator.PushBlankLines();
                            generator.Push($"if (Result != null)");
                            using (generator.NewScope)
                            {
                                generator.Push($"Result.Value = result;");
                            }
                            generator.PushBlankLines();
                        }

                        generator.Push($"return Status.Succeeded;");
                    }
                }
            }

            AddMacro(type, method, generator);
            return true;
        }

        List<object> variableTemplate = new();
        public bool TryGetParamType(ParameterInfo param, out Type type)
        {
            Type parameterType = param.ParameterType;
            return TryGetParamType(parameterType, out type);
        }

        public bool TryGetParamType(Type parameterType, out Type type)
        {
            type = parameterType;

            if (type == typeof(void))
            {
                return true;
            }

            foreach (var item in variableTemplate)
            {
                if (item is IVariableSpecializedType variableSpecialized)
                {
                    if (variableSpecialized.SpecializedType == parameterType)
                    {
                        type = item.GetType();
                        return true;
                    }
                }
            }

            if (parameterType.IsEnum || parameterType.IsValueType)
            {
                type = typeof(RefVar<>).MakeGenericType(parameterType);
                return true;
            }

            Debug.Log($"不支持参数类型  {parameterType}");
            return false;
        }
    }
}
