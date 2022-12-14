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
    internal static class ReflectionExtension
    {
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
                            var connector = DelegateConnector.GetCreater(instanceType, methodInfo.ReturnType);
                            getter = connector.Connect(getInstance, methodInfo);
                            if (getter != null)
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
