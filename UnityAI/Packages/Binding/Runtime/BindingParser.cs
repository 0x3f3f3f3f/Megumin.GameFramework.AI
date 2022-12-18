using System;

namespace Megumin.Binding
{
    

    public class BindingParser
    {
        public static BindingParser Instance { get; set; }

        public virtual (ParseBindingResult ParseResult, Func<T> Getter, Action<T> Setter)
            ParseBinding<T>(string bindingString, object bindingInstance)
        {
            return default;
        }


    }
}
