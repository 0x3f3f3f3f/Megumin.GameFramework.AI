using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using static PlasticGui.PlasticTableCell;

namespace Megumin.Binding
{
    /// <summary>
    /// BindingString格式:  [组件类|静态类|接口]/成员/....../成员/成员。
    /// 最后一个成员的类型需要满足绑定类型，或者可以通过类型适配器转换成绑定类型。
    /// </summary>
    public class UnityBindingParser : BindingParser
    {
        /// <summary>
        /// 这里自动初始化，如果导致项目启动过慢，请修改此处，并手动在适当位置初始化。
        /// </summary>
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            Instance = new UnityBindingParser();

            ///预调用
            CacheAllTypesAsync();
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

                var (instance, instanceType) = GetBindInstanceAndType(path[0], gameObject);

                if (instanceType != null)
                {
                    //处理中间层级
                    //var (nextI, nextT) = GetInstanceAndType(path[1], instance, instanceType);
                    return CreateDelegate<T>(instanceType, instance, path[1]);
                }
                else
                {
                    Debug.LogWarning($"没有找到指定类型 {path[0]}");
                }
            }

            return (ParseResult, Getter, Setter);
        }

        /// <summary>
        /// 尝试绑定propertyInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <param name="ParseResult"></param>
        /// <param name="Getter"></param>
        /// <param name="Setter"></param>
        /// <returns>是否含有成员</returns>
        public static bool TryCreatePropertyDelegate<T>(Type instanceType,
                                                     object instance,
                                                     string memberName,
                                                     out BindResult ParseResult,
                                                     out Func<T> Getter,
                                                     out Action<T> Setter)
        {
            ParseResult = BindResult.None;
            Getter = null;
            Setter = null;

            var propertyInfo = instanceType.GetProperty(memberName);
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

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试绑定field
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <param name="ParseResult"></param>
        /// <param name="Getter"></param>
        /// <param name="Setter"></param>
        /// <returns>是否含有成员</returns>
        public static bool TryCreateFieldDelegate<T>(Type instanceType,
                                                     object instance,
                                                     string memberName,
                                                     out BindResult ParseResult,
                                                     out Func<T> Getter,
                                                     out Action<T> Setter)
        {
            ParseResult = BindResult.None;
            Getter = null;
            Setter = null;

            var fieldInfo = instanceType.GetField(memberName);
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

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试绑定method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <param name="ParseResult"></param>
        /// <param name="Getter"></param>
        /// <param name="Setter"></param>
        /// <returns>是否含有成员</returns>
        public static bool TryCreateMethodDelegate<T>(Type instanceType,
                                                     object instance,
                                                     string memberName,
                                                     out BindResult ParseResult,
                                                     out Func<T> Getter,
                                                     out Action<T> Setter)
        {
            ParseResult = BindResult.None;
            Getter = null;
            Setter = null;

            var methodName = memberName;
            if (memberName.EndsWith("()"))
            {
                methodName = memberName.Replace("()", "");
                //TODO 泛型函数
            }

            var methodInfo = instanceType.GetMethod(methodName);
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

                return true;
            }
            else
            {
                return false;
            }
        }

        public static (BindResult ParseResult, Func<T> Getter, Action<T> Setter)
            CreateDelegate<T>(Type instanceType, object instance, string memberName)
        {
            BindResult ParseResult = BindResult.None;
            Func<T> Getter = null;
            Action<T> Setter = null;

            try
            {
                //属性 字段 方法 逐一尝试绑定。

                if (TryCreatePropertyDelegate(instanceType, instance, memberName, out ParseResult, out Getter, out Setter))
                {
                    return (ParseResult, Getter, Setter);
                }

                if (TryCreateFieldDelegate(instanceType, instance, memberName, out ParseResult, out Getter, out Setter))
                {
                    return (ParseResult, Getter, Setter);
                }

                if (TryCreateMethodDelegate(instanceType, instance, memberName, out ParseResult, out Getter, out Setter))
                {
                    return (ParseResult, Getter, Setter);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"BindParse {e}");
            }

            Debug.LogWarning($"{instanceType.FullName} 没有找到成员 {memberName}");
            return (ParseResult, Getter, Setter);
        }

        public static (object nextIntance, Type memberType)
            GetInstanceAndType(Type instanceType, object instance, string memberName)
        {
            //必须传instance 和Type,可能是静态类型。
            var nextmember = instanceType.GetProperty(memberName);
            var nextIntance = nextmember.GetValue(instance, null);
            Type memberType = nextmember.PropertyType;
            return (nextIntance, memberType);
        }

        /// <summary>
        /// Unity和纯C#运行时解析逻辑时不一样的，unity中第一个字符串表示组件，在纯C#运行时可能会忽略第一个字符串。
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static (object Instance, Type InstanceType)
            GetBindInstanceAndType(string typeFullName, GameObject gameObject)
        {
            if (typeFullName == "UnityEngine.GameObject")
            {
                return (gameObject, typeof(UnityEngine.GameObject));
            }

            //TODO，绑定接口，通过接口取得组件
            var type = GeComponentType(typeFullName);

            if (type == null)
            {
                type = GetCustomType(typeFullName);
                if (type != null && type.IsInterface)
                {
                    var comp = gameObject.GetComponentInChildren(type);
                    if (comp)
                    {
                        return (comp, comp.GetType());
                    }
                }

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
