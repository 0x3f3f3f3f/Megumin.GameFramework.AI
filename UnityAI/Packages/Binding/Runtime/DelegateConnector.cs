using System;
using System.Reflection;

namespace Megumin.Binding
{
    /// <summary>
    /// 用于连接获取实例委托和成员取值委托。将2个委托组个成一个强类型<see cref="Func{TResult}"/>委托。
    /// MakeGenericType 会不会因为剪裁不可使用。如果不适用MakeGenericType，object会导致装箱。
    /// </summary>
    public abstract class DelegateConnector
    {
        /// <summary>
        /// 实例充当中间调用过程。并不是使用实例对象本身。
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static DelegateConnector Get(Type instanceType, Type valueType)
        {
            var type = typeof(DelegateConnectorGeneric<,>).MakeGenericType(instanceType, valueType);
            //TODO : 要不要缓存实例，查表和反射创建那个效率更高？
            //如果直接制作Connect方法的泛型，则需要反射调用。制造实例通过实例方法调用，增加了调用链。
            return (DelegateConnector)Activator.CreateInstance(type);
        }

        //public abstract Delegate Connect(Delegate getinstane, Delegate getter);
        //public abstract Delegate Connect(Delegate getinstane, MethodInfo methodInfo);

        public virtual bool TryConnect(Delegate getinstane, MethodInfo methodInfo, out Delegate getter)
        {
            Type getterDelegateType = typeof(Func<,>).MakeGenericType(methodInfo.DeclaringType, methodInfo.ReturnType);

            //string message = $"MakeG {getterDelegateType} , {typeof(Func<Transform, string>)}";
            //Debug.Log(message);

            var getDeletgate = methodInfo.CreateDelegate(getterDelegateType);

            //TODO 使用强类型委托 避免 DynamicInvoke , 不知道在IL2CPP中会不会有问题，泛型方法无法生成？
            //Func<object, T> getDeletgate2 = getDeletgate as Func<object, T>;

            Func<object> mygetter = () =>
            {
                object instance = null;
                if (getinstane is Func<object> getinstaneGeneric)
                {
                    instance = getinstaneGeneric();
                }
                else
                {
                    instance = getinstane.DynamicInvoke();
                }
                return getDeletgate.DynamicInvoke(instance);
            };

            getter = mygetter;

            //var GETDEtYPE = typeof(Func<>).MakeGenericType(methodInfo.ReturnType);
            //getter 如何转成强类型？
            return true;
        }

        class DelegateConnectorGeneric<I, T> : DelegateConnector
        {
            //public static Func<T> Create(Delegate getinstane, Delegate getter)
            //{
            //    if (getinstane is Func<I> gf)
            //    {
            //        if (getter is Func<I, T> getDelegateGeneric)
            //        {
            //            Func<T> getter = () =>
            //            {
            //                var instance = gf();
            //                return getDelegateGeneric(instance);
            //            };
            //        }
            //    }
            //    return null;
            //}

            //public static Func<T> Create(Delegate getinstane, MethodInfo methodInfo)
            //{
            //    Type getterDelegateType = typeof(Func<,>).MakeGenericType(typeof(I), methodInfo.ReturnType);

            //    //string message = $"MakeG {getterDelegateType} , {typeof(Func<Transform, string>)}";
            //    //Debug.Log(message);

            //    var getDeletgate = methodInfo.CreateDelegate(getterDelegateType);
            //    if (getinstane is Func<I> getinstaneGeneric)
            //    {
            //        if (getDeletgate is Func<I, T> getDelegateGeneric)
            //        {
            //            Func<T> getter = () =>
            //            {
            //                var instance = getinstaneGeneric();
            //                return getDelegateGeneric(instance);
            //            };

            //            return getter;
            //        }
            //    }
            //    return null;
            //}

            public static bool TryCreate(Delegate getinstane, MethodInfo methodInfo, out Func<T> getter)
            {
                Type getterDelegateType = typeof(Func<,>).MakeGenericType(typeof(I), methodInfo.ReturnType);

                //string message = $"MakeG {getterDelegateType} , {typeof(Func<Transform, string>)}";
                //Debug.Log(message);

                var getDeletgate = methodInfo.CreateDelegate(getterDelegateType);

                //TODO 使用强类型委托 避免 DynamicInvoke , 不知道在IL2CPP中会不会有问题，泛型方法无法生成？
                //Func<object, T> getDeletgate2 = getDeletgate as Func<object, T>;

                if (getinstane is Func<I> getinstaneGeneric)
                {
                    if (getDeletgate is Func<I, T> getDelegateGeneric)
                    {
                        getter = () =>
                        {
                            var instance = getinstaneGeneric();
                            return getDelegateGeneric(instance);
                        };

                        return true;
                    }
                }

                getter = null;
                return false;
            }

            //public override Delegate Connect(Delegate getinstane, Delegate getter)
            //{
            //    return Create(getinstane, getter);
            //}

            //public override Delegate Connect(Delegate getinstane, MethodInfo methodInfo)
            //{
            //    return Create(getinstane, methodInfo);
            //}

            public override bool TryConnect(Delegate getinstane, MethodInfo methodInfo, out Delegate getter)
            {
                if (TryCreate(getinstane, methodInfo, out var mygetter))
                {
                    getter = mygetter;
                    return true;
                }

                getter = null;
                return false;
            }
        }
    }
}
