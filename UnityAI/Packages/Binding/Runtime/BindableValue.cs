using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.Binding
{
    [Serializable]
    public class BindableValue<T> : IData<T>, IBindable<T>
    {
        public string Key;
        public bool IsBinding;
        public string BindingPath;
        private T defaultValue;
        public GameObject extnalObj;
        public int xOffset = 0, yOffset = 0;
        public bool IsStatic = false;
        public bool IsMatch = false;
        protected BindResult ParseResult = BindResult.None;
        protected Func<T> Getter;
        protected Action<T> Setter;



        public T Value
        {
            get { return (Getter != null ? Getter() : defaultValue); }
            set
            {
                if (Setter != null)
                {
                    Setter(value);
                }
                else
                {
                    defaultValue = value;
                }
            }
        }

        public T DefaultValue { get => defaultValue; set => defaultValue = value; }

        public void InitializeBinding(GameObject gameObject)
        {
            UnityBindingParse parse = new UnityBindingParse();
            (ParseResult, Getter, Setter) = parse.InitializeBinding<T>(BindingPath, gameObject, extnalObj);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BindableValueInt : BindableValue<int>
    {

    }
}
