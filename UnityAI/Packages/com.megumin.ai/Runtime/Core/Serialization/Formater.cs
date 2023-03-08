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
