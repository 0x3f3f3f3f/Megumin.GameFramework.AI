using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Serialization;

namespace Megumin.Serialization
{
    public class StringFormatter
    {
        protected static readonly Lazy<Dictionary<string, IFormatter<string>>> lut = new(InitFormaters);

        public static Dictionary<string, IFormatter<string>> Lut => lut.Value;

        public static bool TryGet(Type type, out IFormatter<string> formatter)
        {
            formatter = default;
            if (type == null)
            {
                return false;
            }

            return Lut.TryGetValue(type.FullName, out formatter);
        }

        public static bool TryGet(string type, out IFormatter<string> formatter)
        {
            formatter = default;
            if (type == null)
            {
                return false;
            }

            return Lut.TryGetValue(type, out formatter);
        }

        public static string Serialize(object instance)
        {
            if (TryGet(instance?.GetType(), out var formatter))
            {
                return formatter.Serialize(instance);
            }
            return null;
        }

        public static bool TryDeserialize(string typeFullName, string source, out object value)
        {
            if (TryGet(typeFullName, out var formatter))
            {
                return formatter.TryDeserialize(source, out value);
            }

            value = default;
            return false;
        }

        static Dictionary<string, IFormatter<string>> InitFormaters()
        {
            var fs = new Dictionary<string, IFormatter<string>>()
            {
                { typeof(int).FullName,new IntFormater() },
                { typeof(float).FullName,new FloatFormater() },
                { typeof(string).FullName,new StringFormater() },
            };
            return fs;
        }
    }

    public sealed class IntFormater : IFormatter<string>
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

    public sealed class ShortFormater : IFormatter<string>
    {
        public string Serialize(object value)
        {
            return value.ToString();
        }

        public bool TryDeserialize(string source, out object value)
        {
            if (short.TryParse(source, out var result))
            {
                value = result;
                return true;
            }
            value = default;
            return false;
        }
    }

    public sealed class LongFormater : IFormatter<string>
    {
        public string Serialize(object value)
        {
            return value.ToString();
        }

        public bool TryDeserialize(string source, out object value)
        {
            if (long.TryParse(source, out var result))
            {
                value = result;
                return true;
            }
            value = default;
            return false;
        }
    }

    public class FloatFormater : IFormatter<string>
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

    public class DoubleFormater : IFormatter<string>
    {
        public string Serialize(object value)
        {
            return value.ToString();
        }

        public bool TryDeserialize(string source, out object value)
        {
            if (double.TryParse(source, out var result))
            {
                value = result;
                return true;
            }
            value = default;
            return false;
        }
    }

    public class StringFormater : IFormatter<string>
    {
        public string Serialize(object value)
        {
            return value.ToString();
        }

        public bool TryDeserialize(string source, out object value)
        {
            value = source;
            return true;
        }
    }
}
