using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI
{
    [Serializable]
    public class VariableTable
    {

#if UNITY_2023_1_OR_NEWER
        [UnityEngine.SerializeReference]
#endif
        public List<IVariable> Table = new();

        public bool TryGetParam(string name, out IVariable variable)
        {
            variable = null;
            return false;
        }

        public bool TryGetParam<T>(string name, out IVariable<T> variable)
        {
            variable = null;
            return false;
        }

        public bool TrySetValue<T>(string name, T value)
        {
            if (TryGetParam<T>(name, out var variable))
            {
                variable.Value = value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 同名验证时不区分大小写，但是获取值和设定值时区分大小写。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string ValidName(string name)
        {
            while (Table.Any(elem => string.Equals(elem.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                name += " (1)";
            }
            return name;
        }
    }
}
