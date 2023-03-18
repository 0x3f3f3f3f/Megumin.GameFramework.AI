using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.Serialization
{
    /// <summary>
    /// 第一次调用会导致卡顿，在调用前使用多线程初始化，防止阻塞主线程。
    /// </summary>
    public static class TypeCache
    {

#if UNITY_5_3_OR_NEWER

        static readonly Dictionary<string, Type> hotComponentType = new();
        public static Type GetComponentType(string typeFullName, bool forceRecache = false)
        {
            TryGetComponentType(typeFullName, out var type, forceRecache);
            return type;
        }

        public static bool TryGetComponentType(string typeFullName, out Type type, bool forceRecache = false)
        {
            if (string.IsNullOrEmpty(typeFullName))
            {
                type = null;
                return false;
            }

            if (hotComponentType.TryGetValue(typeFullName, out type))
            {
                return true;
            }
            else
            {
                CacheAllTypes(forceRecache);
                if (allComponentType.TryGetValue(typeFullName, out type))
                {
                    hotComponentType[typeFullName] = type;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        static readonly Dictionary<string, Type> hotUnityObjectType = new();
        public static Type GetUnityObjectType(string typeFullName, bool forceRecache = false)
        {
            TryGetUnityObjectType(typeFullName, out var type, forceRecache);
            return type;
        }

        public static bool TryGetUnityObjectType(string typeFullName, out Type type, bool forceRecache = false)
        {

            if (string.IsNullOrEmpty(typeFullName))
            {
                type = null;
                return false;
            }

            if (hotUnityObjectType.TryGetValue(typeFullName, out type))
            {
                return true;
            }
            else
            {
                CacheAllTypes(forceRecache);
                if (allUnityObjectType.TryGetValue(typeFullName, out type))
                {
                    hotUnityObjectType[typeFullName] = type;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

#endif

        static readonly Dictionary<string, Type> hotType = new();
        public static Type GetType(string typeFullName, bool forceRecache = false)
        {
            TryGetType(typeFullName, out var type, forceRecache);
            return type;
        }

        static readonly Unity.Profiling.ProfilerMarker tryGetTypeMarker = new(nameof(TryGetType));
        public static bool TryGetType(string typeFullName, out Type type, bool forceRecache = false)
        {
            using var profiler = tryGetTypeMarker.Auto();

            if (string.IsNullOrEmpty(typeFullName))
            {
                type = null;
                return false;
            }

            if (hotType.TryGetValue(typeFullName, out type))
            {
                return true;
            }
            else
            {
                CacheAllTypes(forceRecache);
                if (allType.TryGetValue(typeFullName, out type))
                {
                    hotType[typeFullName] = type;
                    return true;
                }
                else
                {
                    //制作泛型类
                    if (TryMakeGenericType(typeFullName, out type))
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        public static bool TryGetType(List<string> typeFullName,
                                      out Type[] types,
                                      bool forceRecache = false)
        {
            types = new Type[typeFullName.Count];
            for (int i = 0; i < typeFullName.Count; i++)
            {
                if (TryGetType(typeFullName[i], out var type, forceRecache))
                {
                    types[i] = type;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        static readonly Dictionary<string, Type> allType = new();
        static readonly Dictionary<string, Type> allComponentType = new();
        static readonly Dictionary<string, Type> allUnityObjectType = new();
        static bool CacheTypeInit = false;

        public static bool LogCacheWorning =

#if UNITY_EDITOR
        false;
#else
        true;
#endif

        /// <summary>
        /// 私有类可能导致名字冲突，一个名字仅能保存一个类型，优先Public
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="type"></param>
        /// <param name="logworning"></param>
        static void AddToDic(Dictionary<string, Type> dic, Type type, bool logworning = false)
        {
            //可能存在同名类型 internal,internal Component类型仍然可以挂在gameobject上，所以也要缓存。

            if (dic.ContainsKey(type.FullName))
            {
                var old = dic[type.FullName];
                if (old.IsPublic.CompareTo(type.IsPublic) <= 0)
                {
                    //Public 优先
                    //Unity比System优先，程序集字母顺序unity靠后，自动满足条件。
                    dic[type.FullName] = type;
                }

                if (LogCacheWorning || logworning)
                {
                    Debug.LogWarning($"Key already have  [{type.FullName}]" +
                    $"\n    {type.Assembly.FullName}" +
                    $"\n    {old.Assembly.FullName}");
                }
            }
            else
            {
                dic[type.FullName] = type;
            }
        }

        /// <summary>
        /// 防止多个线程同时缓存浪费性能。
        /// </summary>
        static readonly object cachelock = new();
        static HashSet<string> CachedAssemblyName = new();

        /// <summary>
        /// 第一次缓存类型特别耗时，考虑使用异步，或者使用后台线程预调用。<seealso cref="CacheAllTypesAsync(bool)"/>
        /// </summary>
        /// <param name="forceRecache">强制搜索所有程序集</param>
        public static void CacheAllTypes(bool forceRecache = false)
        {
            lock (cachelock)
            {
                if (forceRecache)
                {
                    CachedAssemblyName.Clear();
                }
                if (CacheTypeInit == false || forceRecache)
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName);
                    //var debugabs = assemblies.ToArray();
                    foreach (var assembly in assemblies)
                    {
                        if (CachedAssemblyName.Contains(assembly.FullName))
                        {
                            continue;
                        }

                        CacheAssembly(assembly);
                    }

                    CacheTypeInit = true;
                }
            }
        }

        /// <summary>
        /// 缓存一个程序集中的所有类型
        /// </summary>
        /// <param name="assembly"></param>
        public static void CacheAssembly(Assembly assembly)
        {
            CachedAssemblyName.Add(assembly.FullName);
            //var debug = assembly.GetTypes();
            foreach (var extype in assembly.GetTypes())
            {
                AddToDic(allType, extype);

#if UNITY_5_3_OR_NEWER

                if (typeof(UnityEngine.Object).IsAssignableFrom(extype))
                {
                    AddToDic(allUnityObjectType, extype);

                    if (typeof(UnityEngine.Component).IsAssignableFrom(extype))
                    {
                        AddToDic(allComponentType, extype);
                    }
                }

#endif

            }
        }

        /// <summary>
        /// 用于匹配非嵌套的特化泛型类型，或者嵌套特化泛型的最内层
        /// </summary>
        /// <remarks>
        /// 原理是泛型部分不能含有方括号，特化部分不能含有`，以此来匹配最内层泛型
        /// <para/>
        /// 思路：
        /// 用正则提取最内层特化泛型类型，将内侧类型替换为hashcode，并生成类型缓存。
        /// 循环向外层测试，直到不能匹配
        /// </remarks>
        public static readonly Regex NonNestedSpecializedGenericTypeRegex
            = new(@"(?<generic>[^\[\]]*?`\d+)\[(?<specialized>[^`]*?\])\]");

        /// <summary>
        /// 用于匹配泛型类型全名和方括号内的内容
        /// </summary>
        public static readonly Regex GenericRegex = new(@"^(?<generic>.*?`\d+)\[(?<specialized>.*)\]$");

        /// <summary>
        /// 用于匹配方括号内的每个子串
        /// </summary>
        public static readonly Regex SpecializedRegex = new(@"(?<=\[)[^,\[]+(?=[,\]])");

        /// <summary>
        /// 用于匹配方括号内的每个子串
        /// </summary>
        public static readonly Regex InnerTypeRegex = new(@"\[(?<typeShortName>(?<=\[)[^,\[]+(?=[,\]]))[^\[\]]*?\]");

        /// <summary>
        /// 输入一个非嵌套的特化泛型类型全名，输出一个泛型类型全名和一个特化类型全名的列表
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="genericTypeName"></param>
        /// <param name="specializedTypeNames"></param>
        /// <returns></returns>
        public static bool TryGetNonNestedSpecializedGenericTypeName(string fullName,
                                                                     out string genericTypeName,
                                                                     out List<string> specializedTypeNames)
        {
            // 使用 GenericRegex 对象匹配输入字符串
            Match match = NonNestedSpecializedGenericTypeRegex.Match(fullName);

            // 如果匹配成功
            if (match.Success)
            {
                // 获取泛型类型全名，并赋值给输出参数 genericTypeName
                genericTypeName = match.Groups["generic"].Value;

                // 获取方括号内的内容，并赋值给输出参数 specializedString
                var specializedString = match.Groups["specialized"].Value;
                // 使用 SpecializedRegex 对象匹配 specializedString 中的每个子串，并将其添加到输出参数 specializedTypeNames 中
                var match2 = SpecializedRegex.Matches(specializedString);
                specializedTypeNames = new List<string>();
                foreach (Match item in match2)
                {
                    specializedTypeNames.Add(item.Value);
                }
                // 返回 true，表示成功获取了特化类型全名
                return specializedTypeNames.Count > 0;
            }

            // 如果匹配失败，将输出参数设为 null，并返回 false
            genericTypeName = null;
            specializedTypeNames = null;
            return false;
        }

        /// <summary>
        /// 输入一个非嵌套的特化泛型类型全名，输出一个泛型类型和一个特化类型数组
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="genericType"></param>
        /// <param name="specializedTypes"></param>
        /// <returns></returns>
        public static bool TryGetNonNestedSpecializedGenericType(string fullName,
                                                                 out Type genericType,
                                                                 out Type[] specializedTypes)
        {
            if (TryGetNonNestedSpecializedGenericTypeName(fullName, out var genericTypeName, out var specializedTypeNames))
            {
                if (TryGetType(genericTypeName, out genericType))
                {
                    if (genericType.IsGenericType)
                    {
                        if (TryGetType(specializedTypeNames, out specializedTypes))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        //为了防止错误检测一下是不是泛型
                        Debug.LogError($"TryGetGenericType Error. {{ {genericType.FullName} }} not IsGenericType.");
                    }
                }
            }

            genericType = null;
            specializedTypes = null;
            return false;
        }

        static readonly Unity.Profiling.ProfilerMarker tryMakeGenericType = new(nameof(TryMakeGenericType));

        /// <summary>
        /// 制作泛型类型，输入一个特化泛型类型全名，输出一个特化泛型类型
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool TryMakeGenericType(string fullName, out Type type)
        {
            using var profiler = tryMakeGenericType.Auto();

            //制作泛型类
            var inners = NonNestedSpecializedGenericTypeRegex.Matches(fullName);
            if (inners.Count == 1 && fullName.StartsWith(inners[0].Value))
            {
                //只有一个匹配，并以匹配结果开始，认为是非嵌套泛型
                //这里没用相等比较而用了StartsWith，因为可能带有AssemblyQualifiedName时，末尾会带有程序集名。

                Match match = inners[0];

                // 获取泛型类型全名，并赋值给输出参数 genericTypeName
                var genericTypeName = match.Groups["generic"].Value;

                // 获取方括号内的内容，并赋值给输出参数 specializedString
                var specializedString = match.Groups["specialized"].Value;
                // 使用 SpecializedRegex 对象匹配 specializedString 中的每个子串，并将其添加到输出参数 specializedTypeNames 中
                var match2 = SpecializedRegex.Matches(specializedString);
                var specializedTypeNames = new List<string>();
                foreach (Match item in match2)
                {
                    specializedTypeNames.Add(item.Value);
                }

                if (specializedTypeNames.Count == 0)
                {
                    //没有找到特化类型名
                    type = null;
                    return false;
                }

                if (TryGetType(genericTypeName, out var genericType) && genericType.IsGenericType)
                {
                    if (TryGetType(specializedTypeNames, out var specializedTypes))
                    {
                        try
                        {
                            var temp = genericType.MakeGenericType(specializedTypes);
                            if (temp != null)
                            {
                                type = temp;

                                //在递归时fullName内部可能已经被替换为hashcode。
                                string realFullName = type.FullName;
                                //只添加到hotType即可，添加到allType没有意义。而且allType元素数量太多，添加操作更开销更大
                                //allType[realFullName] = type;
                                hotType[realFullName] = type;

                                if (realFullName != fullName)
                                {
                                    //将替hashcode换后的临时名字也缓存，下一次遇到时不用在正则解析了。
                                    //allType[fullName] = type;
                                    hotType[fullName] = type;
                                }

                                return true;
                            }
                        }
                        catch (Exception)
                        {
                            //Debug.LogError($"TryMakeGenericType Error. {fullName}");
                        }
                    }
                }
                else
                {
                    //为了防止错误检测一下是不是泛型
                    Debug.LogError($"TryGetGenericType Error. {{ {genericTypeName} }} not IsGenericType.");
                }
            }
            else if (inners.Count >= 1)
            {
                foreach (Match item in inners)
                {
                    //内层特化泛型类型名字
                    var innerSpecializedGenericTypeName = item.Value;
                    if (TryGetType(innerSpecializedGenericTypeName, out var innerType))
                    {
                        //hashcode 以数字或者符号开头，肯定不会和已有类型名冲突，是安全的。
                        var hashcode = innerSpecializedGenericTypeName.GetHashCode().ToString();
                        //allType[hashcode] = innerType;
                        hotType[hashcode] = innerType;
                        fullName = fullName.Replace(innerSpecializedGenericTypeName, hashcode);
                    }
                    else
                    {
                        Debug.LogError($"InnerSpecializedGenericType Error: {innerSpecializedGenericTypeName}");
                        type = null;
                        return false;
                    }
                }

                return TryGetType(fullName, out type);
            }
            else
            {
                //非泛型字符串
            }

            type = null;
            return false;
        }

        public static void Test()
        {
            List<int> a = new();
            TestMake(a);

            Dictionary<int, float> b = new();
            TestMake(b);

            Dictionary<List<int>, float> c = new();
            TestMake(c);

            Dictionary<List<int>, List<string>> d = new();
            TestMake(d);

            Dictionary<string, Dictionary<int, string>> e = new();
            TestMake(e);

            Dictionary<Dictionary<int, string>, float> f = new();
            TestMake(f);

            Dictionary<Dictionary<int, string>, List<float>> g = new();
            TestMake(g);

            Dictionary<List<double>, List<Dictionary<List<byte>, List<bool>>>> fuckType = new();
            TestMake(fuckType);
        }

        static void TestMake<T>(T obj = default)
        {
            var type = typeof(T);
            var fullName = type.FullName;
            if (TryMakeGenericType(fullName, out var makeType) && type == makeType)
            {
                Debug.Log($"测试通过  {fullName}");
            }
            else
            {
                Debug.LogError($"无法解析  {fullName}");
            }
        }

        /// <summary>
        /// TODO,编辑模式初始化时加个进度条
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        public static Task CacheAllTypesAsync(bool force = false)
        {
            return Task.Run(() => { CacheAllTypes(force); });
        }

        public static Task CacheAssemblyAsync(Assembly assembly)
        {
            return Task.Run(() => { CacheAssembly(assembly); });
        }

        /// <summary>
        /// 预热一个类型，可以避免在全类型中查找。
        /// 其他模块可以将常用类型使用static代码，在调用TypeCache前预热。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void HotType<T>()
        {
            var type = typeof(T);
            hotType[type.FullName] = type;
        }
    }
}
