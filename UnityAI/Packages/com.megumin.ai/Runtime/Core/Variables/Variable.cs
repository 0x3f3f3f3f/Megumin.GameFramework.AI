using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI
{
    public interface IVariable
    {
        string Name { get; }
        object GetValue();
    }

    public interface IVariable<T>
    {
        T Value { get; set; }
    }

    [Serializable]
    public abstract class TestVariable : IVariable
    {
        [field: UnityEngine.SerializeField]
        public string Name { get; set; }

        public string Path;

        public ParamVariableMode Mode = ParamVariableMode.MappingAndFallback;
        public abstract object GetValue();
        public abstract void SetValue(object value);
    }

    [Flags]
    public enum ParamVariableMode
    {
        MappingAndFallback = Mapping | Direct,
        Mapping = 1 << 0,
        Direct = 1 << 1,
    }

    /// <summary>
    /// 需要特化类型，不然不支持泛型序列化的版本没办法UndoRecode。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ParamVariable<T> : TestVariable, IVariable<T>
    {
        [field: UnityEngine.SerializeField]
        public T Value { get; set; }

        public override object GetValue()
        {
            return Value;
        }

        public override void SetValue(object value)
        {
            Value = (T)value;
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public struct Trigger
    {
        public bool Value;
    }
}
