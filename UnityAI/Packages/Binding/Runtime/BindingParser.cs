using System;

namespace Megumin.Binding
{
    

    public class BindingParser
    {
        public static BindingParser Instance { get; set; }

        public virtual (ParseBindingResult ParseResult, Func<T> Getter, Action<T> Setter)
            InitializeBinding<T>(string BindingString, object agent, object extnalObj)
        {
            return default;
        }


    }
}
