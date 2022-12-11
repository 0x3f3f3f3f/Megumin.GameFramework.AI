using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

                //处理中间层级
                //var (nextI, nextT) = GetInstanceAndType(path[1], instance, bindType);
                return CreateDelegate<T>(path[1], instance, bindType);
            }

            return (ParseResult, Getter, Setter);
        }

        private static (BindResult ParseResult, Func<T> Getter, Action<T> Setter)
            CreateDelegate<T>(string memberName, object instance, Type bindType)
        {
            BindResult ParseResult = BindResult.None;
            Func<T> Getter = null;
            Action<T> Setter = null;

            try
            {
                //尝试绑定propertyInfo
                {
                    var propertyInfo = bindType.GetProperty(memberName);
                    if (propertyInfo != null)
                    {
                        if (propertyInfo.CanRead)
                        {
                            var getMethod = propertyInfo.GetGetMethod();

                            if (getMethod.IsStatic)
                            {
                                Getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), null, getMethod);
                                ParseResult |= BindResult.Get;
                            }
                            else
                            {
                                if (instance == null)
                                {
                                    Debug.LogError("instance is null");
                                }
                                else
                                {
                                    Getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), instance, getMethod);
                                    ParseResult |= BindResult.Get;
                                }
                            }
                        }

                        if (propertyInfo.CanWrite)
                        {
                            var setMethod = propertyInfo.GetSetMethod();
                            if (setMethod.IsStatic)
                            {
                                Setter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), null, setMethod);
                                ParseResult |= BindResult.Set;
                            }
                            else
                            {
                                if (instance == null)
                                {
                                    Debug.LogError("instance is null");
                                }
                                else
                                {
                                    Setter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), instance, setMethod);
                                    ParseResult |= BindResult.Set;
                                }
                            }
                        }

                        return (ParseResult, Getter, Setter);
                    }

                }

                //尝试绑定field
                {
                    var fieldInfo = bindType.GetField(memberName);
                    if (fieldInfo != null)
                    {
                        {
                            if (fieldInfo.IsStatic)
                            {
                                Getter = () =>
                                {
                                    return (T)fieldInfo.GetValue(null);
                                };
                                ParseResult |= BindResult.Get;
                            }
                            else
                            {
                                if (instance == null)
                                {
                                    Debug.LogError("instance is null");
                                }
                                else
                                {
                                    Getter = () =>
                                    {
                                        return (T)fieldInfo.GetValue(instance);
                                    };
                                    ParseResult |= BindResult.Get;
                                }
                            }
                        }

                        if (!fieldInfo.IsInitOnly)
                        {
                            if (fieldInfo.IsStatic)
                            {
                                Setter = (value) =>
                                {
                                    fieldInfo.SetValue(null, value);
                                };
                                ParseResult |= BindResult.Set;
                            }
                            else
                            {
                                if (instance == null)
                                {
                                    Debug.LogError("instance is null");
                                }
                                else
                                {
                                    Setter = (value) =>
                                    {
                                        fieldInfo.SetValue(instance, value);
                                    };
                                    ParseResult |= BindResult.Set;
                                }
                            }
                        }
                    }
                }

                // 尝试绑定method
                {
                    var methodName = memberName;
                    if (memberName.EndsWith("()"))
                    {
                        methodName = memberName.Replace("()", "");
                        //TODO 泛型函数
                    }

                    var methodInfo = bindType.GetMethod(methodName);
                    if (methodInfo != null && typeof(T).IsAssignableFrom(methodInfo.ReturnType))
                    {
                        var paras = methodInfo.GetParameters();
                        if (paras.Length == 0)
                        {
                            if (methodInfo.IsStatic)
                            {
                                Getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), null, methodInfo);
                                ParseResult |= BindResult.Get;
                            }
                            else
                            {
                                Getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), instance, methodInfo);
                                ParseResult |= BindResult.Get;
                            }
                        }
                        else
                        {
                            //Todo
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"BindParse {e}");
            }

            return (ParseResult, Getter, Setter);
        }

        private static (object nextIntance, Type memberType) GetInstanceAndType(string memberName, object instance, Type instanceType)
        {
            //必须传instance 和Type,可能是静态类型。
            var nextmember = instanceType.GetProperty(memberName);
            var nextIntance = nextmember.GetValue(instance, null);
            Type memberType = nextmember.PropertyType;
            return (nextIntance, memberType);
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
