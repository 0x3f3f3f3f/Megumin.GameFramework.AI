using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Megumin.Reflection
{
    [Flags]
    public enum CreateDelegateResult
    {
        /// <summary>
        /// Get Set 均解析失败
        /// </summary>
        None = 0,
        Get = 1 << 0,
        Set = 1 << 1,
        Both = Get | Set,
        Method = 1 << 2,
    }

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




        //类型根据成员名创建委托

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
        public static bool TryCreatePropertyDelegate<T>(this Type instanceType,
                                                        object instance,
                                                        string memberName,
                                                        out CreateDelegateResult ParseResult,
                                                        out Func<T> Getter,
                                                        out Action<T> Setter,
                                                        bool instanceIsGetDelegate = false)
        {
            ParseResult = CreateDelegateResult.None;
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
                        ParseResult |= CreateDelegateResult.Get;
                    }

                    if (propertyInfo.TryCreateSetterUseTypeAdpter(instanceType,
                        instance, out Setter, instanceIsGetDelegate))
                    {
                        ParseResult |= CreateDelegateResult.Set;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"TryCreatePropertyDelegate:  {e}");
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
        public static bool TryCreateFieldDelegate<T>(this Type instanceType,
                                                     object instance,
                                                     string memberName,
                                                     out CreateDelegateResult ParseResult,
                                                     out Func<T> Getter,
                                                     out Action<T> Setter,
                                                     bool instanceIsGetDelegate = false)
        {
            ParseResult = CreateDelegateResult.None;
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
                        ParseResult |= CreateDelegateResult.Get;
                    }

                    if (fieldInfo.TryCreateSetterUseTypeAdpter(instanceType, instance, out Setter, instanceIsGetDelegate))
                    {
                        ParseResult |= CreateDelegateResult.Set;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"TryCreateFieldDelegate:  {e}");
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
        public static bool TryCreateMethodDelegate<T>(this Type instanceType,
                                                      object instance,
                                                      string memberName,
                                                      out CreateDelegateResult ParseResult,
                                                      out Func<T> Getter,
                                                      out Action<T> Setter,
                                                      bool instanceIsGetDelegate = false)
        {
            ParseResult = CreateDelegateResult.None;
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
                                ParseResult |= CreateDelegateResult.Get;
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
                                    ParseResult |= CreateDelegateResult.Set;
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
                Debug.LogWarning($"TryCreateMethodDelegate:  {e}");
            }
            return hasMember;
        }

        /// <summary>
        /// 反射查找方法，自动识别修正"()"
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(this Type type, string memberName)
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

        public static readonly Regex GetIndexRegex = new(@"^\[(?<index>.+)\]$");

        /// <summary>
        /// 尝试绑定索引器
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
        public static bool TryCreateIndexerDelegate<T>(this Type instanceType,
                                                       object instance,
                                                       string memberName,
                                                       out CreateDelegateResult ParseResult,
                                                       out Func<T> Getter,
                                                       out Action<T> Setter,
                                                       bool instanceIsGetDelegate = false)
        {
            ParseResult = CreateDelegateResult.None;
            Getter = null;
            Setter = null;
            bool indexMatched = false;

            try
            {
                var match = GetIndexRegex.Match(memberName);
                if (match.Success)
                {
                    var index = match.Groups["index"].Value;

                    if (int.TryParse(index, out var intIndex))
                    {
                        //针对Int索引器进行优化
                        if (instance is IList<T> tList)
                        {
                            //针对IList进行优化
                            indexMatched = true;

                            Getter = () =>
                            {
                                return tList[intIndex];
                            };
                            ParseResult |= CreateDelegateResult.Get;

                            Setter = (T value) =>
                            {
                                tList[intIndex] = value;
                            };
                            ParseResult |= CreateDelegateResult.Set;
                        }
                        else if (instance is T[] tArray)
                        {
                            //针对数组进行优化
                            indexMatched = true;

                            Getter = () =>
                            {
                                return tArray[intIndex];
                            };
                            ParseResult |= CreateDelegateResult.Get;

                            Setter = (T value) =>
                            {
                                tArray[intIndex] = value;
                            };
                            ParseResult |= CreateDelegateResult.Set;
                        }
                        else if (instance is IDictionary<int, T> intDic)
                        {
                            //针对int字典进行优化
                            indexMatched = true;
                            Getter = () =>
                            {
                                return intDic[intIndex];
                            };
                            ParseResult |= CreateDelegateResult.Get;

                            Setter = (T value) =>
                            {
                                intDic[intIndex] = value;
                            };
                            ParseResult |= CreateDelegateResult.Set;
                        }
                    }

                    if (indexMatched == false && instance is IDictionary<string, T> stringDic)
                    {
                        //针对string字典进行优化
                        indexMatched = true;
                        Getter = () =>
                        {
                            return stringDic[index];
                        };
                        ParseResult |= CreateDelegateResult.Get;

                        Setter = (T value) =>
                        {
                            stringDic[index] = value;
                        };
                        ParseResult |= CreateDelegateResult.Set;
                    }

                    //if (instance is IDictionary objectDic)
                    //{
                    //    //TODO
                    //    //针对通用字典进行优化
                    //    indexMatched = true;
                    //}

                    if (indexMatched == false)
                    {
                        //普通索引器 通用方式创建委托

                        //为索引器创建委托 
                        var props = instanceType.GetProperties();

                        //索引器是名字为Item的属性
                        //TODO： 索引器重载
                        var prop = props.FirstOrDefault(elem => elem.Name == "Item");
                        if (prop != null)
                        {

                            if (prop.CanRead)
                            {
                                MethodInfo indexGet = prop.GetGetMethod();
                                var indexType = indexGet.GetParameters()[0].ParameterType;

                                indexMatched = true;

                                //这里无法避免装箱,反射调用索引器时同样也需要装箱
                                object indexValue = Convert.ChangeType(index, indexType);

                                Getter = () =>
                                {
                                    //需要转型
                                    var result = indexGet.Invoke(instance, new object[] { indexValue });
                                    //如果正确匹配，返回值强转为T是安全的。
                                    return (T)result;
                                };
                                ParseResult |= CreateDelegateResult.Get;
                            }

                            if (prop.CanWrite)
                            {
                                MethodInfo indexSet = prop.GetSetMethod();
                                var indexType = indexSet.GetParameters()[0].ParameterType;

                                //这里无法避免装箱,反射调用索引器时同样也需要装箱
                                object indexValue = Convert.ChangeType(index, indexType);

                                Setter = (T value) =>
                                {
                                    indexSet.Invoke(instance, new object[] { indexValue, value });
                                };
                                ParseResult |= CreateDelegateResult.Set;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"TryCreateIndexerDelegate:  {e}");
            }

            return indexMatched;
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
        public static (CreateDelegateResult ParseResult, Func<T> Getter, Action<T> Setter)
            CreateDelegate<T>(this Type instanceType,
                              object instance,
                              string memberName,
                              bool instanceIsGetDelegate = false)
        {
            CreateDelegateResult ParseResult = CreateDelegateResult.None;
            Func<T> Getter = null;
            Action<T> Setter = null;

            if (memberName.Contains('['))
            {
                //为索引器创建委托
                if (instanceType.TryCreateIndexerDelegate(instance, memberName, out ParseResult, out Getter, out Setter, instanceIsGetDelegate))
                {
                    return (ParseResult, Getter, Setter);
                }
            }
            else
            {
                //属性 字段 方法  逐一尝试绑定。
                if (instanceType.TryCreatePropertyDelegate(instance, memberName, out ParseResult, out Getter, out Setter, instanceIsGetDelegate))
                {
                    return (ParseResult, Getter, Setter);
                }

                if (instanceType.TryCreateFieldDelegate(instance, memberName, out ParseResult, out Getter, out Setter, instanceIsGetDelegate))
                {
                    return (ParseResult, Getter, Setter);
                }

                if (instanceType.TryCreateMethodDelegate(instance, memberName, out ParseResult, out Getter, out Setter, instanceIsGetDelegate))
                {
                    return (ParseResult, Getter, Setter);
                }
            }

            Debug.LogWarning($"通过 {instanceType.FullName}类型 没有找到 符合标准的 成员 {memberName}。请确认成员是否被IL2CPP剪裁。");
            return (ParseResult, Getter, Setter);
        }
    }
}
