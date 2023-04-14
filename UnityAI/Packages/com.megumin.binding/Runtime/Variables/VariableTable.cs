using System;
using System.Collections.Generic;
using System.Linq;

namespace Megumin.Binding
{
    [Serializable]
    public class VariableTable
    {
        //Paramter 部分
        //API参考 ainmator timeline
        //void Test()
        //{
        //    Animator animator = new Animator();
        //    PlayableDirector playable = new PlayableDirector();
        //}


#if UNITY_2023_1_OR_NEWER
        
#endif
        [UnityEngine.SerializeReference]
        public List<IRefable> Table = new();

        public bool TryGetParam(string name, out IRefable variable)
        {
            var first = Table.FirstOrDefault(elem => elem.RefName == name);
            if (first != null)
            {
                variable = first;
                return true;
            }

            variable = null;
            return false;
        }

        public bool TryGetParam<T>(string name, out IVariable<T> variable)
        {
            if (TryGetParam(name, out var temp) && temp is IVariable<T> valid)
            {
                variable = valid;
                return true;
            }

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
            while (Table.Any(elem => string.Equals(elem.RefName, name, StringComparison.OrdinalIgnoreCase)))
            {
                name += " (1)";
            }
            return name;
        }

        public void ParseBinding(object agent, bool force, object options = null)
        {
            foreach (var item in Table)
            {
                if (item is IBindingParseable parseable)
                {
                    parseable.ParseBinding(agent, force, options);
                }
            }
        }
    }
}
