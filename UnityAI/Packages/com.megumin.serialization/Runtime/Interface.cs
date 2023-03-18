﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        T Serialize(object value);
        bool TryDeserialize(T source, out object value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// 避免装箱
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public interface IFormatter<T, V> : IFormatter<T>
    {
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
}
