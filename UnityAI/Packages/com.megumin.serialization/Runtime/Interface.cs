using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.Serialization
{
    /// <summary>
    /// <para/>用户回调串行器。通常由含有无参构造函数的类型基础。
    /// <para/>遇到实例需要序列化到 T 类型时，首先尝试调用实例自身回调，代替框架默认通用序列化。
    /// <para/>通常用于框架默认序列化无法支持的类型，或者复杂泛型。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICallbackFormatter<in T>
    {
        bool TrySerialize(T destination);
        bool TryDeserialize(T source);
    }

    /// <summary>
    /// 串行器，用与将对象 序列化反序列化/转换 到 T 类型。
    /// 框架默认通用序列化根据具体类型使用的串行器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFormatter<T>
    {
        [Obsolete("", true)]
        T Serialize(object value);
        bool TrySerialize(object value, out T destination);
        bool TryDeserialize(T source, out object value);

        public bool TrySerialize<V>(V value, out string destination)
        {
            if (this is IFormatter<T, V> gFormator)
            {
                return gFormator.TrySerialize(value, out destination);
            }

            return TrySerialize(value, out destination);
        }

        public bool TryDeserialize<V>(T source, out V value)
        {
            if (this is IFormatter<T, V> gFormator)
            {
                return gFormator.TryDeserialize(source, out value);
            }

            if (TryDeserialize(source, out var objV) && objV is V gValue)
            {
                value = gValue;
                return true;
            }

            value = default;
            return false;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// 避免装箱
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public interface IFormatter<T, V> : IFormatter<T>
    {
        bool TrySerialize(V value, out T destination);
        bool TryDeserialize(T source, out V value);
    }

    /// <summary>
    /// 用于用户自定义序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISerializationCallbackReceiver<T>
    {
        /// <summary>
        /// 在框架通用反射序列化之前被调用，某些框架没有支持的类型在此时由用户自行序列化
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="ignoreMemberOnSerialize">后续的序列化过程将忽略这个成员</param>
        void OnBeforeSerialize(List<T> destination, List<string> ignoreMemberOnSerialize);
        /// <summary>
        /// 在框架通用反射序列化之后被调用，用户自行将数据反序列化为特定类型
        /// </summary>
        /// <param name="source"></param>
        void OnAfterDeserialize(List<T> source);
    }

    /// <summary>
    /// 引用查找器
    /// </summary>
    public interface IRefFinder
    {
        bool TryGetRefValue(string refName, out object refValue);
    }

    public static class Extension_E8CF5A3913CC4D2984636F23EAFC8A33
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetRefValue<T>(this IRefFinder refFinder, string refName, out T refValue)
        {
            if (refFinder.TryGetRefValue(refName, out var objValue) && objValue is T tValue)
            {
                refValue = tValue;
                return true;
            }

            refValue = default;
            return false;
        }
    }
}
