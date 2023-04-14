using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Megumin.Reflection;
using UnityEngine;
using static Megumin.Reflection.TypeCache;

namespace Megumin.Binding
{
    /// <summary>
    /// BindingString格式:  (组件类|静态类|接口)/成员/....../成员/成员。  
    /// 最后一个成员的类型需要满足绑定类型，或者可以通过类型适配器转换成绑定类型。
    /// 
    /// https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
    /// https://www.cnblogs.com/xinaixia/p/5777886.html
    /// </summary>
    public partial class UnityBindingParser : BindingParser
    {
        /// <summary>
        /// 这里自动初始化，如果导致项目启动过慢，请修改此处，并手动在适当位置初始化。
        /// </summary>
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            Instance = new UnityBindingParser();

            //if (Application.isPlaying) ///编辑器模式不要预调研，频繁修改代码会很卡并且打印警告
            //{
            //    ///预调用
            //    CacheAllTypesAsync();
            //}
        }

        public override (ParseBindingResult ParseResult, Func<T> Getter, Action<T> Setter)
            ParseBinding<T>(string bindingString, object bindingInstance, object options = null)
        {
            ParseBindingResult ParseResult = ParseBindingResult.None;
            Func<T> Getter = null;
            Action<T> Setter = null;


            if (string.IsNullOrEmpty(bindingString) || string.IsNullOrEmpty(bindingString.Trim()))
            {
                //空白内容默认解析失败。
            }
            else
            {
                GameObject rootInstance = GetRootInstance(bindingInstance);

                var path = bindingString.Split('/');
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
                        if (options is IParseBindingInstanceMode mode && mode.UseInstaneceDelegate)
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

        public static GameObject GetRootInstance(object bindingInstance)
        {
            GameObject rootInstance = bindingInstance as GameObject;

            if (!rootInstance)
            {
                if (bindingInstance is Component component)
                {
                    rootInstance = component.gameObject;
                }
            }

            return rootInstance;
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
            bool hasMember = false;

            try
            {
                var propertyInfo = instanceType.GetProperty(memberName);
                if (propertyInfo != null)
                {
                    hasMember = true;
                    if (propertyInfo.TryCreateGetterUseTypeAdpter(instanceType,
                        instance, out Getter, instanceIsGetDelegate))
                    {
                        ParseResult |= ParseBindingResult.Get;
                    }

                    if (propertyInfo.TryCreateSetterUseTypeAdpter(instanceType,
                        instance, out Setter, instanceIsGetDelegate))
                    {
                        ParseResult |= ParseBindingResult.Set;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"ParseError:  {e}");
            }

            return hasMember;
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
            bool hasMember = false;

            try
            {
                var fieldInfo = instanceType.GetField(memberName);
                if (fieldInfo != null)
                {
                    hasMember = true;
                    if (fieldInfo.TryCreateGetterUseTypeAdpter(instanceType, instance, out Getter, instanceIsGetDelegate))
                    {
                        ParseResult |= ParseBindingResult.Get;
                    }

                    if (fieldInfo.TryCreateSetterUseTypeAdpter(instanceType, instance, out Setter, instanceIsGetDelegate))
                    {
                        ParseResult |= ParseBindingResult.Set;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"ParseError:  {e}");
            }
            return hasMember;
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
            bool hasMember = false;

            try
            {
                MethodInfo methodInfo = GetMethodInfo(instanceType, memberName);
                //TODO，区分方法重载

                if (methodInfo != null)
                {
                    hasMember = true;

                    var paras = methodInfo.GetParameters();
                    if (paras.Length < 2)
                    {
                        if (paras.Length == 0)
                        {
                            //没有参数时认为是Get绑定
                            if (methodInfo.TryCreateGetterUseTypeAdpter(instanceType,
                                        instance, out Getter, instanceIsGetDelegate))
                            {
                                ParseResult |= ParseBindingResult.Get;
                            }
                        }
                        else if (paras.Length == 1)
                        {
                            if (false && methodInfo.ReturnType != typeof(void))
                            {
                                //暂时不检查返回值
                                //返回值必须是void。
                                Debug.LogWarning($"SetMethod must return void.");
                            }
                            else
                            {
                                //一个参数时认为是Set绑定
                                if (methodInfo.TryCreateSetterUseTypeAdpter(instanceType,
                                            instance, out Setter, instanceIsGetDelegate))
                                {
                                    ParseResult |= ParseBindingResult.Set;
                                }
                            }
                        }
                    }
                    else
                    {
                        //TODO 多个参数
                        Debug.LogWarning($"暂不支持 含有参数的方法 {methodInfo}绑定");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"ParseError:  {e}");
            }
            return hasMember;
        }

        public static MethodInfo GetMethodInfo(Type type, string memberName)
        {
            var methodName = memberName;
            if (memberName.EndsWith("()"))
            {
                methodName = memberName.Replace("()", "");
                //TODO 泛型函数
            }

            var methodInfo = type.GetMethod(methodName);
            return methodInfo;
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

            Debug.LogWarning($"通过 {instanceType.FullName}类型 没有找到 符合标准的 成员 {memberName}。请确认成员是否被IL2CPP剪裁。");
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
                    if (propertyInfo.TryCreateGetter(instanceType, instance, out var pGetter, instanceIsGetDelegate))
                    {
                        return (pGetter, propertyInfo.PropertyType);
                    }
                }
            }

            {
                var fieldInfo = instanceType.GetField(memberName);
                if (fieldInfo != null)
                {
                    if (fieldInfo.TryCreateGetter(instanceType, instance, out var pGetter, instanceIsGetDelegate))
                    {
                        return (pGetter, fieldInfo.FieldType);
                    }

                    //if (fieldInfo.IsStatic)
                    //{
                    //    Func<object> getInstanceDelegate = () => { return fieldInfo.GetValue(null); };
                    //    return (getInstanceDelegate, fieldInfo.FieldType);
                    //}
                    //else
                    //{
                    //    if (instance == null)
                    //    {
                    //        return (null, fieldInfo.FieldType);
                    //    }
                    //    else
                    //    {
                    //        if (instanceIsGetDelegate)
                    //        {
                    //            if (instance is Delegate getInstance)
                    //            {
                    //                Func<object> getInstanceDelegate = () =>
                    //                {
                    //                    var temp = getInstance.DynamicInvoke();
                    //                    return fieldInfo.GetValue(temp);
                    //                };

                    //                return (getInstanceDelegate, fieldInfo.FieldType);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Func<object> getInstanceDelegate = () => { return fieldInfo.GetValue(instance); };
                    //            return (getInstanceDelegate, fieldInfo.FieldType);
                    //        }
                    //    }

                    //}
                }
            }

            {
                var methodInfo = instanceType.GetMethod(memberName);
                if (methodInfo != null)
                {
                    if (methodInfo.TryCreateGetter(instanceType, instance, out var pGetter, instanceIsGetDelegate))
                    {
                        return (pGetter, methodInfo.ReturnType);
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
        /// <remarks>
        /// 可能是静态类型，不能只返回instance。
        /// </remarks>
        public static (object Instance, Type InstanceType)
            GetBindInstanceAndType(string typeFullName, GameObject gameObject)
        {
            if (typeFullName == "UnityEngine.GameObject" ||
                typeFullName == nameof(GameObject))
            {
                return (gameObject, typeof(UnityEngine.GameObject));
            }

            //TODO，绑定接口，通过接口取得组件
            var type = GetComponentType(typeFullName);

            if (type == null)
            {
                //没有找到Component，可能是一个接口类型。尝试取得一个实现了接口的组件。
                type = Reflection.TypeCache.GetType(typeFullName);
                if (type != null && type.IsInterface)
                {
                    //通过名字不能从Children获取组件，还是要自己先取得类型
                    //var comp = gameObject.GetComponent(typeFullName);
                    var comp = gameObject.GetComponentInChildren(type);
                    if (comp)
                    {
                        return (comp, comp.GetType());
                    }
                    else
                    {
                        //没有找到组件实例返回空而不返回gameObject，防止设计之外的错误绑定。
                        return (null, type);
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
        /// Unity和纯C#运行时解析逻辑时不一样的，unity中第一个字符串表示组件，在纯C#运行时可能会忽略第一个字符串。
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="rootInstance"></param>
        /// <returns></returns>
        /// <remarks>
        /// 可能是静态类型，不能只返回instance。
        /// </remarks>
        public static (object Instance, Type InstanceType)
            GetBindInstanceAndType2(Span<string> path, GameObject rootInstance)
        {
            if (path.Length == 0)
            {
                return (rootInstance, typeof(GameObject));
            }
            else if (path.Length == 1)
            {
                return GetBindInstanceAndType(path[0], rootInstance);
            }
            else
            {
                //多个层级
                var (instance, instanceType) = GetBindInstanceAndType(path[0], rootInstance);

                //使用实例链的方式处理多级层级绑定
                object innerIntance = instance;
                Type innerInStanceType = instanceType;

                for (int i = 1; i < path.Length; i++)
                {
                    //一级一级反射获取实例
                    var member = path[i];
                    //处理中间层级 每级都取得实例，优点是最终生成的委托性能较高。缺点是中间级别如果对象重新赋值，需要重新绑定。
                    (innerIntance, innerInStanceType) = GetInstanceAndType(innerInStanceType, innerIntance, member);
                }
                return (innerIntance, innerInStanceType);
            }
        }

    }

    public partial class UnityBindingParser
    {
        public override bool TryCreateMethodDelegate(string bindingPath,
                                            object bindingInstance,
                                            Type delegateType,
                                            out Delegate methodDelegate)
        {
            var path = bindingPath.Split('/');
            if (path.Length <= 1)
            {
                methodDelegate = null;
                return false;
            }

            var rootInstance = GetRootInstance(bindingInstance);

            var (instance, instanceType) = GetBindInstanceAndType2(new Span<string>(path, 0, path.Length - 1), rootInstance);
            var memberName = path[path.Length - 1];
            MethodInfo methodInfo = GetMethodInfo(instanceType, memberName);
            if (methodInfo != null)
            {
                var d = methodInfo.CreateDelegate(delegateType, instance);
                methodDelegate = d;
                return true;
            }
            methodDelegate = null;
            return false;
        }
    }
}




