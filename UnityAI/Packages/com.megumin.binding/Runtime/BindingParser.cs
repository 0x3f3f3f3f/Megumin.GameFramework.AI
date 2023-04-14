using System;

namespace Megumin.Binding
{


    public class BindingParser
    {
        public static BindingParser Instance { get; set; }

        public virtual (ParseBindingResult ParseResult, Func<T> Getter, Action<T> Setter)
            ParseBinding<T>(string bindingString, object bindingInstance, object options = null)
        {
            return default;
        }

        public virtual bool TryCreateMethodDelegate(string bindingPath,
                                            object bindingInstance,
                                            Type delegateType,
                                            out Delegate methodDelegate)
        {
            methodDelegate = null;
            return false;
        }

        public bool TryCreateMethodDelegate<T>(string bindingPath, object bindingInstance, out T methodDelegate)
            where T : Delegate
        {
            if (TryCreateMethodDelegate(bindingPath, bindingInstance, typeof(T), out var temp)
                && temp is T tValue)
            {
                methodDelegate = tValue;
                return true;
            }

            methodDelegate = default;
            return false;
        }
    }
}
