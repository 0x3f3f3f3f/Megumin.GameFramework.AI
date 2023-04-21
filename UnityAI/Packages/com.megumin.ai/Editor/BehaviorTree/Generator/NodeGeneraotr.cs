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
using Megumin.Reflection;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class NodeGeneraotr : ScriptableObject
    {
        public UnityEngine.Object OutputFolder;
        [Space]
        public bool MultiThreading = true;
        public Enableable<string> Define;
        public List<Enableable<string>> Types = new();

        public List<string> IgnoreMethods = new();

        [Serializable]
        public class IconReplace
        {
            public string Type;
            public string IconPath;
            public Texture2D Icon;
        }

        public List<IconReplace> ReplaceIcon = new();

        [Space]
        public bool GMethods = true;
        public bool GFields = true;
        public bool GProperties = true;

        List<Task> alltask = new();
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

            foreach (var item in ReplaceIcon)
            {
                if (Megumin.Reflection.TypeCache.TryGetType(item.Type, out var type))
                {
                    if (item.Icon)
                    {
                        typeIcon[type] = AssetDatabase.GetAssetPath(item.Icon);
                    }
                    else
                    {
                        typeIcon[type] = item.IconPath;
                    }
                }
            }

            alltask.Clear();

            if (GFields)
            {
                GenerateField(types);
            }

            if (GProperties)
            {
                GenerateProp(types);
            }

            if (GMethods)
            {
                GenerateMethod(all);
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    var count = alltask.Count(elem => elem.IsCompleted);
                    if (count >= alltask.Count)
                    {
                        break;
                    }

                    EditorUtility.DisplayProgressBar("GenerateCode", $"Write files to disk...", (float)count / alltask.Count);
                    await Task.Delay(500);
                }
            });

            Task.WaitAll(alltask.ToArray());
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
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

                //忽略过时API
                var ob = method.GetCustomAttribute<ObsoleteAttribute>();
                if (ob != null)
                {
                    continue;
                }

                //忽略平台不一致API
                var NativeConditionalAttributeType = Megumin.Reflection.TypeCache.GetType("UnityEngine.Bindings.NativeConditionalAttribute");
                var nc = method.GetCustomAttribute(NativeConditionalAttributeType);
                if (nc != null)
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

        public void GenerateMethod(List<(Type type, MethodInfo method)> all)
        {
            int index = 0;
            foreach (var (type, method) in all)
            {
                EditorUtility.DisplayProgressBar("GenerateCode", $"{type.FullName} {method.Name}", (float)index / all.Count);
                GenerateMethod(type, method);
                index++;
            }
        }

        public void GenerateMethod(Type type, MethodInfo method)
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
                    }
                }
            }

            if (MultiThreading)
            {
                var task = Task.Run(() => { GenerateMethod(type, method, path); });
                alltask.Add(task);
            }
            else
            {
                GenerateMethod(type, method, path);
            }
        }

        public string GetClassName(Type type, MemberInfo member)
        {
            var className = $"{type.Name}_{member.Name}";

            if (member is MethodInfo method)
            {
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
            }

            className = className.Replace("[]", "Array");
            return className;
        }

        public string GetMenuName(Type type, MemberInfo member)
        {
            var result = member.Name;

            if (member is MethodInfo method)
            {
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
            }

            return result;
        }

        public void GenerateAttribute(Type type, CSCodeGenerator generator)
        {
            if (typeIcon.TryGetValue(type, out var iconName) && string.IsNullOrEmpty(iconName) == false)
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

        public void AddMacro(Type type, MemberInfo member, CSCodeGenerator generator)
        {
            generator.Macro["$(ClassName)"] = GetClassName(type, member); ;
            generator.Macro["$(ComponentName)"] = type.FullName;
            generator.Macro["$(MenuName)"] = GetMenuName(type, member);
            generator.Macro["$(DisplayName)"] = $"{type.Name}_{member.Name}";
            generator.Macro["$(MemberName)"] = member.Name;
            generator.Macro["$(CodeGenericType)"] = typeof(NodeGeneraotr).FullName;
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

            return GenerateDeclaringResult(method.ReturnType, generator);
        }

        public bool GenerateDeclaringResult(Type type, CSCodeGenerator generator)
        {
            bool saveResult = false;
            if (type != null && type != typeof(void))
            {
                //存在返回值 。储存返回值
                if (TryGetParamType(type, out var returnType))
                {
                    saveResult = true;
                    generator.PushBlankLines();
                    generator.Push($"[Space]");
                    generator.Push($"public {returnType.ToCodeString()} SaveValueTo;");
                }
            }

            return saveResult;
        }

        public void GenerateMethod(Type type, MethodInfo method, string path)
        {
            CSCodeGenerator codeGenerator = new CSCodeGenerator();
            var success = GenerateMethod(type, method, codeGenerator);
            if (success)
            {
                codeGenerator.Generate(path);
            }
        }

        public bool GenerateMethod(Type type, MethodInfo method, CSCodeGenerator generator)
        {
            if (Define.Enabled)
            {
                generator.Push($"#if {Define.Value}");
                generator.PushBlankLines();
            }

            GenerateUsing(generator);

            generator.Push($"namespace Megumin.GameFramework.AI.BehaviorTree");
            using (generator.NewScope)
            {
                var isConditionDecorator = method.ReturnType == typeof(bool);

                GenerateAttribute(type, generator);

                var UseMyAgent = type.IsSubclassOf(typeof(UnityEngine.Component)) || type == typeof(GameObject);

                var BaseType = isConditionDecorator ? "ConditionDecorator" : "BTActionNode";

                if (method.IsStatic || UseMyAgent == false)
                {
                    generator.Push($"public sealed class $(ClassName) : {BaseType}");
                }
                else
                {
                    generator.Push($"public sealed class $(ClassName) : {BaseType}<$(ComponentName)>");
                }

                using (generator.NewScope)
                {
                    //generator.Push($"public string Title => \"$(Title)\";");
                    if (method.IsStatic == false && UseMyAgent == false)
                    {
                        generator.Push($"[Space]");
                        if (TryGetParamType(type, out var paramType))
                        {
                            generator.Push($"public {paramType.ToCodeString()} MyAgent;");
                        }
                        else
                        {
                            generator.Push($"public {type.ToCodeString()} MyAgent;");
                        }
                        generator.PushBlankLines();
                    }

                    bool saveResult = GenerateDeclaringMember(method, generator);

                    generator.PushBlankLines();

                    var @params = method.GetParameters();

                    string BTNodeFrom = "from";
                    if (@params.Any(elem => elem.Name == BTNodeFrom))
                    {
                        BTNodeFrom += "1";
                    }

                    string ObjectOptions = "options";
                    if (@params.Any(elem => elem.Name == ObjectOptions))
                    {
                        ObjectOptions += "1";
                    }

                    if (isConditionDecorator)
                    {
                        generator.Push($"public override bool CheckCondition(object {ObjectOptions} = null)");
                    }
                    else
                    {
                        generator.Push($"protected override Status OnTick(BTNode {BTNodeFrom}, object {ObjectOptions} = null)");
                    }

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
                            callString += $"(({type.FullName})MyAgent).{method.Name}(";
                        }


                        for (int i = 0; i < @params.Length; i++)
                        {
                            if (i != 0)
                            {
                                callString += ", ";
                            }

                            var param = @params[i];
                            if (param.IsOut)
                            {
                                callString += $"out var ";
                            }

                            callString += $"{param.Name}";
                        }
                        callString += ");";

                        generator.Push(callString);

                        if (saveResult)
                        {
                            generator.PushBlankLines();
                            generator.Push($"if (SaveValueTo != null)");
                            using (generator.NewScope)
                            {
                                generator.Push($"SaveValueTo.Value = result;");
                            }
                            generator.PushBlankLines();
                        }

                        if (isConditionDecorator)
                        {
                            generator.Push($"return result;");
                        }
                        else
                        {
                            generator.Push($"return Status.Succeeded;");
                        }
                    }
                }
            }

            if (Define.Enabled)
            {
                generator.PushBlankLines();
                generator.Push($"#endif");
            }

            generator.PushBlankLines(4);

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

    public partial class NodeGeneraotr
    {

        public bool GetBaseTypeString(Type compType, Type memberType, bool useMyAgent, out string baseTypeSting)
        {
            baseTypeSting = "BTActionNode";
            if (memberType == typeof(bool))
            {
                if (useMyAgent)
                {
                    baseTypeSting = "ConditionDecorator<$(ComponentName)>";
                }
                else
                {
                    baseTypeSting = "ConditionDecorator";
                }
            }
            else if (memberType == typeof(string))
            {
                if (useMyAgent)
                {
                    baseTypeSting = "CompareDecorator<$(ComponentName), string>";
                }
                else
                {
                    baseTypeSting = "CompareDecorator<string>";
                }
            }
            else if (memberType == typeof(int))
            {
                if (useMyAgent)
                {
                    baseTypeSting = "CompareDecorator<$(ComponentName), int>";
                }
                else
                {
                    baseTypeSting = "CompareDecorator<int>";
                }
            }
            else if (memberType == typeof(float))
            {
                if (useMyAgent)
                {
                    baseTypeSting = "CompareDecorator<$(ComponentName), float>";
                }
                else
                {
                    baseTypeSting = "CompareDecorator<float>";
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public const string CompareDecoratorBodyTemplate =
@"[Space]
public $(RefVarType) CompareTo;

[Space]
public $(RefVarType) SaveValueTo;

public override $(MemberTyoe) GetResult()
{
    var result = $(MyAgent).$(MemberName);

    if (SaveValueTo != null)
    {
        SaveValueTo.Value = result;
    }

    return result;
}

public override $(MemberTyoe) GetCompareTo()
{
    return CompareTo;
}
";

        public const string BoolDecoratorBodyTemplate =
@"[Space]
public $(RefVarType) SaveValueTo;

public override bool CheckCondition(object options = null)
{
    var result = $(MyAgent).$(MemberName);

    if (SaveValueTo != null)
    {
        SaveValueTo.Value = result;
    }

    return result;
}
";
        public void GenerateProp(HashSet<Type> types)
        {
            foreach (var type in types)
            {
                GenerateProp(type);
            }
        }

        public void GenerateProp(Type type)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).ToList();

            foreach (var prop in props)
            {
                if (prop.DeclaringType != type)
                {
                    continue;
                }

                if (prop.IsSpecialName)
                {
                    continue;
                }

                //忽略过时API
                var ob = prop.GetCustomAttribute<ObsoleteAttribute>();
                if (ob != null)
                {
                    continue;
                }

                //忽略平台不一致API
                var NativeConditionalAttributeType = Megumin.Reflection.TypeCache.GetType("UnityEngine.Bindings.NativeConditionalAttribute");
                var nc = prop.GetCustomAttribute(NativeConditionalAttributeType);
                if (nc != null)
                {
                    continue;
                }

                //忽略指定方法
                var className = GetClassName(type, prop);
                if (IgnoreMethods.Contains(className))
                {
                    continue;
                }

                GenerateMember(type, prop);
            }
        }

        public void GenerateField(HashSet<Type> types)
        {
            foreach (var type in types)
            {
                GenerateField(type);
            }
        }

        public void GenerateField(Type type)
        {
            var props = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).ToList();

            foreach (var prop in props)
            {
                if (prop.DeclaringType != type)
                {
                    continue;
                }

                if (prop.IsSpecialName)
                {
                    continue;
                }

                //忽略过时API
                var ob = prop.GetCustomAttribute<ObsoleteAttribute>();
                if (ob != null)
                {
                    continue;
                }

                //忽略平台不一致API
                var NativeConditionalAttributeType = Megumin.Reflection.TypeCache.GetType("UnityEngine.Bindings.NativeConditionalAttribute");
                var nc = prop.GetCustomAttribute(NativeConditionalAttributeType);
                if (nc != null)
                {
                    continue;
                }

                //忽略指定方法
                var className = GetClassName(type, prop);
                if (IgnoreMethods.Contains(className))
                {
                    continue;
                }

                GenerateMember(type, prop);
            }
        }

        System.Random random = new();
        public void GenerateMember(Type type, MemberInfo prop)
        {
            string className = GetClassName(type, prop);

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
                    }
                }
            }

            EditorUtility.DisplayProgressBar("GenerateCode", $"{type.FullName} {prop.Name}", random.Next(100) / 100f);

            if (MultiThreading)
            {
                var task = Task.Run(() => { GenerateMemberRead(type, prop, path); });
                alltask.Add(task);
            }
            else
            {
                GenerateMemberRead(type, prop, path);
            }
        }

        public void GenerateMemberRead(Type type, MemberInfo prop, string path)
        {
            CSCodeGenerator generator = new();
            var success = GenerateMemberRead(type, prop, generator);
            if (success)
            {
                generator.Generate(path);
            }
        }

        public bool GenerateMemberRead(Type type, MemberInfo member, CSCodeGenerator generator)
        {
            if (Define.Enabled)
            {
                generator.Push($"#if {Define.Value}");
                generator.PushBlankLines();
            }

            GenerateUsing(generator);

            generator.Push($"namespace Megumin.GameFramework.AI.BehaviorTree");
            using (generator.NewScope)
            {
                GenerateAttribute(type, generator);

                var UseMyAgent = type.IsSubclassOf(typeof(UnityEngine.Component)) || type == typeof(GameObject);

                var isStatic = member.IsStaticMember();

                var memberType = member.GetMemberType();
                if (GetBaseTypeString(type, memberType, !isStatic && UseMyAgent, out var baseTypeSting))
                {
                    generator.Push($"public sealed class $(ClassName) : {baseTypeSting}");
                }
                else
                {
                    return false;
                }

                using (generator.NewScope)
                {
                    if (isStatic == false && UseMyAgent == false)
                    {
                        generator.Push($"[Space]");
                        if (TryGetParamType(type, out var paramType))
                        {
                            generator.Push($"public {paramType.ToCodeString()} MyAgent;");
                        }
                        else
                        {
                            generator.Push($"public {type.ToCodeString()} MyAgent;");
                        }

                        generator.PushBlankLines();
                    }

                    if (memberType == typeof(bool))
                    {
                        generator.PushTemplate(BoolDecoratorBodyTemplate);
                    }
                    else
                    {
                        generator.PushTemplate(CompareDecoratorBodyTemplate);
                    }

                    TryGetParamType(memberType, out var returnType);
                    generator.Macro["$(RefVarType)"] = returnType.ToCodeString();

                    generator.Macro["$(MemberTyoe)"] = memberType.ToCodeString();

                    if (isStatic)
                    {
                        generator.Macro["$(MyAgent)"] = "$(ComponentName)";
                    }
                    else
                    {
                        generator.Macro["$(MyAgent)"] = "(($(ComponentName))MyAgent)";
                    }
                }
            }


            if (Define.Enabled)
            {
                generator.PushBlankLines();
                generator.Push($"#endif");
            }

            generator.PushBlankLines(4);

            AddMacro(type, member, generator);

            return true;
        }

    }
}
