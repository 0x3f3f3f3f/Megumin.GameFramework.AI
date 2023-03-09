using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.Serialization
{
    public interface IFormatter<T>
    {
        T Serialize(object value);
        bool TryDeserialize(T source, out object value);
    }

    /// <summary>
    /// 用于用户自定义序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 目前框架只支持string和CustomParameterData两种类型。当类型时string时，可以fallback到json序列化。
    /// </remarks>
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
