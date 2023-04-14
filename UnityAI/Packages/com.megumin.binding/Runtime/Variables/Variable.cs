using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Serialization;
using UnityEngine;

namespace Megumin.Binding
{
    [Serializable]
    public class Variable : IVariable
    {
        public virtual object GetValue()
        {
            return null;
        }

        public virtual void SetValue(object value)
        {

        }
    }

    /// <summary>
    /// 有Value 不一定有Path ，有Path 不一定有 RefName
    /// 需要特化类型，不然不支持泛型序列化的版本没办法UndoRecode。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Variable<T> : Variable,
        IVariable<T>,
        IBindableFallback,
        IVariableSpecializedType
    {
        [field: SerializeField]
        public T value;

        public virtual Type SpecializedType => typeof(T);

        public virtual T Value { get => value; set => this.value = value; }

        public override object GetValue()
        {
            return Value;
        }

        public override void SetValue(object value)
        {
            Value = (T)value;
        }

        public virtual object GetFallbackValue()
        {
            return value;
        }

        public virtual void SetFallbackValue(object value)
        {
            this.value = (T)value;
        }

        public static implicit operator Variable<T>(T value)
        {
            return new Variable<T> { value = value };
        }

        public static implicit operator T(Variable<T> var)
        {
            return var.Value;
        }
    }

    /// <summary>
    /// Get,Set 分别设置
    /// </summary>
    [Flags]
    public enum ParseMode
    {
        None = 0,
        Log = 1 << 0,
        Worning = 1 << 1,
        Error = 1 << 2,
        ThrowException = 1 << 3,
        FallbackValue = 1 << 4,
        FallbackTypeDefault = 1 << 5,
    }

    /// <summary>
    /// 绑定到一个组件。但是不能存到参数表中，也不能共享
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BindingVar<T> : Variable<T>, IBindable, IBindingParseable
    {
        [field: BindingPathSetter]
        [field: SerializeField]
        protected string bindingPath;

        public string BindingPath { get => bindingPath; set => bindingPath = value; }
        public ParseMode GetMode = ParseMode.FallbackValue;
        public ParseMode SetMode = ParseMode.FallbackValue;

        public override T Value
        {
            get
            {
                if (ParseResult.HasValue)
                {
                    if ((ParseResult.Value & ParseBindingResult.Get) != 0)
                    {
                        //解析成功
                        return Getter();
                    }
                    else
                    {
                        //解析失败
                        if ((GetMode & ParseMode.Log) != 0)
                        {
                            DebugLogInValue();
                        }
                    }
                }
                else
                {
                    //还未解析
                    if ((GetMode & ParseMode.Log) != 0)
                    {
                        DebugLogInValue();
                    }
                }

                if ((GetMode & ParseMode.FallbackValue) != 0)
                {
                    return base.value;
                }

                if ((GetMode & ParseMode.FallbackTypeDefault) != 0)
                {
                    return default;
                }

                throw new Exception();
            }

            set
            {
                if (ParseResult.HasValue)
                {
                    if ((ParseResult.Value & ParseBindingResult.Set) != 0)
                    {
                        //解析成功
                        Setter(value);
                    }
                    else
                    {
                        //解析失败
                        if ((SetMode & ParseMode.Log) != 0)
                        {
                            DebugLogInValue();
                        }
                    }
                }
                else
                {
                    //还未解析
                    if ((SetMode & ParseMode.Log) != 0)
                    {
                        DebugLogInValue();
                    }
                }

                if ((SetMode & ParseMode.FallbackValue) != 0)
                {
                    base.value = value;
                    return;
                }

                if ((SetMode & ParseMode.FallbackTypeDefault) != 0)
                {
                    return;
                }

                throw new Exception();
            }
        }

        /// <summary>
        /// null表示还没有解析绑定
        /// </summary>
        [NonSerialized]
        protected ParseBindingResult? ParseResult = null;
        protected Func<T> Getter;
        protected Action<T> Setter;
        static readonly object parseLock = new object();
        public ParseBindingResult ParseBinding(object bindInstance, bool force = false, object options = null)
        {
            lock (parseLock)
            {
                if (ParseResult == null || force)
                {
                    object instance = bindInstance;

                    (ParseResult, Getter, Setter) =
                        BindingParser.Instance.ParseBinding<T>(BindingPath, instance, options);
                }

                return ParseResult ?? ParseBindingResult.None;
            }
        }

        public string DebugParseResult()
        {
            string message = $"ParseResult:{ParseResult}  | Value:{Value} |  {typeof(T)}  |  {BindingPath}";
            Debug.Log(message);
            return message;
        }

        /// <summary>
        /// 在Value Get Set内使用的Log方法
        /// </summary>
        /// <returns></returns>
        protected string DebugLogInValue()
        {
            string message = $"ParseResult:{ParseResult}  |  {typeof(T)}  |  {BindingPath}";
            Debug.Log(message);
            return message;
        }

        public static implicit operator BindingVar<T>(T value)
        {
            return new BindingVar<T> { value = value };
        }

        public static implicit operator T(BindingVar<T> var)
        {
            if (var == null)
            {
                return default;
            }
            return var.Value;
        }
    }
}



