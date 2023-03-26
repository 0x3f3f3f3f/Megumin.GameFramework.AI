using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Megumin.Reflection
{
    /// <summary>
    /// 重要： https://learn.microsoft.com/zh-cn/dotnet/csharp/programming-guide/concepts/covariance-contravariance/variance-in-delegates#variance-in-generic-type-parameters-for-value-and-reference-types
    /// https://learn.microsoft.com/zh-cn/dotnet/csharp/programming-guide/concepts/covariance-contravariance/
    /// </summary>
    public static class ReflectionExtension_9C4E15F3B30F4FCFBC57EDC2A99A69D0
    {
        public static void TestConvert()
        {
            {
                var b1 = typeof(string).IsAssignableFrom(typeof(object));   //false
                var b2 = typeof(object).IsAssignableFrom(typeof(string));   //true

                var b3 = typeof(int).IsAssignableFrom(typeof(object));   //false
                var b4 = typeof(object).IsAssignableFrom(typeof(int));   //true

                var b5 = typeof(float).IsAssignableFrom(typeof(int));   //false
                var b6 = typeof(int).IsAssignableFrom(typeof(float));   //false

                var b7 = typeof(float).IsAssignableFrom(typeof(double));   //false
                var b8 = typeof(double).IsAssignableFrom(typeof(float));   //false
            }

            {
                var b1 = typeof(string).IsSubclassOf(typeof(object));   //true
                var b2 = typeof(object).IsSubclassOf(typeof(string));   //false

                var b3 = typeof(int).IsSubclassOf(typeof(object));   //true
                var b4 = typeof(object).IsSubclassOf(typeof(int));   //false

                var b5 = typeof(float).IsSubclassOf(typeof(int));   //false
                var b6 = typeof(int).IsSubclassOf(typeof(float));   //false

                var b7 = typeof(float).IsSubclassOf(typeof(double));   //false
                var b8 = typeof(double).IsSubclassOf(typeof(float));   //false
            }


            int a = 200;
            float b = 300f;
            b = a;

            Func<string> funcstring = () => "";
            Func<int> funcint = () => 100;
            Func<float> funcfloat = () => 100f;

            Func<object> funcObj = funcstring;
            // Func<int> 不能协变成  Func<object> 也就认了，毕竟涉及到装箱。
            // Func<float> 协变成  Func<double>也不行? 无法理解
            //https://stackoverflow.com/questions/2169062/faster-way-to-cast-a-funct-t2-to-funct-object
            //https://learn.microsoft.com/zh-cn/dotnet/csharp/programming-guide/concepts/covariance-contravariance/using-variance-for-func-and-action-generic-delegates

            //能不能协变 与 能不能隐式转换 无关。
            //funcObj = funcint;
            //Func<float> funfloat2 = funcint;
            //Func<double> fundouble = funcfloat;
        }

        /// <summary>
        /// 测试是否需要类型适配器。
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanConvertDelegate(Type from, Type to)
        {
            var b1 = from.IsAssignableFrom(to);
            var b2 = to.IsAssignableFrom(from);
            var b3 = from.IsSubclassOf(to);
            var b4 = to.IsSubclassOf(from);


            if (from == to)
            {
                return true;
            }

            if (to.IsAssignableFrom(from))
            {
                if (from.IsValueType)
                {
                    //值类型通常都不能处理Func<To>协变，需要使用适配器转换
                    //https://learn.microsoft.com/zh-cn/dotnet/csharp/programming-guide/concepts/covariance-contravariance/variance-in-delegates#variance-in-generic-type-parameters-for-value-and-reference-types
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 测试是否需要类型适配器。
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanConvertDelegate<T>(this Type from)
        {
            return CanConvertDelegate(from, typeof(T));
        }

        /// <summary>
        /// 使用类型适配器，尝试将成员创建为 <![CDATA[Func<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <typeparam name="T">类型适配器类型</typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateGetterUseTypeAdpter<T>(this PropertyInfo propertyInfo,
                                                           Type instanceType,
                                                           object instance,
                                                           out Func<T> getter,
                                                           bool instanceIsGetDelegate = false)
        {
            if (propertyInfo.CanRead)
            {
                if (propertyInfo.GetMethod.TryCreateGetterUseTypeAdpter(
                    instanceType, instance, out getter, instanceIsGetDelegate))
                {
                    return true;
                }
            }

            getter = null;
            return false;
        }

        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Func<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateGetter<T>(this PropertyInfo propertyInfo,
                                              Type instanceType,
                                              object instance,
                                              out Func<T> getter,
                                              bool instanceIsGetDelegate = false)
        {
            if (propertyInfo.CanRead)
            {
                if (TryCreateGetter(propertyInfo.GetMethod, instanceType, instance, out getter, instanceIsGetDelegate))
                {
                    return true;
                }
            }

            getter = null;
            return false;
        }

        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Func<ReturnType>]]> 的强类型委托，并以Delegate类型返回。
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateGetter(this PropertyInfo propertyInfo,
                                           Type instanceType,
                                           object instance,
                                           out Delegate getter,
                                           bool instanceIsGetDelegate = false)
        {
            if (propertyInfo.CanRead)
            {
                if (TryCreateGetter(propertyInfo.GetMethod, instanceType, instance, out getter, instanceIsGetDelegate))
                {
                    return true;
                }
            }

            getter = null;
            return false;
        }

        /// <summary>
        /// 使用类型适配器，尝试将成员创建为 <![CDATA[Func<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateGetterUseTypeAdpter<T>(this MethodInfo methodInfo,
                                                           Type instanceType,
                                                           object instance,
                                                           out Func<T> getter,
                                                           bool instanceIsGetDelegate = false)
        {
            Type dataType = methodInfo.ReturnType;
            if (dataType.CanConvertDelegate<T>() == false)
            {
                //自动类型适配
                var adp = TypeAdpter.FindGetAdpter<T>(dataType);

                if (adp == null)
                {
                    Debug.LogWarning($"TryCreateGetterUseTypeAdpter : 成员类型{dataType}无法适配目标类型{typeof(T)}, 并且没有找到对应的TypeAdpter<{dataType},{typeof(T)}>");
                }
                else
                {
                    if (methodInfo.TryCreateGetter(instanceType, instance, out var g, instanceIsGetDelegate))
                    {
                        if (adp.TryCreateGetter(g, out getter))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return methodInfo.TryCreateGetter(instanceType, instance, out getter, instanceIsGetDelegate);
            }

            getter = null;
            return false;
        }

        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Func<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateGetter<T>(this MethodInfo methodInfo,
                                              Type instanceType,
                                              object instance,
                                              out Func<T> getter,
                                              bool instanceIsGetDelegate = false)
        {
            if (TryCreateGetter(methodInfo, instanceType, instance, out var mygetter, instanceIsGetDelegate))
            {
                var typeP = methodInfo.ReturnType;
                var typeV = typeof(T);
                if (mygetter is Func<T> mygetterGeneric)
                {
                    getter = mygetterGeneric;
                    return true;
                }
                else
                {
                    Debug.LogWarning($"{mygetter.GetType()} <color=#ff0000>IS NOT</color> {typeof(Func<T>)}.");
                }
            }
            getter = null;
            return false;
        }

        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Func<ReturnType>]]> 的强类型委托，并以Delegate类型返回。
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateGetter(this MethodInfo methodInfo,
                                           Type instanceType,
                                           object instance,
                                           out Delegate getter,
                                           bool instanceIsGetDelegate = false)
        {
            getter = null;
            //var paras = methodInfo.GetParameters();
            Type dataType = methodInfo.ReturnType;
            Type delagateType = typeof(Func<>).MakeGenericType(dataType);
            if (methodInfo.IsStatic)
            {
                getter = methodInfo.CreateDelegate(delagateType, null);
                //getter = Delegate.CreateDelegate(delagateType, null, methodInfo);
                return true;
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
                            var connector = DelegateConnector.Get(instanceType, dataType);
                            if (connector.TryConnectGet(getInstance, methodInfo, out getter))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        getter = methodInfo.CreateDelegate(delagateType, instance);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 使用类型适配器，尝试将成员创建为 <![CDATA[Func<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <typeparam name="T">类型适配器类型</typeparam>
        /// <param name="fieldInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateGetterUseTypeAdpter<T>(this FieldInfo fieldInfo,
                                                           Type instanceType,
                                                           object instance,
                                                           out Func<T> getter,
                                                           bool instanceIsGetDelegate = false)
        {
            Type dataType = fieldInfo.FieldType;
            if (dataType.CanConvertDelegate<T>() == false)
            {
                //自动类型适配
                var adp = TypeAdpter.FindGetAdpter<T>(dataType);

                if (adp == null)
                {
                    Debug.LogWarning($"TryCreateGetterUseTypeAdpter : 成员类型{dataType}无法适配目标类型{typeof(T)}, 并且没有找到对应的TypeAdpter<{dataType},{typeof(T)}>");
                }
                else
                {
                    if (fieldInfo.TryCreateGetter(instanceType, instance, out var g, instanceIsGetDelegate))
                    {
                        if (adp.TryCreateGetter(g, out getter))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return fieldInfo.TryCreateGetter(instanceType, instance, out getter, instanceIsGetDelegate);
            }

            getter = null;
            return false;
        }

        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Func<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateGetter<T>(this FieldInfo fieldInfo,
                                              Type instanceType,
                                              object instance,
                                              out Func<T> getter,
                                              bool instanceIsGetDelegate = false)
        {
            if (TryCreateGetter(fieldInfo, instanceType, instance, out var mygetter, instanceIsGetDelegate))
            {
                var typeP = fieldInfo.FieldType;
                var typeV = typeof(T);
                if (mygetter is Func<T> mygetterGeneric)
                {
                    getter = mygetterGeneric;
                    return true;
                }
                else
                {
                    Debug.LogWarning($"{mygetter.GetType()} <color=#ff0000>IS NOT</color> {typeof(Func<T>)}.");
                }
            }
            getter = null;
            return false;
        }

        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Func<ReturnType>]]> 的强类型委托，并以Delegate类型返回。
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateGetter(this FieldInfo fieldInfo,
                                           Type instanceType,
                                           object instance,
                                           out Delegate getter,
                                           bool instanceIsGetDelegate = false)
        {
            var creatorType = typeof(FieldWrapper<>).MakeGenericType(fieldInfo.FieldType);
            var creator = (IFieldCreateGetterSetter)Activator.CreateInstance(creatorType);
            return creator.TryCreateGetter(fieldInfo, instanceType, instance, out getter, instanceIsGetDelegate);
        }














        /// <summary>
        /// 使用类型适配器，尝试将成员创建为 <![CDATA[Action<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="setter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateSetterUseTypeAdpter<T>(this PropertyInfo propertyInfo,
                                                           Type instanceType,
                                                           object instance,
                                                           out Action<T> setter,
                                                           bool instanceIsGetDelegate = false)
        {
            if (propertyInfo.CanWrite)
            {
                if (propertyInfo.SetMethod.TryCreateSetterUseTypeAdpter(
                    instanceType, instance, out setter, instanceIsGetDelegate))
                {
                    return true;
                }
            }

            setter = null;
            return false;
        }

        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Action<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="setter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateSetter<T>(this PropertyInfo propertyInfo,
                                              Type instanceType,
                                              object instance,
                                              out Action<T> setter,
                                              bool instanceIsGetDelegate = false)
        {
            if (propertyInfo.CanRead)
            {
                if (TryCreateSetter(propertyInfo.GetMethod, instanceType, instance, out setter, instanceIsGetDelegate))
                {
                    return true;
                }
            }

            setter = null;
            return false;
        }

        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Action<ReturnType>]]> 的强类型委托，并以Delegate类型返回。
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="setter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateSetter(this PropertyInfo propertyInfo,
                                           Type instanceType,
                                           object instance,
                                           out Delegate setter,
                                           bool instanceIsGetDelegate = false)
        {
            if (propertyInfo.CanRead)
            {
                if (TryCreateSetter(propertyInfo.SetMethod, instanceType, instance, out setter, instanceIsGetDelegate))
                {
                    return true;
                }
            }

            setter = null;
            return false;
        }

        /// <summary>
        /// 使用类型适配器，尝试将成员创建为 <![CDATA[Action<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="setter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateSetterUseTypeAdpter<T>(this MethodInfo methodInfo,
                                                           Type instanceType,
                                                           object instance,
                                                           out Action<T> setter,
                                                           bool instanceIsGetDelegate = false)
        {
            var paras = methodInfo.GetParameters();
            Type dataType = paras[0].ParameterType;
            if (CanConvertDelegate(typeof(T), dataType) == false)
            {
                //自动类型适配
                var adp = TypeAdpter.FindSetAdpter<T>(dataType);

                if (adp == null)
                {
                    Debug.LogWarning($"TryCreateSetterUseTypeAdpter : 成员类型{typeof(T)}无法适配目标类型{dataType}, 并且没有找到对应的TypeAdpter<{typeof(T)},{dataType}>");
                }
                else
                {
                    if (methodInfo.TryCreateSetter(instanceType, instance, out var g, instanceIsGetDelegate))
                    {
                        if (adp.TryCreateSetter(g, out setter))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return methodInfo.TryCreateSetter(instanceType, instance, out setter, instanceIsGetDelegate);
            }

            setter = null;
            return false;
        }

        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Action<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="setter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateSetter<T>(this MethodInfo methodInfo,
                                              Type instanceType,
                                              object instance,
                                              out Action<T> setter,
                                              bool instanceIsGetDelegate = false)
        {
            if (TryCreateSetter(methodInfo, instanceType, instance, out var mysetter, instanceIsGetDelegate))
            {
                var typeP = methodInfo.ReturnType;
                var typeV = typeof(T);
                if (mysetter is Action<T> mysetterGeneric) //逆变
                {
                    setter = mysetterGeneric;
                    return true;
                }
                else if (mysetter is Func<T, object> mysettGenericR)
                {
                    //尝试忽略返回值，
                    //不知道会不会造成意外的返回值装箱？
                    setter = (value) =>
                    {
                        _ = mysettGenericR(value);
                    };
                    return true;
                }
                else
                {
                    Debug.LogWarning($"{mysetter.GetType()} <color=#ff0000>IS NOT</color> {typeof(Action<T>)}.");
                }
            }
            setter = null;
            return false;
        }

        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Action<ReturnType>]]> 的强类型委托，并以Delegate类型返回。
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        /// <remarks>接收一个目标参数的方法</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateSetter(this MethodInfo methodInfo,
                                           Type instanceType,
                                           object instance,
                                           out Delegate setter,
                                           bool instanceIsGetDelegate = false)
        {
            setter = null;
            var paras = methodInfo.GetParameters();
            Type dataType = paras[0].ParameterType;
            Type delagateType = null;
            if (methodInfo.ReturnType == typeof(void))
            {
                delagateType = typeof(Action<>).MakeGenericType(dataType);
            }
            else
            {
                delagateType = typeof(Func<,>).MakeGenericType(dataType, methodInfo.ReturnType);
            }

            if (methodInfo.IsStatic)
            {
                setter = methodInfo.CreateDelegate(delagateType, null);
                //getter = Delegate.CreateDelegate(delagateType, null, methodInfo);
                return true;
            }
            else
            {
                if (instance == null)
                {
                    Debug.LogWarning("instanceDelegate is null");
                }
                else
                {
                    if (instanceIsGetDelegate)
                    {
                        if (instance is Delegate getInstance)
                        {
                            var connector = DelegateConnector.Get(instanceType, dataType);
                            if (connector.TryConnectSet(getInstance, methodInfo, out setter))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        setter = methodInfo.CreateDelegate(delagateType, instance);
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 使用类型适配器，尝试将成员创建为 <![CDATA[Action<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="setter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateSetterUseTypeAdpter<T>(this FieldInfo fieldInfo,
                                                           Type instanceType,
                                                           object instance,
                                                           out Action<T> setter,
                                                           bool instanceIsGetDelegate = false)
        {
            Type dataType = fieldInfo.FieldType;
            if (CanConvertDelegate(typeof(T), dataType) == false)
            {
                //自动类型适配
                var adp = TypeAdpter.FindSetAdpter<T>(dataType);

                if (adp == null)
                {
                    Debug.LogWarning($"TryCreateSetterUseTypeAdpter : 成员类型{typeof(T)}无法适配目标类型{dataType}, 并且没有找到对应的TypeAdpter<{typeof(T)},{dataType}>");
                }
                else
                {
                    if (fieldInfo.TryCreateSetter(instanceType, instance, out var g, instanceIsGetDelegate))
                    {
                        if (adp.TryCreateSetter(g, out setter))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return fieldInfo.TryCreateSetter(instanceType, instance, out setter, instanceIsGetDelegate);
            }

            setter = null;
            return false;
        }

        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Action<T>]]> 的强类型委托。T 是目标类型，不是成员类型。
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="setter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateSetter<T>(this FieldInfo fieldInfo,
                                              Type instanceType,
                                              object instance,
                                              out Action<T> setter,
                                              bool instanceIsGetDelegate = false)
        {
            if (TryCreateSetter(fieldInfo, instanceType, instance, out var mysetter, instanceIsGetDelegate))
            {
                var typeP = fieldInfo.FieldType;
                var typeV = typeof(T);
                if (mysetter is Action<T> mysetterGeneric) //逆变
                {
                    setter = mysetterGeneric;
                    return true;
                }
                else
                {
                    Debug.LogWarning($"{mysetter.GetType()} <color=#ff0000>IS NOT</color> {typeof(Action<T>)}.");
                }
            }
            setter = null;
            return false;
        }


        /// <summary>
        /// 尝试将成员创建为 <![CDATA[Action<ReturnType>]]> 的强类型委托，并以Delegate类型返回。
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCreateSetter(this FieldInfo fieldInfo,
                                           Type instanceType,
                                           object instance,
                                           out Delegate setter,
                                           bool instanceIsGetDelegate = false)
        {
            var creatorType = typeof(FieldWrapper<>).MakeGenericType(fieldInfo.FieldType);
            var creator = (IFieldCreateGetterSetter)Activator.CreateInstance(creatorType);
            return creator.TryCreateSetter(fieldInfo, instanceType, instance, out setter, instanceIsGetDelegate);
        }





        internal interface IFieldCreateGetterSetter
        {
            bool TryCreateGetter(FieldInfo fieldInfo,
                                 Type instanceType,
                                 object instance,
                                 out Delegate getter,
                                 bool instanceIsGetDelegate = false);

            bool TryCreateSetter(FieldInfo fieldInfo,
                                 Type instanceType,
                                 object instance,
                                 out Delegate setter,
                                 bool instanceIsGetDelegate = false);
        }

        /// <summary>
        /// FieldInfo 没有GetMethod,SetMethod,  
        /// <see cref="FieldInfo.GetValue(object)"/><see cref="FieldInfo.SetValue(object, object)"/>
        /// 无法转换成强类型委托，所以只能创建一个强类型包装一下。
        /// T 必须是字段类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// TODO: <see cref="FieldInfo.SetValueDirect(TypedReference, object)"/>能不能转成强类型委托？
        /// </remarks>
        internal class FieldWrapper<T> : IFieldCreateGetterSetter
        {
            ///测试发现TryCreateGetter，TryCreateSetter 即使改为静态函数，再包装一次，也不能消除FieldWrapper<T>的实例。
            ///实例的生命周期跟随生成的委托。

            public bool TryCreateGetter(FieldInfo fieldInfo,
                                        Type instanceType,
                                        object instance,
                                        out Delegate getter,
                                        bool instanceIsGetDelegate = false)
            {
                getter = null;
                if (fieldInfo.FieldType != typeof(T))
                {
                    Debug.LogWarning($"System.Func`1[{fieldInfo.FieldType}] <color=#ff0000>IS NOT</color> {typeof(Func<T>)}.");
                    return false;
                }

                if (fieldInfo.IsStatic)
                {
                    Func<T> myGetter = () =>
                    {
                        return (T)fieldInfo.GetValue(null);
                    };
                    getter = myGetter;
                    return true;
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
                                Func<T> myGetter = () =>
                                {
                                    var temp = getInstance.DynamicInvoke();
                                    var r = fieldInfo.GetValue(temp);
                                    return (T)r;
                                };
                                getter = myGetter;
                                return true;
                            }
                        }
                        else
                        {
                            //TODO: fieldInfo.GetValue 是否有必要转换成委托？
                            //Func<object, object> func = fieldInfo.GetValue;
                            //Getter = () =>
                            //{
                            //    return (To)func(instance);
                            //};

                            Func<T> myGetter = () =>
                            {
                                return (T)fieldInfo.GetValue(instance);
                            };
                            getter = myGetter;
                            return true;
                        }
                    }
                }

                return false;
            }

            public bool TryCreateSetter(FieldInfo fieldInfo,
                                        Type instanceType,
                                        object instance,
                                        out Delegate setter,
                                        bool instanceIsGetDelegate = false)
            {
                setter = null;

                if (fieldInfo.FieldType != typeof(T))
                {
                    Debug.LogWarning($"System.Action`1[{fieldInfo.FieldType}] <color=#ff0000>IS NOT</color> {typeof(Action<T>)}.");
                    return false;
                }

                if (fieldInfo.IsInitOnly)
                {
                    return false;
                }

                if (fieldInfo.IsStatic)
                {
                    Action<T> mySetter = (value) =>
                    {
                        fieldInfo.SetValue(null, value);
                    };

                    setter = mySetter;
                    return true;
                }
                else
                {
                    if (instance == null)
                    {
                        Debug.LogWarning("instanceDelegate is null");
                    }
                    else
                    {
                        if (instanceIsGetDelegate)
                        {
                            if (instance is Delegate getInstance)
                            {
                                Action<T> mySetter = (value) =>
                                {
                                    var temp = getInstance.DynamicInvoke();
                                    fieldInfo.SetValue(temp, value);
                                };
                                setter = mySetter;
                                return true;
                            }
                        }
                        else
                        {
                            Action<T> mySetter = (value) =>
                            {
                                fieldInfo.SetValue(instance, value);
                            };
                            setter = mySetter;
                            return true;
                        }
                    }
                }

                return false;
            }
        }


    }
}
