using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Linq;
using static Megumin.Binding.BindableIntValue;

namespace Megumin.Binding
{
    public interface IData
    {

    }

    public interface IData<T> : IData
    {
        T Value { get; set; }
    }

    /// <summary>
    /// 运行时值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Bindable<T> : IData<T>
    {
        public T Value { get; set; }

        protected Func<T> Getter;
        protected Action<T> Setter;


        public static implicit operator T(Bindable<T> value)
        {
            return value.Value;
        }
    }

    /// <summary>
    /// 绑定信息序列化
    /// </summary>
    [Serializable]
    public class SerializeValue
    {
        public string Key;
        public bool IsBinding;
        public string BindingString;
        public string Value;


    }

    [Serializable]
    public class TestGAB<T> : IData<T>
    {
        public T mValue;
        public T Value { get; set; }
    }

    [Serializable]
    public class TestG<T> : TestGAB<T>
    {
        public T value2;
    }

    [Serializable]
    public class BindableValue<T> : IData<T>, IData
    {
        public string Key;
        public bool IsBinding;
        public string BindingString;
        public T DefaultValue;
        public GameObject extnalObj;
        public int xOffset = 0, yOffset = 0;
        protected BindResult ParseResult = BindResult.None;
        protected Func<T> Getter;
        protected Action<T> Setter;
        public T Value
        {
            get { return (Getter != null ? Getter() : DefaultValue); }
            set
            {
                if (Setter != null)
                {
                    Setter(value);
                }
                else
                {
                    DefaultValue = value;
                }
            }
        }

        public void InitializeBinding(GameObject gameObject)
        {
            UnityBindingParse parse = new UnityBindingParse();
            (ParseResult, Getter, Setter) = parse.InitializeBinding<T>(BindingString, gameObject, extnalObj);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BindableIntValue : BindableValue<int>
    {

    }
}
