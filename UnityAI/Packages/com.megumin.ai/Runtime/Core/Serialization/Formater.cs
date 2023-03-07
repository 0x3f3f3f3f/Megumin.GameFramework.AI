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

        static Dictionary<string, Iformater> InitFormaters()
        {
            var fs = new Dictionary<string, Iformater>()
            {
                { typeof(int).FullName,new IntF() },
                { typeof(float).FullName,new IntF() },
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
    }
}
