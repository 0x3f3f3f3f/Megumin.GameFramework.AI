using System;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.Reflection
{
    public interface ITypeAdpterGet<T>
    {
        bool TryCreateGetter(Delegate get, out Func<T> getter);
    }

    public interface ITypeAdpterSet<T>
    {
        bool TryCreateSetter(Delegate get, out Action<T> setter);
    }

    public interface IConvertTypealbe<From, To> :
        ITypeAdpterGet<To>,
        ITypeAdpterSet<From>,
        ITypeAdpterDefineType
    {
        To Convert(From value);
    }

    /// <summary>
    /// 用于查看转换类型
    /// </summary>
    public interface ITypeAdpterDefineType
    {
        Type From { get; }
        Type To { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class TypeAdpter
    {
        /// <summary>
        /// 与DelegateConnector不同，类型适配器时运行其他用户扩展的，每个泛型都要手动实现，不能反射构造<see cref="DelegateConnector.Get(Type, Type)"/>
        /// </summary>
        public readonly static Dictionary<(Type, Type), object> Adpters = new()
        {
            { (typeof(object),typeof(string)) , new TypeAdpter_anyType_to_string<object>() },

            { (typeof(bool),typeof(string)) , new TypeAdpter_anyType_to_string<bool>() },
            { (typeof(char),typeof(string)) , new TypeAdpter_anyType_to_string<char>() },
            { (typeof(byte),typeof(string)) , new TypeAdpter_anyType_to_string<byte>() },
            { (typeof(short),typeof(string)) , new TypeAdpter_anyType_to_string<short>() },
            { (typeof(int),typeof(string)) , new TypeAdpter_anyType_to_string<int>() },
            { (typeof(long),typeof(string)) , new TypeAdpter_anyType_to_string<long>() },
            { (typeof(float),typeof(string)) , new TypeAdpter_anyType_to_string<float>() },
            { (typeof(double),typeof(string)) , new TypeAdpter_anyType_to_string<double>() },
            { (typeof(decimal),typeof(string)) , new TypeAdpter_anyType_to_string<decimal>() },

            { (typeof(sbyte),typeof(string)) , new TypeAdpter_anyType_to_string<sbyte>() },
            { (typeof(ushort),typeof(string)) , new TypeAdpter_anyType_to_string<ushort>() },
            { (typeof(uint),typeof(string)) , new TypeAdpter_anyType_to_string<uint>() },
            { (typeof(ulong),typeof(string)) , new TypeAdpter_anyType_to_string<ulong>() },

            { (typeof(DateTime),typeof(string)) , new TypeAdpter_anyType_to_string<DateTime>() },
            { (typeof(DateTimeOffset),typeof(string)) , new TypeAdpter_anyType_to_string<DateTimeOffset>() },

            { (typeof(bool),typeof(int)) , new TypeAdpter_bool_to_int() },
            { (typeof(int),typeof(float)) , new TypeAdpter_int_to_float() },
            { (typeof(int),typeof(double)) , new TypeAdpter_int_to_double() },
            { (typeof(float),typeof(double)) , new TypeAdpter_float_to_double() },
        };

        /// <summary>
        /// 没有明确指定，通过协变记录的适配器
        /// </summary>
        public readonly static Dictionary<(Type, Type), object> AdptersMapped = new();

        public static bool TryFindAdpter(Type from, Type to, out object adpter)
        {
            var key = (from, to);
            if (Adpters.ContainsKey(key))
            {
                return Adpters.TryGetValue(key, out adpter);
            }

            if (AdptersMapped.TryGetValue(key, out adpter))
            {
                return true;
            }
            else
            {
                //Test： Gameobjet -> string 使用 object -> string

                //查找基类型
                if (from.BaseType != null)
                {
                    TryFindAdpter(from.BaseType, to, out adpter);
                }

                if (adpter == null)
                {
                    //查找接口
                    var interfaces = from.GetInterfaces();
                    foreach (var @interface in interfaces)
                    {
                        if (TryFindAdpter(@interface, to, out adpter))
                        {
                            if (adpter != null)
                            {
                                break;
                            }
                        }
                    }
                }

                //要不要搜索To类型逆变？可能导致耗时过长

                //搜索完父类后无论是否找到结果都为true，即使是null，这是为了null时记录搜索结果。
                //不要判断是否是null，即使是null也要记录，防止后续二次搜索。
                AdptersMapped.Add((from, to), adpter);
                return true;
            }
        }


        public static IConvertTypealbe<F, T> FindAdpter<F, T>()
        {
            if (TryFindAdpter(typeof(F), typeof(T), out var adp))
            {
                if (adp is IConvertTypealbe<F, T> gadp)
                {
                    return gadp;
                }
                else
                {
                    if (adp != null)
                    {
                        Debug.LogError($"{adp}");
                    }
                }
            }
            return null;
        }

        public static ITypeAdpterGet<T> FindGetAdpter<T>(Type type)
        {
            if (TryFindAdpter(type, typeof(T), out var adp))
            {
                if (adp is ITypeAdpterGet<T> gadp)
                {
                    return gadp;
                }
                else
                {
                    if (adp != null)
                    {
                        Debug.LogError($"{adp}");
                    }
                }
            }
            return null;
        }

        public static ITypeAdpterSet<T> FindSetAdpter<T>(Type type)
        {
            if (TryFindAdpter(typeof(T), type, out var adp))
            {
                if (adp is ITypeAdpterSet<T> gadp)
                {
                    return gadp;
                }
                else
                {
                    if (adp != null)
                    {
                        Debug.LogError($"{adp}");
                    }
                }
            }
            return null;
        }
    }

    public abstract class TypeAdpter<F, T> : IConvertTypealbe<F, T>
    {
        public Type From => typeof(F);
        public Type To => typeof(T);

        public abstract T Convert(F value);

        public bool TryCreateGetter(Delegate get, out Func<T> getter)
        {
            if (get is Func<T> same)
            {
                getter = same;
                return true;
            }

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
            if (set is Action<F> same)
            {
                setter = same;
                return true;
            }

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


    /// <summary>
    /// 防止傻瓜
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public class TypeAdpterSameType<S> : TypeAdpter<S, S>
    {
        public static readonly TypeAdpterSameType<S> Instance = new TypeAdpterSameType<S>();
        public override S Convert(S value)
        {
            return value;
        }
    }

    public class TypeAdpter_SystemConvert<F, T> : TypeAdpter<F, T>
    {
        public override T Convert(F value)
        {
            unsafe
            {
                return (T)System.Convert.ChangeType(value, typeof(T));
            }
        }
    }

    public class TypeAdpter_anyType_to_string<F> : TypeAdpter<F, string>
    {
        public override string Convert(F value)
        {
            return value.ToString();
        }
    }

    public class TypeAdpter_bool_to_int : TypeAdpter<bool, int>
    {
        public override int Convert(bool value)
        {
            return value ? 1 : 0;
        }
    }

    public class TypeAdpter_int_to_float : TypeAdpter<int, float>
    {
        public override float Convert(int value)
        {
            return value;
        }
    }

    public class TypeAdpter_int_to_double : TypeAdpter<int, double>
    {
        public override double Convert(int value)
        {
            return value;
        }
    }

    public class TypeAdpter_float_to_double : TypeAdpter<float, double>
    {
        public override double Convert(float value)
        {
            return value;
        }
    }
}
