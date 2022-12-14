using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using static Megumin.Binding.CacheType;

namespace Megumin.Binding
{
    /// <summary>
    /// BindingString格式:  (组件类|静态类|接口)/成员/....../成员/成员。  
    /// 最后一个成员的类型需要满足绑定类型，或者可以通过类型适配器转换成绑定类型。
    /// 
    /// https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
    /// https://www.cnblogs.com/xinaixia/p/5777886.html
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

            if (Application.isPlaying) ///编辑器模式不要预调研，频繁修改代码会很卡并且打印警告
            {
                ///预调用
                CacheAllTypesAsync();
            }
        }

        /// <summary>
        /// 委托链模式还是实例链模式
        /// </summary>
        public bool DeepParseMode = true;

        public override (ParseBindingResult ParseResult, Func<T> Getter, Action<T> Setter)
            InitializeBinding<T>(string bindingString, object bindingInstance)
        {
            ParseBindingResult ParseResult = ParseBindingResult.None;
            Func<T> Getter = null;
            Action<T> Setter = null;


            if (string.IsNullOrEmpty(bindingString))
            {

            }
            else
            {
                var path = bindingString.Split('/');
                GameObject rootInstance = bindingInstance as GameObject;

                if (!rootInstance)
                {
                    if (bindingInstance is Component component)
                    {
                        rootInstance = component.gameObject;
                    }
                }

                var (instance, instanceType) = GetBindInstanceAndType(path[0], rootInstance);

                if (instanceType != null)
                {
                    if (path.Length == 1)
                    {
                        //当path中只有类型时，处理有限的绑定。
                        if (typeof(T) == typeof(System.Type))
                        {
                            //处理Type绑定
                            if (instanceType is T resulttype)
                            {
                                Getter = () =>
                                {
                                    return resulttype;
                                };
                                ParseResult |= ParseBindingResult.Get;
                            }
                        }
                    }
                    else if (path.Length == 2)
                    {
                        return CreateDelegate<T>(instanceType, instance, path[1]);
                    }
                    else
                    {
                        if (DeepParseMode)
                        {
                            //使用委托链的方式处理多级层级绑定
                            //https://zhuanlan.zhihu.com/p/105292546

                            Delegate getInstaneceDelegate = null;
                            Type innerInStanceType = instanceType;
                            (getInstaneceDelegate, innerInStanceType)
                                = GetGetInstanceDelegateAndReturnType(innerInStanceType, instance, path[1]);

                            for (int i = 2; i < path.Length - 1; i++)
                            {
                                (getInstaneceDelegate, innerInStanceType) =
                                    GetGetInstanceDelegateAndReturnType(innerInStanceType, getInstaneceDelegate, path[i], true);
                            }

                            if (innerInStanceType != null)
                            {
                                return CreateDelegate<T>(innerInStanceType, getInstaneceDelegate, path[path.Length - 1], true);
                            }
                        }
                        else
                        {
                            //使用实例链的方式处理多级层级绑定
                            object innerIntance = instance;
                            Type innerInStanceType = instanceType;
                            for (int i = 1; i < path.Length - 1; i++)
                            {
                                var member = path[i];
                                //处理中间层级 每级都取得实例，优点是最终生成的委托性能较高。缺点是中间级别如果对象重新赋值，需要重新绑定。
                                (innerIntance, innerInStanceType) = GetInstanceAndType(innerInStanceType, innerIntance, member);
                            }

                            if (innerInStanceType != null)
                            {
                                return CreateDelegate<T>(innerInStanceType, innerIntance, path[path.Length - 1]);
                            }

                        }

                        Debug.LogWarning($"无法处理多层级绑定 {path[0]}");
                    }
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
        /// <param name="instanceIsGetDelegate">temp 是不是 delegate要明确指定，而不能用重载。否则遇到类型恰好是delegate是会出现冲突。
        /// </param>
        /// <returns>是否含有成员</returns>
        public static bool TryCreatePropertyDelegate<T>(Type instanceType,
                                                     object instance,
                                                     string memberName,
                                                     out ParseBindingResult ParseResult,
                                                     out Func<T> Getter,
                                                     out Action<T> Setter,
                                                     bool instanceIsGetDelegate = false)
        {
            ParseResult = ParseBindingResult.None;
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
                        ParseResult |= ParseBindingResult.Get;
                    }
                    else
                    {
                        if (instance == null)
                        {
                            Debug.LogError("instanceDelegate is null");
                        }
                        else
                        {
                            if (instanceIsGetDelegate)
                            {
                                if (instance is Delegate getInstance)
                                {
                                    Type getterDelegateType = typeof(Func<,>).MakeGenericType(instanceType, typeof(T));

                                    //string message = $"MakeG {getterDelegateType} , {typeof(Func<Transform, string>)}";
                                    //Debug.Log(message);

                                    var getDeletgate = getMethod.CreateDelegate(getterDelegateType);

                                    //TODO 使用强类型委托 避免 DynamicInvoke
                                    //Func<object, T> getDeletgate2 = getDeletgate as Func<object, T>;

                                    Getter = () =>
                                    {
                                        var temp = getInstance.DynamicInvoke();
                                        var r = getDeletgate.DynamicInvoke(temp);
                                        return (T)r;
                                    };
                                    ParseResult |= ParseBindingResult.Get;
                                }
                            }
                            else
                            {
                                if (propertyInfo.PropertyType != typeof(T))
                                {
                                    //自动类型适配
                                    var adp = TypeAdpter.GetTypeAdpter<T>(propertyInfo.PropertyType);

                                    if (propertyInfo.TryGetGetDelegate(instanceType, instance, out var g, instanceIsGetDelegate))
                                    {
                                        if (adp != null && adp.TryGetGetDeletgate(g, out Getter))
                                        {
                                            ParseResult |= ParseBindingResult.Get;
                                        }
                                    }
                                }


                                Getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), instance, getMethod);
                                ParseResult |= ParseBindingResult.Get;
                            }
                        }
                    }



                }

                if (propertyInfo.CanWrite)
                {
                    var setMethod = propertyInfo.GetSetMethod();
                    if (setMethod.IsStatic)
                    {
                        Setter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), null, setMethod);
                        ParseResult |= ParseBindingResult.Set;
                    }
                    else
                    {
                        if (instance == null)
                        {
                            Debug.LogError("instanceDelegate is null");
                        }
                        else
                        {
                            if (instanceIsGetDelegate)
                            {
                                if (instance is Delegate getInstance)
                                {
                                    Type setterDelegateType = typeof(Action<,>).MakeGenericType(instanceType, typeof(T));

                                    //string message = $"MakeG {getterDelegateType} , {typeof(Func<Transform, string>)}";
                                    //Debug.Log(message);

                                    var setDelegate = setMethod.CreateDelegate(setterDelegateType);
                                    Setter = (value) =>
                                    {
                                        var temp = getInstance.DynamicInvoke();
                                        setDelegate.DynamicInvoke(temp, value);
                                    };
                                    ParseResult |= ParseBindingResult.Set;
                                }
                            }
                            else
                            {
                                if (propertyInfo.PropertyType != typeof(T))
                                {
                                    //自动类型适配  TODO
                                    var adp = TypeAdpter.GetTypeAdpter<T>(propertyInfo.PropertyType);

                                    var de = Delegate.CreateDelegate(typeof(Func<T>), instance, setMethod);

                                    if (adp != null && adp.TryGetSetDelegate(de, out Setter))
                                    {
                                        ParseResult |= ParseBindingResult.Get;
                                    }
                                }

                                Setter = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), instance, setMethod);
                                ParseResult |= ParseBindingResult.Set;
                            }
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
        /// TODO 强类型优化
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="getInstance"></param>
        /// <param name="memberName"></param>
        /// <param name="ParseResult"></param>
        /// <param name="Getter"></param>
        /// <param name="Setter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        public static bool TryCreatePropertyDelegate<I, T>(I instance,
                            Func<T> getInstance, string memberName,
                            out ParseBindingResult ParseResult,
                                                     out Func<T> Getter,
                                                     out Action<T> Setter,
                                                     bool instanceIsGetDelegate = false)
        {
            ParseResult = ParseBindingResult.None;
            Getter = null;
            Setter = null;
            return false;
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
        /// <param name="instanceIsGetDelegate">temp 是不是 delegate要明确指定，而不能用重载。否则遇到类型恰好是delegate是会出现冲突。
        /// <returns>是否含有成员</returns>
        public static bool TryCreateFieldDelegate<T>(Type instanceType,
                                                     object instance,
                                                     string memberName,
                                                     out ParseBindingResult ParseResult,
                                                     out Func<T> Getter,
                                                     out Action<T> Setter,
                                                     bool instanceIsGetDelegate = false)
        {
            ParseResult = ParseBindingResult.None;
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
                        ParseResult |= ParseBindingResult.Get;
                    }
                    else
                    {
                        if (instance == null)
                        {
                            Debug.LogError("instanceDelegate is null");
                        }
                        else
                        {
                            if (instanceIsGetDelegate)
                            {
                                if (instance is Delegate getInstance)
                                {
                                    Getter = () =>
                                    {
                                        var temp = getInstance.DynamicInvoke();
                                        var r = fieldInfo.GetValue(temp);
                                        return (T)r;
                                    };
                                    ParseResult |= ParseBindingResult.Get;
                                }
                            }
                            else
                            {
                                //TODO: fieldInfo.GetValue 是否有必要转换成委托？
                                //Func<object, object> func = fieldInfo.GetValue;
                                //Getter = () =>
                                //{
                                //    return (T)func(instance);
                                //};

                                Getter = () =>
                                {
                                    return (T)fieldInfo.GetValue(instance);
                                };
                                ParseResult |= ParseBindingResult.Get;
                            }
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
                        ParseResult |= ParseBindingResult.Set;
                    }
                    else
                    {
                        if (instance == null)
                        {
                            Debug.LogError("instanceDelegate is null");
                        }
                        else
                        {
                            if (instanceIsGetDelegate)
                            {
                                if (instance is Delegate getInstance)
                                {
                                    Setter = (value) =>
                                    {
                                        var temp = getInstance.DynamicInvoke();
                                        fieldInfo.SetValue(temp, value);
                                    };
                                    ParseResult |= ParseBindingResult.Set;
                                }
                            }
                            else
                            {
                                Setter = (value) =>
                                {
                                    fieldInfo.SetValue(instance, value);
                                };
                                ParseResult |= ParseBindingResult.Set;
                            }
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
        /// <param name="instanceIsGetDelegate">temp 是不是 delegate要明确指定，而不能用重载。否则遇到类型恰好是delegate是会出现冲突。
        /// <returns>是否含有成员</returns>
        public static bool TryCreateMethodDelegate<T>(Type instanceType,
                                                     object instance,
                                                     string memberName,
                                                     out ParseBindingResult ParseResult,
                                                     out Func<T> Getter,
                                                     out Action<T> Setter,
                                                     bool instanceIsGetDelegate = false)
        {
            ParseResult = ParseBindingResult.None;
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
                        ParseResult |= ParseBindingResult.Get;
                    }
                    else
                    {
                        if (instance == null)
                        {
                            Debug.LogError("instanceDelegate is null");
                        }
                        else
                        {
                            if (instanceIsGetDelegate)
                            {
                                if (instance is Delegate getInstance)
                                {
                                    Type getterDelegateType = typeof(Func<,>).MakeGenericType(instanceType, typeof(T));

                                    //string message = $"MakeG {getterDelegateType} , {typeof(Func<Transform, string>)}";
                                    //Debug.Log(message);

                                    var getDeletgate = methodInfo.CreateDelegate(getterDelegateType);

                                    Getter = () =>
                                    {
                                        var temp = getInstance.DynamicInvoke();
                                        var r = getDeletgate.DynamicInvoke(temp);
                                        return (T)r;
                                    };
                                    ParseResult |= ParseBindingResult.Get;
                                }
                            }
                            else
                            {
                                Getter = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), instance, methodInfo);
                                ParseResult |= ParseBindingResult.Get;
                            }
                        }
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <param name="instanceIsGetDelegate">instance 是不是 delegate要明确指定，而不能用重载。否则遇到类型恰好是delegate时会出现冲突。
        /// <returns></returns>
        public static (ParseBindingResult ParseResult, Func<T> Getter, Action<T> Setter)
            CreateDelegate<T>(Type instanceType,
                              object instance,
                              string memberName,
                              bool instanceIsGetDelegate = false)
        {
            ParseBindingResult ParseResult = ParseBindingResult.None;
            Func<T> Getter = null;
            Action<T> Setter = null;

            try
            {
                //属性 字段 方法 逐一尝试绑定。

                if (TryCreatePropertyDelegate(instanceType, instance, memberName, out ParseResult, out Getter, out Setter, instanceIsGetDelegate))
                {
                    return (ParseResult, Getter, Setter);
                }

                if (TryCreateFieldDelegate(instanceType, instance, memberName, out ParseResult, out Getter, out Setter, instanceIsGetDelegate))
                {
                    return (ParseResult, Getter, Setter);
                }

                if (TryCreateMethodDelegate(instanceType, instance, memberName, out ParseResult, out Getter, out Setter, instanceIsGetDelegate))
                {
                    return (ParseResult, Getter, Setter);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"ParseError:  {e}");
                return (ParseResult, Getter, Setter);
            }

            Debug.LogWarning($"{instanceType.FullName} 没有找到成员 {memberName}。请确认成员是否被IL2CPP剪裁。");
            return (ParseResult, Getter, Setter);
        }

        /// <summary>
        /// 生成一个获取memberName值的委托和memberName值类型。
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        /// <remarks>
        /// 考虑一种用例，一个属性返回一个class1类型，返回值时null，但是class1含有静态成员。
        /// 那么即使中间出现null实例，因为后续绑定是静态的，所以也能成立。
        /// null无法取得类型信息，所以instanceType和instance要独立返回。
        /// </remarks>
        public static (Delegate GetInstanceDelegate, Type InstanceType)
            GetGetInstanceDelegateAndReturnType(Type instanceType,
                                                object instance,
                                                string memberName,
                                                bool instanceIsGetDelegate = false)
        {
            //必须传instance 和Type,可能是静态类型。
            {
                var propertyInfo = instanceType.GetProperty(memberName);
                if (propertyInfo != null)
                {
                    if (propertyInfo.CanRead)
                    {
                        var getMethod = propertyInfo.GetGetMethod();
                        if (getMethod.IsStatic)
                        {
                            var getInstanceDelegate = Delegate.CreateDelegate(typeof(Func<object>), null, getMethod);
                            return (getInstanceDelegate, getMethod.ReturnType);
                        }
                        else
                        {
                            if (instance == null)
                            {
                                return (null, getMethod.ReturnType);
                            }
                            else
                            {
                                if (instanceIsGetDelegate)
                                {
                                    if (instance is Delegate getInstance)
                                    {
                                        Type getterDelegateType = typeof(Func<,>).MakeGenericType(instanceType, getMethod.ReturnType);

                                        //string message = $"MakeG {getterDelegateType} , {typeof(Func<Transform, string>)}";
                                        //Debug.Log(message);

                                        var getDeletgate = getMethod.CreateDelegate(getterDelegateType);

                                        Func<object> getInstanceDelegate = () =>
                                        {
                                            var temp = getInstance.DynamicInvoke();
                                            var r = getDeletgate.DynamicInvoke(temp);
                                            return r;
                                        };

                                        return (getInstanceDelegate, getMethod.ReturnType);
                                    }
                                }
                                else
                                {
                                    var getInstanceDelegate = Delegate.CreateDelegate(typeof(Func<object>), instance, getMethod);
                                    return (getInstanceDelegate, getMethod.ReturnType);
                                }
                            }
                        }
                    }
                }
            }

            {
                var fieldInfo = instanceType.GetField(memberName);
                if (fieldInfo != null)
                {
                    if (fieldInfo.IsStatic)
                    {
                        Func<object> getInstanceDelegate = () => { return fieldInfo.GetValue(null); };
                        return (getInstanceDelegate, fieldInfo.FieldType);
                    }
                    else
                    {
                        if (instance == null)
                        {
                            return (null, fieldInfo.FieldType);
                        }
                        else
                        {
                            if (instanceIsGetDelegate)
                            {
                                if (instance is Delegate getInstance)
                                {
                                    Func<object> getInstanceDelegate = () =>
                                    {
                                        var temp = getInstance.DynamicInvoke();
                                        return fieldInfo.GetValue(temp);
                                    };

                                    return (getInstanceDelegate, fieldInfo.FieldType);
                                }
                            }
                            else
                            {
                                Func<object> getInstanceDelegate = () => { return fieldInfo.GetValue(instance); };
                                return (getInstanceDelegate, fieldInfo.FieldType);
                            }
                        }

                    }
                }
            }

            {
                var methodInfo = instanceType.GetMethod(memberName);
                if (methodInfo != null)
                {
                    if (methodInfo.IsStatic)
                    {
                        var getInstanceDelegate = Delegate.CreateDelegate(typeof(Func<object>), null, methodInfo);
                        return (getInstanceDelegate, methodInfo.ReturnType);
                    }
                    else
                    {
                        if (instance == null)
                        {
                            return (null, methodInfo.ReturnType);
                        }
                        else
                        {
                            if (instanceIsGetDelegate)
                            {
                                if (instance is Delegate getInstance)
                                {
                                    Type getterDelegateType = typeof(Func<,>).MakeGenericType(instanceType, methodInfo.ReturnType);

                                    //string message = $"MakeG {getterDelegateType} , {typeof(Func<Transform, string>)}";
                                    //Debug.Log(message);

                                    var getDeletgate = methodInfo.CreateDelegate(getterDelegateType);

                                    Func<object> getInstanceDelegate = () =>
                                    {
                                        var temp = getInstance.DynamicInvoke();
                                        var r = getDeletgate.DynamicInvoke(temp);
                                        return r;
                                    };

                                    return (getInstanceDelegate, methodInfo.ReturnType);
                                }
                            }
                            else
                            {
                                var getInstanceDelegate = Delegate.CreateDelegate(typeof(Func<object>), instance, methodInfo);
                                return (getInstanceDelegate, methodInfo.ReturnType);
                            }
                        }
                    }
                }
            }

            return (null, null);
        }


        public static (object Intance, Type InstanceType)
            GetInstanceAndType(Type instanceType, object instance, string memberName)
        {
            //必须传instance 和Type,可能是静态类型。
            {
                var propertyInfo = instanceType.GetProperty(memberName);
                if (propertyInfo != null)
                {
                    var nextIntance = propertyInfo.GetValue(instance, null);
                    Type memberType = propertyInfo.PropertyType;
                    return (nextIntance, memberType);
                }
            }

            {
                var fieldInfo = instanceType.GetField(memberName);
                if (fieldInfo != null)
                {
                    var nextIntance = fieldInfo.GetValue(instance);
                    Type memberType = fieldInfo.FieldType;
                    return (nextIntance, memberType);
                }
            }

            {
                var methodInfo = instanceType.GetMethod(memberName);
                if (methodInfo != null)
                {
                    var nextIntance = methodInfo.Invoke(instance, null);
                    Type memberType = methodInfo.ReturnType;
                    return (nextIntance, memberType);
                }
            }

            return (null, null);
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
            var type = FindComponentType(typeFullName);

            if (type == null)
            {
                type = FindType(typeFullName);
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
















        /// <summary>
        /// TODO
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        class SetterWrapper<TTarget, TValue>
        {
            private Action<TTarget, TValue> _setter;
        }

        interface IGetValue
        {
            object Get(object target);
        }

        /// <summary>
        /// TODO 优化委托链调用
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        class GetterWrapper<TTarget, TValue> : IGetValue
        {
            private Func<TTarget, TValue> _getter;

            public GetterWrapper(PropertyInfo propertyInfo)
            {
                if (propertyInfo == null)
                    throw new ArgumentNullException("propertyInfo");

                if (propertyInfo.CanRead == false)
                    throw new InvalidOperationException("属性不支持读操作。");

                MethodInfo m = propertyInfo.GetGetMethod(true);
                _getter = (Func<TTarget, TValue>)Delegate.CreateDelegate(typeof(Func<TTarget, TValue>), null, m);
            }

            public TValue GetValue(TTarget target)
            {
                return _getter(target);
            }
            object IGetValue.Get(object target)
            {
                return _getter((TTarget)target);
            }
        }
    }
}
