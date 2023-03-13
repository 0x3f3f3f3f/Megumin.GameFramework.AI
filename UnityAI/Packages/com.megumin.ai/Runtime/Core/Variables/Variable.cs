using System;
using Megumin.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

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
    public class TestVariable : IVariable
    {
        [field: UnityEngine.SerializeField]
        public string Name { get; set; }

        public string Path;

        public ParamVariableMode Mode = ParamVariableMode.MappingAndFallback;
        public virtual object GetValue() { return null; }
        public virtual void SetValue(object value) { }
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
    /// 用于识别公开参数 todo 重命名
    /// </summary>
    public interface IMMDataable
    {
        object GetValue();
        void SetValue(object value);
    }

    /// <summary>
    /// 有Value 不一定有Path ，有Path 不一定有 Name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MMData<T> : IMMDataable
    {
        [field: SerializeField]
        public T Value { get; set; }

        public virtual object GetValue()
        {
            return Value;
        }

        public virtual void SetValue(object value)
        {
            Value = (T)value;
        }
    }

    /// <summary>
    /// 可绑定的，绑定到一个组件
    /// </summary>
    public interface IBindable
    {
        string Path { get; set; }
    }

    /// <summary>
    /// 绑定到一个组件。但是不能存到参数表中，也不能共享
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MMData2<T> : MMData<T>, IBindable
    {
        [field: SerializeField]
        public string Path { get; set; }
        public ParamVariableMode Mode = ParamVariableMode.MappingAndFallback;
    }

    /// <summary>
    /// 可以存放在参数表的，可以在多个节点共享的
    /// </summary>
    public interface IRefSharedable
    {
        string Name { get; set; }
    }

    [Serializable]
    public class MMData3<T> : MMData2<T>, IRefSharedable
    {
        [field: SerializeField]
        public string Name { get; set; }
    }

    /// <summary>
    /// 可以自动类型转换的
    /// </summary>
    public interface IConvtable
    {

    }

    /// <summary>
    /// Wapper类可以类型转换的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MMAutoConvertData<T>
    {
        /// <summary>
        /// 必然不是T类型，否则就不用转型了。
        /// </summary>
        public IRefSharedable RefVar { get; set; }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public struct Trigger
    {
        public bool Value;
    }

    [Serializable]
    public class MMDataSerializationData
    {
        public string MemberName;
        public string TypeName;
        public CollectionSerializationData Data;
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

            var type = Megumin.Serialization.TypeCache.GetType(TypeName);
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


#if UNITY_EDITOR

    //[UnityEditor.CustomPropertyDrawer(typeof(TestVariable))]
    public class Pro : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                var ex = EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
#endif
}


