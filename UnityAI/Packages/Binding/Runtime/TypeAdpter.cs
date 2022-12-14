using System;
using System.Collections.Generic;

namespace Megumin.Binding
{
    public interface ITypeAdpter<T>
    {
        bool TryGetGetDeletgate(Delegate get, out Func<T> getter);
        bool TryGetSetDelegate(Delegate get, out Action<T> setter);
    }

    public interface IsetAD<T>
    {

    }

    public interface iconver<F, T> : ITypeAdpter<T>
    {
        T Convert(F value);
    }

    /// <summary>
    /// 
    /// </summary>
    public static class TypeAdpter
    {

        /// <summary>
        /// 与DelegateConnector不同，类型适配器时运行其他用户扩展的，每个泛型都要手动实现，不能反射构造<see cref="DelegateConnector.GetCreater(Type, Type)"/>
        /// </summary>
        static Dictionary<(Type, Type), object> adps = new Dictionary<(Type, Type), object>();

        //TODO,基类型自动适配。
        public static ITypeAdpter<T> GetTypeAdpter<T>(Type type)
        {
            var key = (type, typeof(T));
            if (adps.TryGetValue(key, out var adp))
            {
                if (adp is ITypeAdpter<T> gadp)
                {
                    return gadp;
                }
            }
            return null;
        }

        //TODO,基类型自动适配。
        public static TypeAdpter<F, T> GetTypeAdpter<F, T>()
        {
            var key = (typeof(F), typeof(T));
            if (adps.TryGetValue(key, out var adp))
            {
                if (adp is TypeAdpter<F, T> gadp)
                {
                    return gadp;
                }
            }
            return null;
        }


    }


    public abstract class TypeAdpter<F, T> : iconver<F, T>
    {
        public abstract T Convert(F value);

        public bool TryGetGetDeletgate(Delegate get, out Func<T> getter)
        {
            if (get is Func<F> fget)
            {
                getter = () =>
                {
                    return Convert(fget());
                };
                return true;
            }

            getter = null;
            return false;
        }

        public bool TryGetSetDelegate(Delegate get, out Action<T> setter)
        {
            if (get is Action<F> gset)
            {
                var ad = TypeAdpter.GetTypeAdpter<T, F>();
                if (ad != null)
                {
                    setter = (T value) =>
                    {
                        gset(ad.Convert(value));
                    };
                    return true;
                }
            }
            setter = null;
            return false;
        }
    }

    public class TypeAdpterOS : TypeAdpter<object, string> 
    {
        public override string Convert(object value)
        {
            return value?.ToString();
        }
    }
}
