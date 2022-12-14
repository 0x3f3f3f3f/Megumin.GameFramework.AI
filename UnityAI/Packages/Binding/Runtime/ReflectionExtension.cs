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
            if (propertyInfo.PropertyType != typeof(T))
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
                if (mygetter is Func<T> mygetterGeneric)
                {
                    getter = mygetterGeneric;
                    return true;
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
