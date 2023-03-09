using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
            //https://learn.microsoft.com/zh-cn/dotnet/api/system.type.isprimitive?view=net-8.0
            var fs = new Dictionary<string, IFormatter<string>>()
            {
                { typeof(bool).FullName,new UnityJsonFormatter<bool>() },
                { typeof(byte).FullName,new UnityJsonFormatter<byte>() },
                { typeof(sbyte).FullName,new UnityJsonFormatter<sbyte>() },
                { typeof(char).FullName,new UnityJsonFormatter<char>() },

                { typeof(short).FullName,new UnityJsonFormatter<short>() },
                { typeof(ushort).FullName,new UnityJsonFormatter<ushort>() },

                { typeof(int).FullName,new UnityJsonFormatter<int>() },
                { typeof(uint).FullName,new UnityJsonFormatter<uint>() },

                { typeof(long).FullName,new UnityJsonFormatter<long>() },
                { typeof(ulong).FullName,new UnityJsonFormatter<ulong>() },

                { typeof(float).FullName,new UnityJsonFormatter<float>() },
                { typeof(double).FullName,new UnityJsonFormatter<double>() },

                { typeof(string).FullName,new UnityJsonFormatter<string>() },

                { typeof(Vector2).FullName,new UnityJsonFormatter<Vector2>() },
                { typeof(Vector2Int).FullName,new UnityJsonFormatter<Vector2Int>() },

                { typeof(Vector3).FullName,new UnityJsonFormatter<Vector3>() },
                { typeof(Vector3Int).FullName,new UnityJsonFormatter<Vector3Int>() },
                { typeof(Vector4).FullName,new UnityJsonFormatter<Vector4>() },

                { typeof(Quaternion).FullName,new UnityJsonFormatter<Quaternion>() },

                { typeof(DateTime).FullName,new UnityJsonFormatter<DateTime>() },
                { typeof(DateTimeOffset).FullName,new UnityJsonFormatter<DateTimeOffset>() },
                { typeof(TimeSpan).FullName,new UnityJsonFormatter<TimeSpan>() },



            };
            return fs;
        }

        public class UnityJsonFormatter<T> : IFormatter<string>
        {
            public string Serialize(object value)
            {
                return UnityEngine.JsonUtility.ToJson(value);
            }

            public bool TryDeserialize(string source, out object value)
            {
                try
                {
                    value = UnityEngine.JsonUtility.FromJson<T>(source);
                    return true ;
                }
                catch (Exception)
                {
                    value = default;
                }

                return false;
            }
        }

        internal protected sealed class IntFormater : IFormatter<string>
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

        internal protected sealed class ShortFormater : IFormatter<string>
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

        internal protected sealed class LongFormater : IFormatter<string>
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

        internal protected sealed class FloatFormater : IFormatter<string>
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

        internal protected sealed class DoubleFormater : IFormatter<string>
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

        internal protected sealed class StringFormatter2 : IFormatter<string>
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
}
