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
