using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Search;
using UnityEngine;

namespace Megumin.Binding
{
    internal static class ReflectionExtension_9C4E15F3B30F4FCFBC57EDC2A99A69D0
    {
        public static void TestConvert()
        {
            var b1 = typeof(string).IsAssignableFrom(typeof(object));
            var b2 = typeof(object).IsAssignableFrom(typeof(string));

            var b3 = typeof(int).IsAssignableFrom(typeof(object));
            var b4 = typeof(object).IsAssignableFrom(typeof(int));

            var b5 = typeof(float).IsAssignableFrom(typeof(int));
            var b6 = typeof(int).IsAssignableFrom(typeof(float));

            var b7 = typeof(float).IsAssignableFrom(typeof(double));
            var b8 = typeof(double).IsAssignableFrom(typeof(float));

            int a = 200;
            float b = 300f;
            b = a;

            Func<string> funcstring = () => "";
            Func<int> funcint = () => 100;
            Func<float> funcfloat = () => 100f;

            Func<object> funcObj = funcstring;
            // Func<int> 不能协变成  Func<object> 也就认了，毕竟涉及到装箱。
            // Func<float> 协变成  Func<double>也不行? 无法理解
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
        public static bool CanAutoConvertFuncT(Type from, Type to)
        {
            if (to.IsAssignableFrom(from))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 测试是否需要类型适配器。
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static bool CanAutoConvertFuncT<T>(this Type myValueType)
        {
            return CanAutoConvertFuncT(myValueType, typeof(T));
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
            var typeP = propertyInfo.PropertyType;
            var typeV = typeof(T);
            var b1 = typeV.IsAssignableFrom(typeP);
            var b2 = typeP.IsAssignableFrom(typeV);

            if (propertyInfo.PropertyType.CanAutoConvertFuncT<T>() == false)
            {
                //自动类型适配
                var adp = TypeAdpter.GetTypeAdpter<T>(propertyInfo.PropertyType);

                if (adp != null)
                {
                    if (propertyInfo.TryGetGetDelegate(instanceType, instance, out var g, instanceIsGetDelegate))
                    {
                        if (adp.TryGetGetDeletgate(g, out getter))
                        {
                            return true;
                        }
                    }
                }
            }

            return TryGetGetDelegate(propertyInfo, instanceType, instance, out getter, instanceIsGetDelegate);
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

        public static bool TryGetGetDelegate(this MethodInfo methodInfo,
                                             Type instanceType,
                                             object instance,
                                             out Delegate getter,
                                             bool instanceIsGetDelegate = false)
        {
            getter = null;
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





    }


}
