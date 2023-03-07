using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Serialization;

namespace Megumin.GameFramework.AI.Serialization
{
    public interface Iformater
    {
        string Serialize(object value);
        bool TreDes(string source, out object value);
    }

    internal class Formater
    {
        protected static readonly Lazy<Dictionary<string, Iformater>> fsdic = new(InitFormaters);

        public static Dictionary<string, Iformater> Formaters => fsdic.Value;

        public static bool TryGet(Type type, out Iformater iformater)
        {
            iformater = default;
            if (type == null)
            {
                return false;
            }

            return Formaters.TryGetValue(type.FullName, out iformater);
        }

        public static bool TryGet(string type, out Iformater iformater)
        {
            iformater = default;
            if (type == null)
            {
                return false;
            }

            return Formaters.TryGetValue(type, out iformater);
        }

        static Dictionary<string, Iformater> InitFormaters()
        {
            var fs = new Dictionary<string, Iformater>()
            {
                { typeof(int).FullName,new IntF() },
                { typeof(float).FullName,new FntF() },
            };
            return fs;
        }
    }

    public class IntF : Iformater
    {
        public string Serialize(object value)
        {
            return value.ToString();
        }

        public bool TreDes(string source, out object value)
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

    public class FntF : Iformater
    {
        public string Serialize(object value)
        {
            return value.ToString();
        }

        public bool TreDes(string source, out object value)
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
