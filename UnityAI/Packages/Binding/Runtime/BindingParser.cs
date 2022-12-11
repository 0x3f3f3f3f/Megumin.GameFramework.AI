using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Search;
using UnityEngine;

namespace Megumin.Binding
{
    [Flags]
    public enum BindResult
    {
        None = 0,
        Get = 1,
        Set = 2,
        Both = Get | Set,
    }

    public class BindingParser
    {
        public static BindingParser Instance { get; set; }

        public virtual (BindResult ParseResult, Func<T> Getter, Action<T> Setter)
            InitializeBinding<T>(string BindingString, object agent, object extnalObj)
        {
            return default;
        }


    }
}
