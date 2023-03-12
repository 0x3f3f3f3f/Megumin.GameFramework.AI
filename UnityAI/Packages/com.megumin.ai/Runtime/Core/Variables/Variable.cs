using System;
using Megumin.GameFramework.AI.Serialization;
using Megumin.Serialization;

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



    [Serializable]
    public class VariableSerializationData : SerializationData
    {
        public string TypeName;
        public string Path;
        public CollectionSerializationData fallbackData;

        public bool TrySerialize(IVariable item)
        {
            if (item is TestVariable variable)
            {
                TypeName = variable.GetType().FullName;
                Name = variable.Name;
                Path = variable.Path;
                fallbackData = new CollectionSerializationData();
                return fallbackData.TrySerialize("fallbackData", variable.GetValue());
            }

            return false;
        }

        public bool TryDeserialize(out IVariable value)
        {
            value = default;

            var type = TypeCache.GetType(TypeName);
            if (type == null)
            {
                return false;
            }
            var variable = Activator.CreateInstance(type) as TestVariable;
            variable.Name = Name;
            variable.Path = Path;
            if (fallbackData.TryDeserialize(out var data))
            {
                variable.SetValue(data);
            }
            value = variable;
            return true;
        }
    }
}


