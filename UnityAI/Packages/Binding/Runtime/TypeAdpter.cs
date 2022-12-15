using System;
using System.Collections.Generic;

namespace Megumin.Binding
{
    public interface ITypeAdpterGet<T>
    {
        bool TryCreateGetter(Delegate get, out Func<T> getter);
    }

    public interface ITypeAdpterSet<T>
    {
        bool TryCreateSetter(Delegate get, out Action<T> setter);
    }

    public interface IConvertTypealbe<From, To> : ITypeAdpterGet<To>, ITypeAdpterSet<From>
    {
        To Convert(From value);
    }

    /// <summary>
    /// 
    /// </summary>
    public static class TypeAdpter
    {
        /// <summary>
        /// 与DelegateConnector不同，类型适配器时运行其他用户扩展的，每个泛型都要手动实现，不能反射构造<see cref="DelegateConnector.Get(Type, Type)"/>
        /// </summary>
        static Dictionary<(Type, Type), object> adps = new Dictionary<(Type, Type), object>()
        {
            { (typeof(int),typeof(string)) , new TypeAdpter_Int2String() },
            { (typeof(object),typeof(string)) , new TypeAdpter_Object2String() },
        };

        //TODO,基类型自动适配。
        public static ITypeAdpterGet<T> FindGetAdpter<T>(Type type)
        {
            var key = (type, typeof(T));
            if (adps.TryGetValue(key, out var adp))
            {
                if (adp is ITypeAdpterGet<T> gadp)
                {
                    return gadp;
                }
            }
            return null;
        }

        public static ITypeAdpterSet<T> FindSetAdpter<T>(Type type)
        {
            var key = (typeof(T), type);
            if (adps.TryGetValue(key, out var adp))
            {
                if (adp is ITypeAdpterSet<T> gadp)
                {
                    return gadp;
                }
            }
            return null;
        }

        //TODO,基类型自动适配。
        public static TypeAdpter<F, T> FindAdpter<F, T>()
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


    public abstract class TypeAdpter<F, T> : IConvertTypealbe<F, T>
    {
        public abstract T Convert(F value);

        public bool TryCreateGetter(Delegate get, out Func<T> getter)
        {
            if (get is Func<F> getGeneric)
            {
                getter = () =>
                {
                    return Convert(getGeneric());
                };
                return true;
            }

            getter = null;
            return false;
        }

        public bool TryCreateSetter(Delegate set, out Action<F> setter)
        {
            if (set is Action<T> setGeneric)
            {
                setter = (F value) =>
                {
                    setGeneric(Convert(value));
                };
                return true;
            }
            setter = null;
            return false;
        }
    }

    public class TypeAdpter_Object2String : TypeAdpter<object, string>
    {
        public override string Convert(object value)
        {
            return value?.ToString();
        }
    }

    public class TypeAdpter_Int2String : TypeAdpter<int, string>
    {
        public override string Convert(int value)
        {
            return value.ToString();
        }
    }
}
