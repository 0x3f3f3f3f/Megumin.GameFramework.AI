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
                { typeof(bool).FullName,new BoolFormatter() },
                { typeof(byte).FullName,new ByteFormatter() },
                { typeof(sbyte).FullName,new SByteFormatter() },
                { typeof(char).FullName,new CharFormatter() },

                { typeof(short).FullName,new ShortFormatter() },
                { typeof(ushort).FullName,new UShortFormatter() },

                { typeof(int).FullName,new IntFormatter() },
                { typeof(uint).FullName,new UIntFormatter() },

                { typeof(long).FullName,new LongFormatter() },
                { typeof(ulong).FullName,new ULongFormatter() },

                { typeof(float).FullName,new FloatFormatter() },
                { typeof(double).FullName,new DoubleFormatter() },

                { typeof(string).FullName,new StringFormatter2() },

                { typeof(Vector2).FullName,new UnityJsonFormatter<Vector2>() },
                { typeof(Vector2Int).FullName,new UnityJsonFormatter<Vector2Int>() },

                { typeof(Vector3).FullName,new UnityJsonFormatter<Vector3>() },
                { typeof(Vector3Int).FullName,new UnityJsonFormatter<Vector3Int>() },
                { typeof(Vector4).FullName,new UnityJsonFormatter<Vector4>() },

                { typeof(Quaternion).FullName,new UnityJsonFormatter<Quaternion>() },

                { typeof(DateTime).FullName,new DataTimeFormatter() },
                { typeof(DateTimeOffset).FullName,new DateTimeOffsetFormatter() },
                { typeof(TimeSpan).FullName,new TimeSpanFormatter() },
                { typeof(Type).FullName,new TypeFormatter() },



            };
            return fs;
        }

        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/JsonUtility.ToJson.html
        /// 基元类型不能使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
                    return true;
                }
                catch (Exception)
                {
                    value = default;
                }

                return false;
            }
        }

        public sealed class BoolFormatter : IFormatter<string>
        {
            public string Serialize(object value)
            {
                return value.ToString();
            }

            public bool TryDeserialize(string source, out object value)
            {
                if (bool.TryParse(source, out var result))
                {
                    value = result;
                    return true;
                }
                value = default;
                return false;
            }
        }

        public sealed class ByteFormatter : IFormatter<string>
        {
            public string Serialize(object value)
            {
                return value.ToString();
            }

            public bool TryDeserialize(string source, out object value)
            {
                if (byte.TryParse(source, out var result))
                {
                    value = result;
                    return true;
                }
                value = default;
                return false;
            }
        }

        public sealed class SByteFormatter : IFormatter<string>
        {
            public string Serialize(object value)
            {
                return value.ToString();
            }

            public bool TryDeserialize(string source, out object value)
            {
                if (sbyte.TryParse(source, out var result))
                {
                    value = result;
                    return true;
                }
                value = default;
                return false;
            }
        }

        public sealed class CharFormatter : IFormatter<string>
        {
            public string Serialize(object value)
            {
                return value.ToString();
            }

            public bool TryDeserialize(string source, out object value)
            {
                if (char.TryParse(source, out var result))
                {
                    value = result;
                    return true;
                }
                value = default;
                return false;
            }
        }

        public sealed class ShortFormatter : IFormatter<string>
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

        public sealed class UShortFormatter : IFormatter<string>
        {
            public string Serialize(object value)
            {
                return value.ToString();
            }

            public bool TryDeserialize(string source, out object value)
            {
                if (ushort.TryParse(source, out var result))
                {
                    value = result;
                    return true;
                }
                value = default;
                return false;
            }
        }

        public sealed class IntFormatter : IFormatter<string>
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

        public sealed class UIntFormatter : IFormatter<string>
        {
            public string Serialize(object value)
            {
                return value.ToString();
            }

            public bool TryDeserialize(string source, out object value)
            {
                if (uint.TryParse(source, out var result))
                {
                    value = result;
                    return true;
                }
                value = default;
                return false;
            }
        }

        public sealed class LongFormatter : IFormatter<string>
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

        public sealed class ULongFormatter : IFormatter<string>
        {
            public string Serialize(object value)
            {
                return value.ToString();
            }

            public bool TryDeserialize(string source, out object value)
            {
                if (ulong.TryParse(source, out var result))
                {
                    value = result;
                    return true;
                }
                value = default;
                return false;
            }
        }

        public sealed class FloatFormatter : IFormatter<string>
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

        public sealed class DoubleFormatter : IFormatter<string>
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

        public sealed class StringFormatter2 : IFormatter<string>
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

        public sealed class DataTimeFormatter : IFormatter<string>
        {
            public string Serialize(object value)
            {
                if (value is DateTime stamp)
                {
                    return stamp.ToFileTimeUtc().ToString();
                }
                return value.ToString();
            }

            public bool TryDeserialize(string source, out object value)
            {
                if (long.TryParse(source, out var result))
                {
                    value = DateTime.FromFileTimeUtc(result);
                    return true;
                }
                value = default;
                return false;
            }
        }

        public sealed class DateTimeOffsetFormatter : IFormatter<string>
        {
            public string Serialize(object value)
            {
                if (value is DateTimeOffset stamp)
                {
                    return stamp.ToFileTime().ToString();
                }
                return value.ToString();
            }

            public bool TryDeserialize(string source, out object value)
            {
                if (long.TryParse(source, out var result))
                {
                    value = DateTimeOffset.FromFileTime(result);
                    return true;
                }
                value = default;
                return false;
            }
        }

        public sealed class TimeSpanFormatter : IFormatter<string>
        {
            public string Serialize(object value)
            {
                if (value is TimeSpan span)
                {
                    return span.Ticks.ToString();
                }
                return value.ToString();
            }

            public bool TryDeserialize(string source, out object value)
            {
                if (long.TryParse(source, out var result))
                {
                    value = TimeSpan.FromTicks(result);
                    return true;
                }
                value = default;
                return false;
            }
        }

        public sealed class TypeFormatter : IFormatter<string>
        {
            public string Serialize(object value)
            {
                if (value is Type type)
                {
                    return type.FullName;
                }
                return value.ToString();
            }

            public bool TryDeserialize(string source, out object value)
            {
                if (TypeCache.TryGetType(source, out var type))
                {
                    value = type;
                    return true;
                }
                value = default;
                return false;
            }
        }
    }
}
