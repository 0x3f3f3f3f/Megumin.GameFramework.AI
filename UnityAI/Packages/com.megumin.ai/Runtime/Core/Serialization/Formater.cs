using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Serialization;

namespace Megumin.GameFramework.AI.Serialization
{
    public interface IFormater2String
    {
        string Serialize(object value);
        bool TryDeserialize(string source, out object value);
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

    internal class Formater
    {
        protected static readonly Lazy<Dictionary<string, IFormater2String>> fsdic = new(InitFormaters);

        public static Dictionary<string, IFormater2String> Formaters => fsdic.Value;

        public static bool TryGet(Type type, out IFormater2String iformater)
        {
            iformater = default;
            if (type == null)
            {
                return false;
            }

            return Formaters.TryGetValue(type.FullName, out iformater);
        }

        public static bool TryGet(string type, out IFormater2String iformater)
        {
            iformater = default;
            if (type == null)
            {
                return false;
            }

            return Formaters.TryGetValue(type, out iformater);
        }

        static Dictionary<string, IFormater2String> InitFormaters()
        {
            var fs = new Dictionary<string, IFormater2String>()
            {
                { typeof(int).FullName,new IntFormater() },
                { typeof(float).FullName,new FloatFormater() },
            };
            return fs;
        }
    }

    public class IntFormater : IFormater2String
    {
        public string Serialize(object value)
        {
            return value.ToString();
        }

        public bool TryDeserialize(string source, out object value)
        {
            if (int.TryParse(source, out var result))
            {
                value = result;
                return true;
            }
            value = default;
            return false;
        }
    }

    public class FloatFormater : IFormater2String
    {
        public string Serialize(object value)
        {
            return value.ToString();
        }

        public bool TryDeserialize(string source, out object value)
        {
            if (float.TryParse(source, out var result))
            {
                value = result;
                return true;
            }
            value = default;
            return false;
        }
    }
}
