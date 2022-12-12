﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.Binding
{
    [Serializable]
    public class BindableValue<T> : IData<T>, IBindable<T>, IUnityBindingParseable
    {
        public string Key;
        public bool IsBinding;
        public string BindingPath;
        public T defaultValue;
        public GameObject extnalObj;
        public int xOffset = 0, yOffset = 0;
        public bool IsStatic = false;
        public bool IsMatch = false;
        /// <summary>
        /// null表示还没有解析绑定
        /// </summary>
        protected BindResult? ParseResult = null;
        protected Func<T> Getter;
        protected Action<T> Setter;


        /// <summary>
        /// 没有调用<see cref="InitializeBinding(GameObject, bool)"/>时，映射到<see cref="defaultValue"/>。<para/> 
        /// 调用<see cref="InitializeBinding(GameObject, bool)"/>后，无论是否成功绑定，都不会在映射到映射到<see cref="defaultValue"/>。
        /// </summary>
        public T Value
        {
            get
            {
                if (ParseResult.HasValue)
                {
                    if (Getter == null)
                    {
                        Debug.LogWarning($"{BindingPath} cant Get");
                        return default;
                    }
                    else
                    {
                        return Getter();
                    }
                }
                else
                {
                    return defaultValue;
                }
            }
            set
            {
                if (ParseResult.HasValue)
                {
                    if (Setter == null)
                    {
                        Debug.LogWarning($"{BindingPath} cant Set");
                    }
                    else
                    {
                        Setter(value);
                    }
                }
                else
                {
                    defaultValue = value;
                }
            }
        }

        public T DefaultValue { get => defaultValue; set => defaultValue = value; }

        public void InitializeBinding(GameObject gameObject, bool force = false)
        {
            if (ParseResult == null || force)
            {
                (ParseResult, Getter, Setter) = BindingParser.Instance.InitializeBinding<T>(BindingPath, gameObject, extnalObj);
            }
        }

        public void DebugParseResult()
        {
            Debug.Log($"ParseResult:{ParseResult}    {Value}");
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
