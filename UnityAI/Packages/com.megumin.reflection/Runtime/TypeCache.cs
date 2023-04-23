using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.Reflection
{
    /// <summary>
    /// 类型缓存。
    /// 通过缓存所有已加载程序集，实现通过类型全名获取类型。
    /// 第一次调用会导致卡顿，在调用前考虑使用多线程初始化，防止阻塞主线程。
    /// </summary>
    /// <remarks>
    /// 为了防止第一次卡顿，可以考虑<see cref="CacheAssembly(Assembly, bool)"/>手动缓存马上就会使用到的类型。
    /// 再使用<see cref="CacheAllTypesAsync(bool, Func{Assembly, bool})"/>异步缓存所有类型。
    /// <para/> 因为特化的泛型在程序集中不存在，
    /// 所以特化泛型肯定会触发<see cref="CacheAllTypes(bool, Func{Assembly, bool})"/>
    /// <para/> 不要使用 RegexOptions.Compiled，可能造成严重的初始化卡顿。https://stackoverflow.com/a/7707369
    /// </remarks>
    public static partial class TypeCache
    {

#if UNITY_5_3_OR_NEWER

        static readonly ConcurrentDictionary<string, Type> hotComponentType = new();
        public static Type GetComponentType(string typeFullName)
        {
            TryGetComponentType(typeFullName, out var type);
            return type;
        }

        public static bool TryGetComponentType(string typeFullName, out Type type)
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
                if (TryGetTypeDerivedFrom<Component>(typeFullName, out type))
                {
                    hotComponentType[typeFullName] = type;
                    return true;
                }

                return false;
            }
        }

        static readonly ConcurrentDictionary<string, Type> hotUnityObjectType = new();
        public static Type GetUnityObjectType(string typeFullName)
        {
            TryGetUnityObjectType(typeFullName, out var type);
            return type;
        }

        public static bool TryGetUnityObjectType(string typeFullName, out Type type)
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
                if (TryGetTypeDerivedFrom<UnityEngine.Object>(typeFullName, out type))
                {
                    hotComponentType[typeFullName] = type;
                    return true;
                }

                return false;
            }
        }

#endif

        /// <summary>
        /// 使用ConcurrentDictionary 解决多线程访问问题
        /// </summary>
        static readonly ConcurrentDictionary<string, Type> hotType = new();
        public static Type GetType(string typeFullName)
        {
            TryGetType(typeFullName, out var type);
            return type;
        }

        static readonly Unity.Profiling.ProfilerMarker tryGetTypeMarker = new(nameof(TryGetType));
        public static bool TryGetType(string typeFullName, out Type type)
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
                //Q：触发CacheAllTypes 防在allType.TryGetValue前还是allType.TryGetValue失败后？
                //A：放在前面，优点是第一次调用时能直接触发，不用double check
                //A：放在后面，以后每次获取类型时，不用去检查是否已经初始化缓存，性能可以稍稍高一点。
                //   但是由于有hotType机制，只有第一次获取这个类型时才有性能提升。
                //   缺点是 allType.TryGetValue需要写2次。
                //结论：放在后面，allType.TryGetValue找不到类型只有第一次和错误类型名，是极小概率事件

                //这里加入CacheTypeInit判断能一定程度减少多线程冲突
                if (CacheTypeInit && allType.TryGetValue(typeFullName, out type))
                {
                    hotType[typeFullName] = type;
                    return true;
                }
                else
                {
                    //没有找到类型时，看看是不是还没有初始化
                    CacheAllTypes();

                    //总是 double check 防止多线程错误。
                    //不使用CacheAllTypes返回值决定是否 double check。
                    //CacheAllTypes返回值在多线程下并不是完全准确。

                    //真实环境遇到的问题是，两个线程同时获取一个类型，一个线程开始执行缓存，
                    //另一个线程因为锁而阻塞，最终结果是被阻塞的线程CacheAllTypes返回值fasle。
                    if (allType.TryGetValue(typeFullName, out type))
                    {
                        hotType[typeFullName] = type;
                        return true;
                    }

                    //现有类型不存在，尝试制作泛型类型
                    if (TryMakeGenericType(typeFullName, out type))
                    {
                        return true;
                    }

                    if (TryMakeArrayType(typeFullName, out type))
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        public static Type GetTypeDerivedFrom<T>(string typeFullName)
        {
            TryGetTypeDerivedFrom<T>(typeFullName, out var type);
            return type;
        }

        public static bool TryGetTypeDerivedFrom<T>(string typeFullName, out Type type)
        {
            if (TryGetType(typeFullName, out var result)
                && typeof(T).IsAssignableFrom(result)
                /*&& result.IsSubclassOf(typeof(T))*/)
            {
                type = result;
                return true;
            }

            type = null;
            return false;
        }

        public static List<Type> GetTypesDerivedFrom<T>()
        {
            List<Type> result = new List<Type>();
            TryGetTypesDerivedFrom<T>(result);
            return result;
        }

        public static bool TryGetTypesDerivedFrom<T>(List<Type> types)
        {
            bool has = false;
            if (types != null)
            {
                CacheAllTypes();
                foreach (var item in allType)
                {
                    if (typeof(T).IsAssignableFrom(item.Value))
                    {
                        has = true;
                        types.Add(item.Value);
                    }
                }
            }
            return has;
        }

        public static bool TryGetType(List<string> typeFullName,
                                      out Type[] types)
        {
            types = new Type[typeFullName.Count];
            for (int i = 0; i < typeFullName.Count; i++)
            {
                if (TryGetType(typeFullName[i], out var type))
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
        //static readonly Dictionary<string, Type> allComponentType = new();
        //static readonly Dictionary<string, Type> allUnityObjectType = new();
        static bool CacheTypeInit = false;

        public static bool LogCacheWorning =

#if UNITY_EDITOR
        false;
#else
        true;
#endif

        /// <summary>
        /// 私有类可能导致名字冲突，一个名字仅能保存一个类型。
        /// 优先Public。后添加的替换先添加的。
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="type"></param>
        /// <param name="logworning"></param>
        static void AddToDic(Dictionary<string, Type> dic, Type type, bool logworning = false)
        {
            //可能存在同名类型 internal,internal Component类型仍然可以挂在gameobject上，所以也要缓存。
            if (dic.TryGetValue(type.FullName, out var old))
            {
                if (old != type && old.IsPublic.CompareTo(type.IsPublic) <= 0)
                {
                    //Public 优先 。
                    //后添加的替换先添加的。
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
        /// 第一次缓存类型特别耗时，考虑使用异步，或者使用后台线程预调用。
        /// <seealso cref="CacheAllTypesAsync(bool, Func{Assembly, bool})"/>
        /// </summary>
        /// <param name="forceRecache">强制搜索所有程序集</param>
        /// <param name="assemblyFilter">过滤掉一些不常用程序集，返回true时程序集不会被缓存</param>
        /// <returns>
        /// <see langword="true"/>：缓存发生变化
        /// <see langword="false"/>：缓存没有发生变化
        /// </returns>
        public static bool CacheAllTypes(bool forceRecache = false, Func<Assembly, bool> assemblyFilter = null)
        {
            bool hasChange = false;
            if (CacheTypeInit == false || forceRecache)
            {
                hasChange |= true;
            }

            lock (cachelock)
            {
                //using ProgressBarScope sope = new("CacheAllTypes");

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
                        if (assemblyFilter?.Invoke(assembly) ?? false)
                        {
                            continue;
                        }

                        hasChange |= CacheAssembly(assembly, forceRecache);
                    }

                    CacheTypeInit = true;
                }

                return hasChange;
            }
        }

        /// <summary>
        /// 缓存一个程序集中的所有类型。
        /// </summary>
        /// <param name="assembly"></param>
        /// <remarks>
        /// 建议游戏开始时手动缓存 System(先) 和 unity(后) 程序集。足以应付大多数常见类型。之后有更多时间缓存全部类型。
        /// </remarks>
        public static bool CacheAssembly(Assembly assembly, bool forceRecache = false)
        {
            if (forceRecache)
            {
                CachedAssemblyName.Add(assembly.FullName);
            }
            else
            {
                if (CachedAssemblyName.Add(assembly.FullName) == false)
                {
                    return false;
                }
            }

            var assemblyAllType = assembly.GetTypes();
            foreach (var extype in assemblyAllType)
            {
                AddToDic(allType, extype);

                //因为有hot机制，额外缓存unity相关类型只有第一次查找每个类型时有收益。
                //缓存时不在额外缓存unity相关类型，获取时额外从总类型查找。提高缓存时的速度。减少内存占用。
                //#if UNITY_5_3_OR_NEWER

                //                if (typeof(UnityEngine.Object).IsAssignableFrom(extype))
                //                {
                //                    AddToDic(allUnityObjectType, extype);

                //                    if (typeof(UnityEngine.Component).IsAssignableFrom(extype))
                //                    {
                //                        AddToDic(allComponentType, extype);
                //                    }
                //                }

                //#endif

            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static bool AssemblyFilter(Assembly assembly)
        {
            //过滤掉一些，不然肯能太卡
            var assName = assembly.FullName;
            if (assName.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //可以通过这个宏来强行搜索unity中的类型
            if (assName.StartsWith("Unity", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 用于匹配非嵌套的特化泛型类型，或者嵌套特化泛型的最内层
        /// <para/>末尾的<![CDATA[(?!\[,*\])]]>是一个 零宽负向先行断言，排除泛型数组，不能以数组[]或者[,]结尾。
        /// https://www.runoob.com/w3cnote/reg-lookahead-lookbehind.html
        /// </summary>
        /// <remarks>
        /// 原理是泛型部分不能含有方括号，特化部分不能含有`，以此来匹配最内层泛型
        /// <para/>
        /// 思路：
        /// 用正则提取最内层特化泛型类型，将内侧类型替换为hashcode，并生成类型缓存。
        /// 循环向外层测试，直到不能匹配
        /// </remarks>
        public static readonly Regex NonNestedSpecializedGenericTypeRegex
            = new(@"(?<generic>[^\[\]]+?`\d+)\[(?<specialized>[^`]{2,}?\])\](?!\[,*\])");

        /// <summary>
        /// 用于匹配泛型类型全名和方括号内的内容
        /// </summary>
        public static readonly Regex GenericRegex = new(@"^(?<generic>.*?`\d+)\[(?<specialized>.*)\]$");

        /// <summary>
        /// 用于匹配方括号内的每个子串
        /// <para/>前面是[，
        /// <para/>中间是特化类型名：不为,[]的名字 和 0个或多个[,]可能是数组的方括号串 构成的特化类型名
        /// <para/>后面是,或]
        /// </summary>
        /// <remarks>
        /// 这里可以匹配交错数组名，但是后续制作泛型是，交错数组仍是无法解析类型的。
        /// </remarks>
        public static readonly Regex SpecializedRegex = new(@"(?<=\[)[^,\[\]]+(?:\[,*\])*(?=[,\]])");

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
            System.Diagnostics.Stopwatch stopwatch = new();
            stopwatch.Start();

            //因为有泛型，无论如何都会触发CacheAllType.
            CacheAssembly(typeof(int).Assembly);
            CacheAssembly(typeof(GameObject).Assembly);

            List<int> a = new();
            TestParse(a);

            Dictionary<int, float> b = new();
            TestParse(b);

            Dictionary<List<int>, float> c = new();
            TestParse(c);

            Dictionary<List<int>, List<string>> d = new();
            TestParse(d);

            Dictionary<string, Dictionary<int, string>> e = new();
            TestParse(e);

            Dictionary<Dictionary<int, string>, float> f = new();
            TestParse(f);

            Dictionary<Dictionary<int, string>, List<float>> g = new();
            TestParse(g);

            Dictionary<List<double>, List<Dictionary<List<byte>, List<bool>>>> fuckType = new();
            TestParse(fuckType);

            //数组，多维数组
            int rank = 0;
            int length = 0;
            int[] h = new int[3];
            rank = h.Rank;
            length = h.Length;
            TestParse(h);

            GameObject[] i = new GameObject[3];
            rank = i.Rank;
            length = i.Length;
            TestParse(i);

            List<int>[] j = new List<int>[3];
            rank = j.Rank;
            length = j.Length;
            TestParse(j);

            int[,] k = new int[4, 2];
            rank = k.Rank;
            length = k.Length;
            TestParse(k);

            int[,,,] l = new int[4, 2, 3, 5];
            rank = l.Rank;
            length = l.Length;
            TestParse(l);


            int[][] m = new int[2][];
            rank = m.Rank;
            length = m.Length;
            TestParse(m);

            int[][,] jaggedArray4 = new int[3][,]
            {
                new int[,] { {1,3}, {5,7} },
                new int[,] { {0,2}, {4,6}, {8,10} },
                new int[,] { {11,22}, {99,88}, {0,9} }
            };
            rank = jaggedArray4.Rank;
            length = jaggedArray4.Length;
            TestParse(jaggedArray4);

            //泛型和数组互相组合
            List<int[]> genericAndArray1 = new();
            TestParse(genericAndArray1);

            Dictionary<List<int>, string[]> genericAndArray2 = new();
            TestParse(genericAndArray2);

            List<int[,,]> genericAndArray3 = new();
            TestParse(genericAndArray3);

            Dictionary<int[,,], Dictionary<int[], int[,,,,]>> genericAndArray4 = new();
            TestParse(genericAndArray4);

            stopwatch.Stop();
            Debug.Log($"ElapsedTime: {stopwatch.ElapsedMilliseconds}");
        }

        static void TestParse<T>(T obj = default)
        {
            var type = typeof(T);
            var fullName = type.FullName;
            if (TryGetType(fullName, out var makeType) && type == makeType)
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
        /// <param name="forceRecache"></param>
        /// <param name="assemblyFilter">过滤掉一些不常用程序集，返回true时程序集不会被缓存</param>
        /// <returns></returns>
        public static Task CacheAllTypesAsync(bool forceRecache = false, Func<Assembly, bool> assemblyFilter = null)
        {
            return Task.Run(() => { CacheAllTypes(forceRecache, assemblyFilter); });
        }

        public static Task CacheAssemblyAsync(Assembly assembly, bool forceRecache = false)
        {
            return Task.Run(() => { CacheAssembly(assembly, forceRecache); });
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

        /// <summary>
        /// <inheritdoc cref="HotType(string, Type)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aliasName"></param>
        public static void HotType<T>(string aliasName)
        {
            HotType(aliasName, typeof(T));
        }

        /// <summary>
        /// 在热点类型中增加别名映射。
        /// <para/> 用于因为修改类型名，已有的序列化文件无法打开的情况。
        /// <para/> 可以在反序列化之前，将旧的类名手动映射到新的类型。从而不用修改反序列化代码。
        /// </summary>
        /// <param name="aliasName"></param>
        /// <param name="type"></param>
        public static void HotType(string aliasName, Type type)
        {
            if (string.IsNullOrEmpty(aliasName))
            {
                return;
            }

            hotType[aliasName] = type;
        }

        public static void HotAllTypeAlias()
        {
            foreach (var item in allType)
            {
                var type = item.Value;
                HotTypeAlias(type);
            }
        }

        public static void HotTypeAlias(Type type)
        {
            //类型继承的别名不考虑
            var attris = type.GetCustomAttributes<SerializationAliasAttribute>(false);
            if (attris != null)
            {
                foreach (var attri in attris)
                {
                    hotType[attri.Alias] = type;
                }
            }
        }

        /// <summary>
        /// 处理<see cref="SerializationAliasAttribute"/>特性。用于类名变更
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void HotTypeAliasDerivedFrom<T>()
        {
            foreach (var type in GetTypesDerivedFrom<T>())
            {
                HotTypeAlias(type);
            }
        }

        /// <summary>
        /// 用于序列化时缩短名字
        /// </summary>
        public static List<string> StripTypeNameList { get; } = new()
        {
            ", mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
            ", UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",
            ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null",
        };

        /// <summary>
        /// 缩短类型全名，不影响类型查找和反序列化
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        /// <remarks>
        /// 不确定会不会产生类型冲突，谨慎使用
        /// </remarks>
        public static string StripTypeName(this string typeFullName)
        {
            foreach (var item in StripTypeNameList)
            {
                typeFullName = typeFullName.Replace(item, "");
            }
            return typeFullName;
        }
    }

    public static partial class TypeCache
    {
        /// <summary>
        /// 用于匹配数组类型。
        /// 目前故意不支持交错数组。
        /// </summary>
        public static readonly Regex ArrayRegex = new(@"^(?<element>.+?)\[(?<rank>,*)\]");

        static readonly Unity.Profiling.ProfilerMarker tryMakeArrayType = new(nameof(TryMakeArrayType));

        /// <summary>
        /// 制作数组类型，输入一个数组类型全名，输出一个数组类型类型
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <remarks>
        /// 无法创建交错数组，没找到对应API，所以正则故意没有匹配交错数组。
        /// </remarks>
        public static bool TryMakeArrayType(string fullName, out Type type)
        {
            using var profiler = tryMakeArrayType.Auto();

            // 使用 ArrayRegex 对象匹配输入字符串
            Match match = ArrayRegex.Match(fullName);
            if (match.Success)
            {
                var elementTypeName = match.Groups["element"].Value;
                var rank = match.Groups["rank"].Value.Length + 1;

                if (TryGetType(elementTypeName, out var elementType))
                {
                    if (rank > 1)
                    {
                        type = elementType.MakeArrayType(rank);
                        hotType[fullName] = type;
                        return true;
                    }
                    else if (rank >= 0)
                    {
                        //elementType.MakeArrayType(1); 的结果与预期不同 返回int[*],不知道为什么多个*号
                        type = elementType.MakeArrayType();
                        hotType[fullName] = type;
                        return true;
                    }
                }
            }

            type = null;
            return false;
        }
    }

    /// <summary>
    /// 别名。如果标记在类型上，要使用类型全名，带上命名空间。
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class SerializationAliasAttribute : Attribute
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        public SerializationAliasAttribute(string alias)
        {
            Alias = alias;
        }
    }
}

#if UNITY_EDITOR

namespace Megumin
{
    using System.Diagnostics;
    using UnityEditor;

    //[Conditional("UnityEditor")]
    public struct ProgressBarScope : IDisposable
    {
        public ProgressBarScope(string title, string info = "", float progress = 0f)
        {
            EditorUtility.DisplayProgressBar(title, info, progress);
        }

        public void Dispose()
        {
            EditorUtility.ClearProgressBar();
        }
    }

}

#endif

