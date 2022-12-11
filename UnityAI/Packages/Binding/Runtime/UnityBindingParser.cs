using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Megumin.Binding
{
    public class UnityBindingParser : BindingParser
    {

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            Instance = new UnityBindingParser();
        }

        public override (BindResult ParseResult, Func<T> Getter, Action<T> Setter)
            InitializeBinding<T>(string BindingString, object agent, object extnalObj)
        {
            BindResult ParseResult = BindResult.None;
            Func<T> Getter = null;
            Action<T> Setter = null;


            if (string.IsNullOrEmpty(BindingString))
            {

            }
            else
            {
                var path = BindingString.Split('/');
                GameObject gameObject = agent as GameObject;
                if (extnalObj is GameObject ex && ex)
                {
                    gameObject = ex;
                }

                var (instance, bindType) = GetBindInstanceAndType(path[0], gameObject);

                {
                    //尝试绑定propertyInfo
                    var propertyInfo = bindType.GetProperty(path[1]);
                    if (propertyInfo.CanRead)
                    {
                        var getMethod = propertyInfo.GetGetMethod();
                        var firstArgs = instance;
                        if (getMethod.IsStatic)
                        {
                            firstArgs = null;
                        }
                        Getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), firstArgs, getMethod);
                        ParseResult |= BindResult.Get;
                    }

                    if (propertyInfo.CanWrite)
                    {
                        var setMethod = propertyInfo.GetSetMethod();
                        var firstArgs = instance;
                        if (setMethod.IsStatic)
                        {
                            firstArgs = null;
                        }
                        Setter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), firstArgs, setMethod);
                        ParseResult |= BindResult.Set;
                    }
                }

                {
                    //尝试绑定field
                    var fieldInfo = instance.GetType().GetField(path[1]);

                }
            }

            return (ParseResult, Getter, Setter);
        }

        private static (object Object, Type Type) GetBindInstanceAndType(string typeFullName, GameObject gameObject)
        {
            if (typeFullName == "UnityEngine.GameObject")
            {
                return (gameObject, typeof(UnityEngine.GameObject));
            }

            var type = GeComponentType(typeFullName);


            if (type == null)
            {
                type = GetCustomType(typeFullName);
                return (gameObject, type);
            }
            else
            {
                var comp = gameObject.GetComponentInChildren(type);
                return (comp, type);
            }
        }

        static readonly Dictionary<string, Type> hotType = new Dictionary<string, Type>();
        private static Type GetCustomType(string typeFullName)
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

        static readonly Dictionary<string, Type> hotComponentType = new Dictionary<string, Type>();
        public static Type GeComponentType(string typeFullName)
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


        static readonly Dictionary<string, Type> allType = new Dictionary<string, Type>();
        static readonly Dictionary<string, Type> allComponentType = new Dictionary<string, Type>();
        static readonly Dictionary<string, Type> allUnityObjectType = new Dictionary<string, Type>();
        static bool CacheTypeInit = false;
        static bool LogCacheWorning = true;

        public static void AddToDic(Dictionary<string, Type> dic, Type type, bool logworning = false)
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

        public static void CacheAllTypes(bool force = false)
        {
            if (CacheTypeInit == false || force)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
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

        public static Task CacheAllTypesAsync(bool force = false)
        {
            return Task.Run(() => { CacheAllTypes(force); });
        }
    }
}
