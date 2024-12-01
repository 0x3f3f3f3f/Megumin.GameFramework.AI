using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Megumin.Binding;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Megumin.AI.Editor;
using System.ComponentModel;
using System.Threading.Tasks;
using Megumin.Reflection;
using UnityEngine.Serialization;
using System.Text.RegularExpressions;

namespace Megumin.AI.BehaviorTree.Editor
{
    [CreateAssetMenu(fileName = "NodeGenerator", menuName = "Megumin/AI/NodeGenerator")]
    public partial class NodeGenerator : ScriptableObject
    {
        public UnityEngine.Object OutputFolder;

        [Space]
        public bool MultiThreading = true;

        [Space]
        public List<Enable<string>> Assemblys = new()
        {
            new Enable<string>() { Value = "Assembly-CSharp", Enabled = false },
        };

        public List<Enable<string>> Types = new();


        /// <summary>
        /// 节点生成选项
        /// </summary>
        /// <remarks>
        /// 用于解决API废弃，不同版本支持不同的问题
        /// </remarks>
        [Serializable]
        public class GenerateOption
        {
            /// <summary>
            /// 默认生成选项
            /// </summary>
            public static GenerateOption Default { get; } = new();
            //用于匹配的正则表达式
            public string Regex;
            /// <summary>
            /// 忽略生成
            /// </summary>
            public bool IgnoreGenerated = false;
            /// <summary>
            /// 编译宏。
            /// </summary>
            /// <remarks>
            /// 不需要StartEnd，直接拼成一个字符就行了。例如：UNITY_5_3_OR_NEWER && !UNITY_2021_3_OR_NEWER
            /// </remarks>
            public string CompileMacro;
            //public string StartMacro;
            //public string EndMacro;
            public bool ObsoleteGenerated = false;
            public string ObsoleteMessage = "Obsolete API in a future version of Unity";
            public bool ObsoleteError = false;
            public string OverrideIconPath;
            public Texture2D OverrideIconTexture2D;
            public string OverrideCategory;
            public string OverrideDisplayName;

            public Regex RegexInstance { get; internal set; }
            public string OverrideIconTexture2DCacheString { get; set; }
            /// <summary>
            /// 设定的节点图标
            /// </summary>
            public string OverrideIcon
            {
                get
                {
                    if (string.IsNullOrEmpty(OverrideIconPath))
                    {
                        return OverrideIconTexture2DCacheString;
                    }
                    else
                    {
                        return OverrideIconPath;
                    }
                }
            }
        }

        /// <summary>
        /// 根据要生成的节点的名字的生成设置
        /// </summary>
        [Space]
        public List<GenerateOption> ClassNameOptions = new();
        /// <summary>
        /// 根据成员的所在的声明的类型名字的生成设置
        /// </summary>
        public List<GenerateOption> DeclearTypeNameOptions = new();

        [Serializable]
        public class CategoryReplace
        {
            public string oldValue;
            public string newValue;
        }

        [Space]
        public List<CategoryReplace> ReplaceCategory = new();

        [Space]
        public bool Field2GetNode = false;
        public bool Field2SetNode = false;
        public bool Field2Deco = true;

        [Space]
        public bool Proterty2GetNode = false;
        public bool Proterty2SetNode = false;
        public bool Proterty2Deco = true;

        [Space]
        public bool Method2Node = true;
        public bool Method2Deco = false;

        [Space]
        public string OutputNamespace = "Megumin.AI.BehaviorTree";

        List<Task> alltask = new();
        System.Random random = new();
        /// <summary>
        /// 当前插件环境支持的节点参数类型，包含了用户自定义类型
        /// </summary>
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

        /// <summary>
        /// 获取最匹配的生成参数，优先使用生成节点的类名测试，然后使用声明类型测试。
        /// 如果没有找到则返回空。
        /// </summary>
        /// <param name="className"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public GenerateOption FindGenerateOption(string className, Type type)
        {
            foreach (var item in ClassNameOptions)
            {
                if (item.RegexInstance != null && item.RegexInstance.IsMatch(className))
                {
                    return item;
                }
            }

            foreach (var item in DeclearTypeNameOptions)
            {
                if (item.RegexInstance != null && item.RegexInstance.IsMatch(type.FullName))
                {
                    return item;
                }
            }

            return GenerateOption.Default;
        }

        string OutputDir = "";
        [ContextMenu("Generate")]
        public void Generate()
        {
            //获取输出路径
            OutputDir = AssetDatabase.GetAssetPath(OutputFolder);

            //初始化正则表达式
            foreach (var item in ClassNameOptions.Concat(DeclearTypeNameOptions))
            {
                if (!string.IsNullOrEmpty(item.Regex))
                {
                    Regex regex = new Regex(item.Regex);
                    item.RegexInstance = regex;
                }

                if (item.OverrideIconTexture2D)
                {
                    item.OverrideIconTexture2DCacheString = AssetDatabase.GetAssetPath(item.OverrideIconTexture2D);
                }
            }

            //获取支持的参数类型
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

            HashSet<Type> types = ClollectGenrateType();

            //统计Icon列表，计算Icon重写值
            foreach (var item in types)
            {
                typeIcon[item] = AssetPreview.GetMiniTypeThumbnail(item)?.name;
            }

            alltask.Clear();

            List<Generator> generators = ClollectGenerateTask(types);

            foreach (var item in generators)
            {
                item.Generate();
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

        /// <summary>
        /// 收集要生成节点的类型。
        /// </summary>
        /// <returns></returns>
        public HashSet<Type> ClollectGenrateType()
        {
            HashSet<Type> types = new();

            //从程序集中获取
            foreach (var item in Assemblys)
            {
                if (item.Enabled)
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var assm = assemblies.FirstOrDefault(elem => elem.GetName().Name == item.Value);
                    if (assm != null)
                    {
                        foreach (var type in assm.GetTypes())
                        {
                            types.Add(type);
                        }
                    }
                }
            }

            //从配置的名字中获取
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

            return types;
        }

        Dictionary<Type, string> typeIcon = new();


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
                        className += $"_{parameterType.ToIdentifier()}";
                    }
                    //Debug.LogError(className);
                }
            }

            className = className.ToIdentifier();
            return className;
        }

        public const string CompareDecoratorBodyTemplate =
@"[Space]
public $(RefVarType) CompareTo;

[Space]
public $(RefVarType) SaveValueTo;

public override $(MemberType) GetResult()
{
    $(GetValueCode)

    if (SaveValueTo != null)
    {
        SaveValueTo.Value = result;
    }

    return result;
}

public override $(MemberType) GetCompareTo()
{
    return CompareTo;
}
";

        public const string BoolDecoratorBodyTemplate =
@"[Space]
public $(RefVarType) SaveValueTo;

public override bool CheckCondition(object $(ObjectOptions) = null)
{
    $(GetValueCode)

    if (SaveValueTo != null)
    {
        SaveValueTo.Value = result;
    }

    return result;
}
";

        public const string FieldGetValueNodeTemplate =
@"[Space]
public $(RefVarType) SaveValueTo;

protected override Status OnTick(BTNode $(BTNodeFrom), object $(ObjectOptions) = null)
{
    $(GetValueCode)

    if (SaveValueTo != null)
    {
        SaveValueTo.Value = result;
    }

    return Status.Succeeded;
}
";

        public const string FieldSetValueNodeTemplate =
@"[Space]
public $(RefVarType) Value;

protected override Status OnTick(BTNode $(BTNodeFrom), object $(ObjectOptions) = null)
{
    $(GetValueCode)
    return Status.Succeeded;
}
";
        public List<Generator> ClollectGenerateTask(HashSet<Type> types)
        {
            List<Generator> generators = new List<Generator>();

            int index = 0;
            foreach (var type in types)
            {
                var members = type.GetMembers(
                System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance
                | BindingFlags.Static).ToList();

                var progress = (float)index / types.Count;

                for (int i = 0; i < members.Count; i++)
                {
                    MemberInfo member = members[i];
                    if (member.DeclaringType != type)
                    {
                        continue;
                    }

                    EditorUtility.DisplayProgressBar("GenerateCode",
                                                     $"Clollect Generate Task ...  {type.Name}.{member.Name}",
                                                     progress);

                    //忽略过时API
                    var ob = member.GetCustomAttribute<ObsoleteAttribute>();
                    if (ob != null)
                    {
                        continue;
                    }

                    //忽略平台不一致API
                    var NativeConditionalAttributeType = Megumin.Reflection.TypeCache.GetType("UnityEngine.Bindings.NativeConditionalAttribute");
                    var nc = member.GetCustomAttribute(NativeConditionalAttributeType);
                    if (nc != null)
                    {
                        continue;
                    }


                    if (member is FieldInfo filed)
                    {
                        if (filed.IsSpecialName)
                        {
                            continue;
                        }

                        if (Field2Deco)
                        {
                            //忽略指定方法
                            var className = GetClassName(type, member);
                            className += "_Decorator";

                            if (TargetTypeCan2Deco(filed.FieldType) == false)
                            {
                                continue;
                            }

                            var generator = new FeildProperty2DecoGenerator();
                            generator.ClassName = className;
                            generator.IsStatic = filed.IsStatic;
                            generator.MemberInfo = member;
                            generator.Type = type;
                            generator.Setting = this;
                            generator.GenerateOption = FindGenerateOption(className, type);

                            if (generator.CheckPath())
                            {
                                generators.Add(generator);
                            }
                        }

                        if (Field2GetNode)
                        {
                            var className = GetClassName(type, member);
                            className += "_Get_Node";

                            var generator = new FeildProperty2GetNode();
                            generator.ClassName = className;
                            generator.IsStatic = filed.IsStatic;
                            generator.MemberInfo = member;
                            generator.Type = type;
                            generator.Setting = this;
                            generator.GenerateOption = FindGenerateOption(className, type);

                            if (generator.CheckPath())
                            {
                                generators.Add(generator);
                            }
                        }

                        if (Field2SetNode)
                        {
                            if (filed.IsInitOnly)
                            {
                                continue;
                            }

                            if (filed.IsLiteral)
                            {
                                continue;
                            }

                            if (type.IsValueType)
                            {
                                //值类型的成员不要Set。
                                continue;
                            }

                            var className = GetClassName(type, member);
                            className += "_Set_Node";

                            var generator = new FeildProperty2SetNode();
                            generator.ClassName = className;
                            generator.IsStatic = filed.IsStatic;
                            generator.MemberInfo = member;
                            generator.Type = type;
                            generator.Setting = this;
                            generator.GenerateOption = FindGenerateOption(className, type);

                            if (generator.CheckPath())
                            {
                                generators.Add(generator);
                            }
                        }
                    }
                    else if (member is PropertyInfo prop)
                    {
                        if (prop.IsSpecialName)
                        {
                            continue;
                        }

                        bool isIndexer = prop.GetIndexParameters().Length > 0;
                        //忽略索引器属性。
                        if (isIndexer)
                        {
                            continue;
                        }

                        if (Proterty2Deco)
                        {
                            //忽略指定方法
                            var className = GetClassName(type, member);
                            className += "_Decorator";

                            if (TargetTypeCan2Deco(prop.PropertyType) == false)
                            {
                                continue;
                            }

                            if (prop.CanRead)
                            {
                                var generator = new FeildProperty2DecoGenerator();
                                generator.ClassName = className;
                                generator.IsStatic = prop.GetMethod.IsStatic;
                                generator.MemberInfo = member;
                                generator.Type = type;
                                generator.Setting = this;
                                generator.GenerateOption = FindGenerateOption(className, type);

                                if (generator.CheckPath())
                                {
                                    generators.Add(generator);
                                }
                            }
                        }

                        if (Proterty2GetNode)
                        {
                            var className = GetClassName(type, member);
                            className += "_Get_Node";

                            if (prop.CanRead)
                            {
                                var generator = new FeildProperty2GetNode();
                                generator.ClassName = className;
                                generator.IsStatic = prop.GetMethod.IsStatic;
                                generator.MemberInfo = member;
                                generator.Type = type;
                                generator.Setting = this;
                                generator.GenerateOption = FindGenerateOption(className, type);

                                if (generator.CheckPath())
                                {
                                    generators.Add(generator);
                                }
                            }
                        }

                        if (Proterty2SetNode)
                        {
                            var className = GetClassName(type, member);
                            className += "_Set_Node";

                            if (type.IsValueType)
                            {
                                //值类型的成员不要Set。
                                continue;
                            }

                            if (prop.CanWrite)
                            {
                                var generator = new FeildProperty2SetNode();
                                generator.ClassName = className;
                                generator.IsStatic = prop.SetMethod.IsStatic;
                                generator.MemberInfo = member;
                                generator.Type = type;
                                generator.Setting = this;
                                generator.GenerateOption = FindGenerateOption(className, type);

                                if (generator.CheckPath())
                                {
                                    generators.Add(generator);
                                }
                            }
                        }
                    }
                    else if (member is MethodInfo method)
                    {
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


                        if (Method2Node)
                        {
                            var className = GetClassName(type, member);
                            className += "_Node";

                            var generator = new Method2NodeGenerator();
                            generator.ClassName = className;
                            generator.IsStatic = method.IsStatic;
                            generator.MemberInfo = member;
                            generator.Type = type;
                            generator.Setting = this;
                            generator.GenerateOption = FindGenerateOption(className, type);

                            if (generator.CheckPath())
                            {
                                generators.Add(generator);
                            }
                        }

                        if (Method2Deco)
                        {
                            var className = GetClassName(type, member);
                            className += "_Method_Decorator";

                            if (TargetTypeCan2Deco(method.ReturnType) == false)
                            {
                                continue;
                            }

                            var generator = new Method2DecoGenerator();
                            generator.ClassName = className;
                            generator.IsStatic = method.IsStatic;
                            generator.MemberInfo = member;
                            generator.Type = type;
                            generator.Setting = this;
                            generator.GenerateOption = FindGenerateOption(className, type);

                            if (generator.CheckPath())
                            {
                                generators.Add(generator);
                            }
                        }
                    }
                }

                index++;
            }

            return generators;
        }

        public bool TargetTypeCan2Deco(Type type)
        {
            if (type == typeof(bool)
                || type == typeof(string)
                || type == typeof(int)
                || type == typeof(float)
                || type == typeof(double))
            {
                return true;
            }

            return false;
        }


        public abstract class Generator
        {
            public string ClassName { get; internal set; }
            public MemberInfo MemberInfo { get; internal set; }
            public Type Type { get; internal set; }
            public NodeGenerator Setting { get; internal set; }
            public GenerateOption GenerateOption { get; internal set; }

            public bool IsStatic { get; set; }

            public string path;
            public CSCodeGenerator generator = new();

            public bool UseComponent => Type.IsSubclassOf(typeof(UnityEngine.Component)) || Type == typeof(GameObject) || Type.IsInterface;


            public bool CheckPath()
            {
                var fileName = $"{ClassName}.cs";
                var dir = Path.Combine(Setting.OutputDir, Type.Name);
                string filePath = Path.Combine(dir, fileName);

                path = Path.GetFullPath(filePath);

                //检查现有类型是不是在目标位置，如果不是在目标位置表示节点是手动编写的，应该跳过生成。
                if (Megumin.Reflection.TypeCache.TryGetType(
                    $"{Setting.OutputNamespace}.{ClassName}",
                    out var oldType))
                {
                    var script = oldType.GetMonoScript().Result;
                    if (script != null)
                    {
                        var oldPath = AssetDatabase.GetAssetPath(script);
                        oldPath = Path.GetFullPath(oldPath);
                        if (oldPath != path)
                        {
                            Debug.Log($"发现已有脚本文件，跳过生成。 {oldPath}");
                            return false;
                        }
                    }
                }

                return true;
            }

            public void Generate()
            {
                if (GenerateOption != null)
                {
                    if (GenerateOption.IgnoreGenerated)
                    {
                        return;
                    }
                }
                bool useCompileMacro = !string.IsNullOrEmpty(GenerateOption.CompileMacro);
                string compileMacro = GenerateOption.CompileMacro;

                if (useCompileMacro)
                {
                    generator.Push($"#if {compileMacro}");
                    generator.PushBlankLines();
                }

                GenerateCore();

                if (useCompileMacro)
                {
                    generator.PushBlankLines();
                    generator.Push($"#endif");
                }

                generator.PushBlankLines(4);

                generator.Generate(path);
            }

            protected abstract void GenerateCore();

            public void GenerateUsing(CSCodeGenerator generator)
            {
                generator.Push($"using System;");
                generator.Push($"using System.Collections;");
                generator.Push($"using System.Collections.Generic;");
                generator.Push($"using System.ComponentModel;");
                generator.Push($"using UnityEngine;");
                generator.PushBlankLines();
            }

            public void GenerateAttribute(Type type, MemberInfo member, string className, CSCodeGenerator generator)
            {
                if (string.IsNullOrEmpty(GenerateOption.OverrideIcon))
                {
                    //没有设定Icon，使用Unity的类型Icon
                    if (Setting.typeIcon.TryGetValue(type, out var iconName) && string.IsNullOrEmpty(iconName) == false)
                    {
                        generator.Push($"[Icon(\"{iconName}\")]");
                    }
                }
                else
                {
                    //包含重写Icon，直接使用重写Icon
                    generator.Push($"[Icon(\"{GenerateOption.OverrideIcon}\")]");
                }

                generator.Push($"[DisplayName(\"$(DisplayName)\")]");

                var category = $"{type.FullName.Replace('.', '/')}";
                foreach (var item in Setting.ReplaceCategory)
                {
                    if (string.IsNullOrEmpty(item?.oldValue) == false)
                    {
                        category = category.Replace(item.oldValue, item.newValue);
                    }
                }

                generator.Push($"[Category(\"{category}\")]");
                generator.Push($"[AddComponentMenu(\"$(MenuName)\")]");
                generator.Push($"[CodeGeneratorInfo(Name = \"Megumin.CSCodeGenerator\")]");

                //if (Setting.ObsoleteAPIInFuture.Contains(className))
                //{
                //    generator.Push($"[Obsolete(\"Obsolete API in a future version of Unity\", true)]");
                //}

                if (GenerateOption.ObsoleteGenerated)
                {
                    generator.Push($"[Obsolete(\"{GenerateOption.ObsoleteMessage}\", {GenerateOption.ObsoleteError.ToCode()})]");
                }
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

            public void AddMacro()
            {
                var type = Type;
                var member = MemberInfo;

                generator.Macro[CodeGenerator.ClassName] = ClassName;
                generator.Macro[CodeGenerator.Namespace] = Setting.OutputNamespace;
                generator.Macro["$(ComponentName)"] = type.FullName;
                generator.Macro["$(MenuName)"] = GetMenuName(type, member);
                generator.Macro["$(DisplayName)"] = $"{type.Name}_{member.Name}";
                generator.Macro["$(MemberName)"] = member.Name;
                generator.Macro[CodeGenerator.CodeGenericBy] = typeof(NodeGenerator).FullName;
                generator.Macro["$(BTNodeFrom)"] = "from";
                generator.Macro["$(ObjectOptions)"] = "options";

                var memberType = member.GetMemberType();
                generator.Macro["$(MemberType)"] = memberType.ToCode();

                Setting.TryGetParamType(memberType, out var returnType);
                generator.Macro["$(RefVarType)"] = returnType.ToCode();

                generator.Macro[CodeGenerator.SourceFilePath] = AssetDatabase.GetAssetPath(Setting);
            }

            public string GetBaseTypeString(Type memberType, bool useComponent, bool isnode)
            {
                string baseTypeSting = null;
                if (isnode)
                {
                    if (useComponent)
                    {
                        baseTypeSting = "BTActionNode<$(ComponentName)>";
                    }
                    else
                    {
                        baseTypeSting = "BTActionNode";
                    }
                }
                else
                {
                    if (memberType == typeof(bool))
                    {
                        if (useComponent)
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
                        if (useComponent)
                        {
                            baseTypeSting = "CompareDecorator<$(ComponentName), string>";
                        }
                        else
                        {
                            baseTypeSting = "CompareDecorator<string>";
                        }
                    }
                    else if (memberType == typeof(int)
                        || memberType == typeof(float)
                        || memberType == typeof(double))
                    {
                        if (useComponent)
                        {
                            baseTypeSting = $"CompareDecorator<$(ComponentName), {memberType.ToCode()}>";
                        }
                        else
                        {
                            baseTypeSting = $"CompareDecorator<{memberType.ToCode()}>";
                        }
                    }
                }

                return baseTypeSting;
            }

            public void DeclaringMyAgent(Type type)
            {
                if (IsStatic == false && UseComponent == false)
                {
                    generator.Push($"[Space]");
                    if (Setting.TryGetParamType(type, out var paramType))
                    {
                        generator.Push($"public {paramType.ToCode()} MyAgent;");
                    }
                    else
                    {
                        generator.Push($"public {type.ToCode()} MyAgent;");
                    }
                    generator.PushBlankLines();
                }
            }
        }

        public class Method2NodeGenerator : Generator
        {
            public void DeclaringParams(MethodInfo method, CSCodeGenerator generator)
            {
                //声明参数
                var @params = method.GetParameters();
                if (@params.Length > 0)
                {
                    generator.Push($"[Space]");
                }

                foreach (var param in @params)
                {
                    if (Setting.TryGetParamType(param, out var paramType))
                    {
                        generator.Push($"public {paramType.ToCode()} {param.Name};");
                    }
                    else
                    {
                        generator.Push($"public {param.ParameterType.ToCode()} {param.Name};");
                    }
                }

                if (@params.Length > 0)
                {
                    generator.PushBlankLines();
                }
            }

            protected override void GenerateCore()
            {
                string className = ClassName;

                var type = Type;
                var method = MemberInfo as MethodInfo;

                AddMacro();
                GenerateUsing(generator);

                generator.Push($"namespace {CodeGenerator.Namespace}");
                using (generator.NewScope)
                {
                    GenerateAttribute(type, method, className, generator);

                    generator.Push($"public sealed class $(ClassName) : $(BaseClassName)");

                    using (generator.NewScope)
                    {
                        //generator.Push($"public string Title => \"$(Title)\";");
                        DeclaringMyAgent(type);
                        DeclaringParams(method, generator);

                        GenerateMainMethod(type, method);
                    }
                }
            }

            public virtual void GenerateMainMethod(Type type, MethodInfo method)
            {
                bool saveResult = false;
                if (method.ReturnType != null && method.ReturnType != typeof(void))
                {
                    //存在返回值 。储存返回值
                    if (Setting.TryGetParamType(method.ReturnType, out var returnType))
                    {
                        saveResult = true;
                        generator.Push($"[Space]");
                        generator.Push($"public {returnType.ToCode()} SaveValueTo;");
                        generator.PushBlankLines();
                    }
                }

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

                generator.Push($"protected override Status OnTick(BTNode {BTNodeFrom}, object {ObjectOptions} = null)");

                using (generator.NewScope)
                {
                    string callString = GetValueCode(type, method, saveResult);

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

                    generator.Push($"return Status.Succeeded;");
                }

                generator.Macro["$(BaseClassName)"] = GetBaseTypeString(method.ReturnType, UseComponent, true);
            }

            public virtual string GetValueCode(Type type, MethodInfo method, bool saveResult)
            {
                var @params = method.GetParameters();
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
                return callString;
            }
        }

        public class Method2DecoGenerator : Method2NodeGenerator
        {
            public override void GenerateMainMethod(Type type, MethodInfo method)
            {
                if (method.ReturnType == typeof(bool))
                {
                    generator.PushTemplate(BoolDecoratorBodyTemplate);
                }
                else
                {
                    generator.PushTemplate(CompareDecoratorBodyTemplate);
                }

                generator.Macro["$(GetValueCode)"] = GetValueCode(type, method, true);
                generator.Macro["$(BaseClassName)"] = GetBaseTypeString(method.ReturnType, UseComponent, false);
            }
        }

        public class FeildProperty2DecoGenerator : Generator
        {
            protected override void GenerateCore()
            {
                string className = ClassName;
                var type = Type;
                var member = MemberInfo;
                var memberType = member.GetMemberType();

                AddMacro();
                GenerateUsing(generator);

                generator.Push($"namespace {CodeGenerator.Namespace}");
                using (generator.NewScope)
                {
                    GenerateAttribute(type, member, className, generator);

                    generator.Push($"public sealed class $(ClassName) : $(BaseClassName)");

                    using (generator.NewScope)
                    {
                        if (IsStatic == false && UseComponent == false)
                        {
                            generator.Push($"[Space]");
                            if (Setting.TryGetParamType(type, out var paramType))
                            {
                                generator.Push($"public {paramType.ToCode()} MyAgent;");
                            }
                            else
                            {
                                generator.Push($"public {type.ToCode()} MyAgent;");
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

                        if (IsStatic)
                        {
                            generator.Macro["$(GetValueCode)"] = $"var result = $(ComponentName).$(MemberName);";
                        }
                        else
                        {
                            generator.Macro["$(GetValueCode)"] = $"var result = (($(ComponentName))MyAgent).$(MemberName);";
                        }
                    }
                }

                generator.Macro["$(BaseClassName)"] = GetBaseTypeString(memberType, UseComponent, false);
            }
        }

        public class FeildProperty2GetNode : Generator
        {
            protected override void GenerateCore()
            {
                string className = ClassName;
                var type = Type;
                var member = MemberInfo;
                var memberType = member.GetMemberType();

                AddMacro();
                GenerateUsing(generator);

                generator.Push($"namespace {CodeGenerator.Namespace}");
                using (generator.NewScope)
                {
                    GenerateAttribute(type, MemberInfo, className, generator);

                    generator.Push($"public sealed class $(ClassName) : $(BaseClassName)");

                    using (generator.NewScope)
                    {
                        //generator.Push($"public string Title => \"$(Title)\";");
                        DeclaringMyAgent(type);

                        generator.PushTemplate(FieldGetValueNodeTemplate);

                        if (IsStatic)
                        {
                            generator.Macro["$(GetValueCode)"] = $"var result = $(ComponentName).$(MemberName);";
                        }
                        else
                        {
                            generator.Macro["$(GetValueCode)"] = $"var result = (($(ComponentName))MyAgent).$(MemberName);";
                        }
                    }
                }

                generator.Macro["$(BaseClassName)"] = GetBaseTypeString(memberType, UseComponent, true);
                generator.Macro["$(DisplayName)"] = $"Get_{type.Name}_{member.Name}";
                generator.Macro["$(MenuName)"] = $"Get_{GetMenuName(type, member)}";
            }
        }

        public class FeildProperty2SetNode : Generator
        {
            protected override void GenerateCore()
            {
                string className = ClassName;
                var type = Type;
                var member = MemberInfo;
                var memberType = member.GetMemberType();

                AddMacro();
                GenerateUsing(generator);

                generator.Push($"namespace {CodeGenerator.Namespace}");
                using (generator.NewScope)
                {
                    GenerateAttribute(type, MemberInfo, className, generator);

                    generator.Push($"public sealed class $(ClassName) : $(BaseClassName)");

                    using (generator.NewScope)
                    {
                        //generator.Push($"public string Title => \"$(Title)\";");
                        DeclaringMyAgent(type);

                        generator.PushTemplate(FieldSetValueNodeTemplate);

                        if (IsStatic)
                        {
                            generator.Macro["$(GetValueCode)"] = $"$(ComponentName).$(MemberName) = Value;";
                        }
                        else
                        {
                            generator.Macro["$(GetValueCode)"] = $"(($(ComponentName))MyAgent).$(MemberName) = Value;";
                        }
                    }
                }

                generator.Macro["$(BaseClassName)"] = GetBaseTypeString(memberType, UseComponent, true);
                generator.Macro["$(DisplayName)"] = $"Set_{type.Name}_{member.Name}";
                generator.Macro["$(MenuName)"] = $"Set_{GetMenuName(type, member)}";
            }
        }
    }
}
