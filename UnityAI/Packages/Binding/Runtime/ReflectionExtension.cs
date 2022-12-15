﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Search;
using UnityEngine;

namespace Megumin.Binding
{
    /// <summary>
    /// 重要： https://learn.microsoft.com/zh-cn/dotnet/csharp/programming-guide/concepts/covariance-contravariance/variance-in-delegates#variance-in-generic-type-parameters-for-value-and-reference-types
    /// </summary>
    internal static class ReflectionExtension_9C4E15F3B30F4FCFBC57EDC2A99A69D0
    {
        public static void TestConvert()
        {
            var b1 = typeof(string).IsAssignableFrom(typeof(object));   //false
            var b2 = typeof(object).IsAssignableFrom(typeof(string));   //true

            var b3 = typeof(int).IsAssignableFrom(typeof(object));   //false
            var b4 = typeof(object).IsAssignableFrom(typeof(int));   //true

            var b5 = typeof(float).IsAssignableFrom(typeof(int));   //false
            var b6 = typeof(int).IsAssignableFrom(typeof(float));   //false

            var b7 = typeof(float).IsAssignableFrom(typeof(double));   //false
            var b8 = typeof(double).IsAssignableFrom(typeof(float));   //false

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
        public static bool CanConvertDelegate(Type from, Type to)
        {
            var b1 = from.IsAssignableFrom(to);
            var b2 = to.IsAssignableFrom(from);

            if (from == to)
            {
                return true;
            }

            if (to.IsAssignableFrom(from))
            {
                if (from.IsValueType)
                {
                    //值类型通常都不能处理Func<T>协变，需要使用适配器转换
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
        public static bool CanConvertDelegate<T>(this Type from)
        {
            return CanConvertDelegate(from, typeof(T));
        }

        /// <summary>
        /// 使用类型适配器创建委托。
        /// </summary>
        /// <typeparam name="T">类型适配器类型</typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        public static bool TryGetGetDelegateUseTypeAdpter<T>(this PropertyInfo propertyInfo,
                                                Type instanceType,
                                                object instance,
                                                out Func<T> getter,
                                                bool instanceIsGetDelegate = false)
        {
            if (propertyInfo.CanRead)
            {
                if (propertyInfo.GetMethod.TryGetGetDelegateUseTypeAdpter(
                    instanceType, instance, out getter, instanceIsGetDelegate))
                {
                    return true;
                }
            }

            getter = null;
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"><see cref="PropertyInfo.PropertyType"/></typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        public static bool TryGetGetDelegate<T>(this PropertyInfo propertyInfo,
                                                Type instanceType,
                                                object instance,
                                                out Func<T> getter,
                                                bool instanceIsGetDelegate = false)
        {
            if (propertyInfo.CanRead)
            {
                if (TryGetGetDelegate(propertyInfo.GetMethod, instanceType, instance, out getter, instanceIsGetDelegate))
                {
                    return true;
                }
            }

            getter = null;
            return false;
        }

        public static bool TryGetGetDelegate(this PropertyInfo propertyInfo,
                                             Type instanceType,
                                             object instance,
                                             out Delegate getter,
                                             bool instanceIsGetDelegate = false)
        {
            if (propertyInfo.CanRead)
            {
                if (TryGetGetDelegate(propertyInfo.GetMethod, instanceType, instance, out getter, instanceIsGetDelegate))
                {
                    return true;
                }
            }

            getter = null;
            return false;
        }


        public static bool TryGetGetDelegateUseTypeAdpter<T>(this MethodInfo methodInfo,
                                                Type instanceType,
                                                object instance,
                                                out Func<T> getter,
                                                bool instanceIsGetDelegate = false)
        {
            if (methodInfo.ReturnType.CanConvertDelegate<T>() == false)
            {
                //自动类型适配
                var adp = TypeAdpter.GetTypeAdpter<T>(methodInfo.ReturnType);

                if (adp == null)
                {
                    Debug.LogError($"成员类型{methodInfo.ReturnType}无法满足目标类型{typeof(T)},并且没有找到对应的TypeAdpter");
                }
                else
                {
                    if (methodInfo.TryGetGetDelegate(instanceType, instance, out var g, instanceIsGetDelegate))
                    {
                        if (adp.TryGetGetDeletgate(g, out getter))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return methodInfo.TryGetGetDelegate(instanceType, instance, out getter, instanceIsGetDelegate);
            }

            getter = null;
            return false;
        }


        public static bool TryGetGetDelegate<T>(this MethodInfo methodInfo,
                                                Type instanceType,
                                                object instance,
                                                out Func<T> getter,
                                                bool instanceIsGetDelegate = false)
        {
            if (TryGetGetDelegate(methodInfo, instanceType, instance, out var mygetter, instanceIsGetDelegate))
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
        /// 将无参方法创建为 <![CDATA[Func<ReturnType>]]> 的强类型委托，并以Delegate类型返回。
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        public static bool TryGetGetDelegate(this MethodInfo methodInfo,
                                             Type instanceType,
                                             object instance,
                                             out Delegate getter,
                                             bool instanceIsGetDelegate = false)
        {
            getter = null;
            //var paras = methodInfo.GetParameters();
            Type delagateType = typeof(Func<>).MakeGenericType(methodInfo.ReturnType);
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
                            var connector = DelegateConnector.Get(instanceType, methodInfo.ReturnType);
                            if (connector.TryConnect(getInstance, methodInfo, out getter))
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


















        public static bool TryGetSetDelegateUseTypeAdpter<T>(this PropertyInfo propertyInfo,
                                                Type instanceType,
                                                object instance,
                                                out Action<T> setter,
                                                bool instanceIsGetDelegate = false)
        {
            if (propertyInfo.CanWrite)
            {
                if (propertyInfo.SetMethod.TryGetSetDelegateUseTypeAdpter(
                    instanceType, instance, out setter, instanceIsGetDelegate))
                {
                    return true;
                }
            }

            setter = null;
            return false;
        }

        public static bool TryGetSetDelegateUseTypeAdpter<T>(this MethodInfo methodInfo,
                                                Type instanceType,
                                                object instance,
                                                out Action<T> setter,

                                                bool instanceIsGetDelegate = false)
        {
            var paras = methodInfo.GetParameters();
            Type firstArgsType = paras[0].ParameterType;
            if (CanConvertDelegate(typeof(T), firstArgsType) == false)
            {
                //自动类型适配
                var adp = TypeAdpter.GetSetTypeAdpter<T>(firstArgsType);

                if (adp == null)
                {
                    Debug.LogError($"成员类型{firstArgsType}无法满足目标类型{typeof(T)},并且没有找到对应的TypeAdpter");
                }
                else
                {
                    if (methodInfo.TryGetSetDelegate(instanceType, instance, out var g, instanceIsGetDelegate))
                    {
                        if (adp.TryGetSetDelegate(g, out setter))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                return methodInfo.TryGetSetDelegate(instanceType, instance, out setter, instanceIsGetDelegate);
            }

            setter = null;
            return false;
        }

        public static bool TryGetSetDelegate<T>(this MethodInfo methodInfo,
                                                Type instanceType,
                                                object instance,
                                                out Action<T> setter,
                                                bool instanceIsGetDelegate = false)
        {
            if (TryGetSetDelegate(methodInfo, instanceType, instance, out var mysetter, instanceIsGetDelegate))
            {
                var typeP = methodInfo.ReturnType;
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
        /// 将接收一个目标参数的方法创建为 <![CDATA[Action<ReturnType>]]> 的强类型委托，并以Delegate类型返回。
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="instanceIsGetDelegate"></param>
        /// <returns></returns>
        public static bool TryGetSetDelegate(this MethodInfo methodInfo,
                                             Type instanceType,
                                             object instance,
                                             out Delegate setter,
                                             bool instanceIsGetDelegate = false)
        {
            setter = null;
            var paras = methodInfo.GetParameters();
            Type firstArgsType = paras[0].ParameterType;
            Type delagateType = typeof(Action<>).MakeGenericType(firstArgsType);
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
                    Debug.LogError("instanceDelegate is null");
                }
                else
                {
                    if (instanceIsGetDelegate)
                    {
                        if (instance is Delegate getInstance)
                        {
                            var connector = DelegateConnector.Get(instanceType, firstArgsType);
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
    }


}
