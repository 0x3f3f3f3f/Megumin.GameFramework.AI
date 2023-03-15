using System;

namespace Megumin.Binding
{
    /// <summary>
    /// 用于识别公开参数
    /// </summary>
    public interface IVariable
    {
        object GetValue();
        void SetValue(object value);
    }

    public interface IVariable<T>
    {
        T Value { get; set; }
    }

    /// <summary>
    /// 可绑定的，绑定到一个组件的成员
    /// </summary>
    public interface IBindable
    {
        string BindingPath { get; set; }
    }

    /// <summary>
    /// 存在fallback值时，<see cref="IVariable.GetValue()"/> 获取的是未解析值。
    /// 会导致fallback值没办法获取并序列化，所以需要一个单独的接口来获取fallback值。
    /// </summary>
    public interface IBindableFallback 
    {
        object GetFallbackValue();
        void SetFallbackValue(object value);
    }

    public interface IBindingParseable
    {
        ParseBindingResult ParseBinding(object bindInstance, bool force = false);
        string DebugParseResult();
    }

    [Flags]
    public enum ParseBindingResult
    {
        /// <summary>
        /// Get Set 均解析失败
        /// </summary>
        None = 0,
        Get = 1 << 0,
        Set = 1 << 1,
        Both = Get | Set,
    }
}
