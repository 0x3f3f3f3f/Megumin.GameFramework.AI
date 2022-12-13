using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.Binding
{
    [Serializable]
    public class BindableValue<T> : IData<T>, IBindable<T>, IBindingParseable
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
        protected ParseBindingResult? ParseResult = null;
        protected Func<T> Getter;
        protected Action<T> Setter;


        /// <summary>
        /// 没有调用<see cref="ParseBinding(GameObject, bool)"/>时，映射到<see cref="defaultValue"/>。<para/> 
        /// 调用<see cref="ParseBinding(GameObject, bool)"/>后，无论是否成功绑定，都不会在映射到映射到<see cref="defaultValue"/>。
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

        public void ParseBinding(object bindInstance, bool force = false)
        {
            if (ParseResult == null || force)
            {
                object instance = bindInstance;
                if (extnalObj != null && extnalObj)
                {
                    //有限使用自己保存的对象
                    instance = extnalObj;
                }

                (ParseResult, Getter, Setter) =
                    BindingParser.Instance.InitializeBinding<T>(BindingPath, instance);
            }
        }

        public string DebugParseResult()
        {
            string message = $"ParseResult:{ParseResult}  |  Value : {Value}  |  {BindingPath}";
            Debug.Log(message);
            return message;
        }
    }

    [Serializable]
    public class BindableValueInt : BindableValue<int>
    {

    }

    [Serializable]
    public class BindableValueString : BindableValue<string>
    {

    }
}
