using System;
using System.Reflection;

namespace Megumin.Binding
{
    /// <summary>
    /// 用于连接获取实例委托和成员取值委托。将2个委托组个成一个强类型Func<T>委托。
    /// </summary>
    public abstract class DelegateConnector
    {
        public static DelegateConnector GetCreater(Type instanceType, Type valueType)
        {
            var type = typeof(DelegateConnectorGeneric<,>).MakeGenericType(instanceType, valueType);
            //TODO : 要不要缓存实例，查表和反射创建那个效率更高？
            //如果直接制作Connect方法的泛型，则需要反射调用。制造实例通过实例方法调用，增加了调用链。
            return (DelegateConnector)Activator.CreateInstance(type);
        }

        //public abstract Delegate Connect(Delegate getinstane, Delegate getter);
        public abstract Delegate Connect(Delegate getinstane, MethodInfo methodInfo);

        class DelegateConnectorGeneric<I, T> : DelegateConnector
        {
            //public static Func<T> Create(Delegate getinstane, Delegate getter)
            //{
            //    if (getinstane is Func<I> gf)
            //    {
            //        if (getter is Func<I, T> gv)
            //        {
            //            Func<T> func = () =>
            //            {
            //                var instance = gf();
            //                return gv(instance);
            //            };
            //        }
            //    }
            //    return null;
            //}

            public static Func<T> Create(Delegate getinstane, MethodInfo methodInfo)
            {
                Type getterDelegateType = typeof(Func<,>).MakeGenericType(typeof(I), methodInfo.ReturnType);

                //string message = $"MakeG {getterDelegateType} , {typeof(Func<Transform, string>)}";
                //Debug.Log(message);

                var getDeletgate = methodInfo.CreateDelegate(getterDelegateType);
                if (getinstane is Func<I> gg)
                {
                    if (getDeletgate is Func<I, T> gv)
                    {
                        Func<T> func = () =>
                        {
                            var instance = gg();
                            return gv(instance);
                        };
                    }
                }
                return null;
            }

            //public override Delegate Connect(Delegate getinstane, Delegate getter)
            //{
            //    return Create(getinstane, getter);
            //}

            public override Delegate Connect(Delegate getinstane, MethodInfo methodInfo)
            {
                return Create(getinstane, methodInfo);
            }
        }
    }
}
