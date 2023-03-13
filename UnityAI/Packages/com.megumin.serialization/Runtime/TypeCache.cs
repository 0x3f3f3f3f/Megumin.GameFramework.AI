using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.Serialization
{
    public static class TypeCache
    {

#if UNITY_5_3_OR_NEWER

        static readonly Dictionary<string, Type> hotComponentType = new Dictionary<string, Type>();
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

        static readonly Dictionary<string, Type> hotUnityObjectType = new Dictionary<string, Type>();
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

        static readonly Dictionary<string, Type> hotType = new Dictionary<string, Type>();
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
                    if (TryGetGenericAndSpecializedType(typeFullName, out var genericTypeName, out var specializedTypeNames))
                    {
                        if (TryGetType(genericTypeName, out var gType)
                            && gType.IsGenericType
                            && TryGetType(specializedTypeNames, out var specializedTypes))
                        {
                            try
                            {
                                var temp = gType.MakeGenericType(specializedTypes);
                                if (temp != null)
                                {
                                    type = temp;
                                    allType[typeFullName] = temp;
                                    hotType[typeFullName] = temp;
                                    return true;
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }

                    return false;
                }
            }
        }

        public static bool TryGetType(List<string> typeFullName, out Type[] types)
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

        static readonly Dictionary<string, Type> allType = new Dictionary<string, Type>();
        static readonly Dictionary<string, Type> allComponentType = new Dictionary<string, Type>();
        static readonly Dictionary<string, Type> allUnityObjectType = new Dictionary<string, Type>();
        static bool CacheTypeInit = false;

        public static bool LogCacheWorning =

#if UNITY_EDITOR
        false;
#else
        true;
#endif

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
        static readonly object cachelock = new object();

        /// <summary>
        /// 第一次缓存类型特别耗时，考虑使用异步，或者使用后台线程预调用。
        /// </summary>
        /// <param name="forceRecache">强制搜索所有程序集</param>
        public static void CacheAllTypes(bool forceRecache = false)
        {
            lock (cachelock)
            {
                if (CacheTypeInit == false || forceRecache)
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName);
                    //var debugabs = assemblies.ToArray();
                    foreach (var assembly in assemblies)
                    {
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

                    CacheTypeInit = true;
                }
            }
        }

        // 定义一个静态的正则表达式对象，用于匹配泛型类型全名和方括号内的内容
        public static readonly Regex GenericRegex = new(@"^(?<generic>.*`\d+)\[(?<specialized>.*)\]$");
        // 定义一个静态的正则表达式对象，用于匹配方括号内的每个子串
        public static readonly Regex SpecializedRegex = new(@"(?<=\[)[^,\[]+(?=[,\]])");

        /// <summary>
        /// 输入一个泛型类型全名，输出一个泛型类型全名和一个特化类型全名的列表
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="genericTypeName"></param>
        /// <param name="specializedTypeNames"></param>
        /// <returns></returns>
        public static bool TryGetGenericAndSpecializedType(string fullName, out string genericTypeName, out List<string> specializedTypeNames)
        {
            // 使用 GenericRegex 对象匹配输入字符串
            Match match = GenericRegex.Match(fullName);

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
        /// TODO,编辑模式初始化时加个进度条
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        public static Task CacheAllTypesAsync(bool force = false)
        {
            return Task.Run(() => { CacheAllTypes(force); });
        }
    }
}
