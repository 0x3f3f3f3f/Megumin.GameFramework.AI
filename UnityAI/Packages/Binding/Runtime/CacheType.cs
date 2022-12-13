using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.Binding
{
    public static class CacheType
    {
        static readonly Dictionary<string, Type> hotComponentType = new Dictionary<string, Type>();
        public static Type FindComponentType(string typeFullName)
        {
            if (hotComponentType.TryGetValue(typeFullName, out var type))
            {
                return type;
            }
            else
            {
                CacheAllTypes();
                if (allComponentType.TryGetValue(typeFullName, out type))
                {
                    hotComponentType[typeFullName] = type;
                    return type;
                }
                else
                {
                    return null;
                }
            }
        }

        static readonly Dictionary<string, Type> hotType = new Dictionary<string, Type>();
        public static Type FindType(string typeFullName)
        {
            if (hotType.TryGetValue(typeFullName, out var type))
            {
                return type;
            }
            else
            {
                CacheAllTypes();
                if (allType.TryGetValue(typeFullName, out type))
                {
                    hotType[typeFullName] = type;
                    return type;
                }
                else
                {
                    return null;
                }
            }
        }

        static readonly Dictionary<string, Type> allType = new Dictionary<string, Type>();
        static readonly Dictionary<string, Type> allComponentType = new Dictionary<string, Type>();
        static readonly Dictionary<string, Type> allUnityObjectType = new Dictionary<string, Type>();
        static bool CacheTypeInit = false;
        public static bool LogCacheWorning = true;

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
        /// <param name="force"></param>
        public static void CacheAllTypes(bool force = false)
        {
            lock (cachelock)
            {
                if (CacheTypeInit == false || force)
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName);
                    var debugabs = assemblies.ToArray();
                    foreach (var assembly in assemblies)
                    {
                        var debug = assembly.GetTypes();
                        foreach (var extype in assembly.GetTypes())
                        {
                            AddToDic(allType, extype);

                            if (typeof(UnityEngine.Object).IsAssignableFrom(extype))
                            {
                                AddToDic(allUnityObjectType, extype);

                                if (typeof(UnityEngine.Component).IsAssignableFrom(extype))
                                {
                                    AddToDic(allComponentType, extype);
                                }
                            }
                        }
                    }

                    CacheTypeInit = true;
                }
            }
        }

        public static Task CacheAllTypesAsync(bool force = false)
        {
            return Task.Run(() => { CacheAllTypes(force); });
        }
    }
}
