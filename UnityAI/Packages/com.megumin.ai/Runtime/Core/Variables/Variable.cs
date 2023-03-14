using System;
using Megumin.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace Megumin.GameFramework.AI
{
    //public interface IVariable
    //{
    //    string Name { get; }
    //    object GetValue();
    //}

    public interface IVariable<T>
    {
        T Value { get; set; }
    }

    //[Serializable]
    //public class TestVariable : IVariable
    //{
    //    [field: UnityEngine.SerializeField]
    //    public string Name { get; set; }

    //    public string Path;

    //    public ParamVariableMode Mode = ParamVariableMode.MappingAndFallback;
    //    public virtual object GetValue() { return null; }
    //    public virtual void SetValue(object value) { }
    //}

    [Flags]
    public enum ParamVariableMode
    {
        MappingAndFallback = Mapping | Direct,
        Mapping = 1 << 0,
        Direct = 1 << 1,
    }

    ///// <summary>
    ///// 需要特化类型，不然不支持泛型序列化的版本没办法UndoRecode。
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //[Serializable]
    //public class ParamVariable<T> : TestVariable, IVariable<T>
    //{
    //    [field: UnityEngine.SerializeField]
    //    public T Value { get; set; }

    //    public override object GetValue()
    //    {
    //        return Value;
    //    }

    //    public override void SetValue(object value)
    //    {
    //        Value = (T)value;
    //    }
    //}

    /// <summary>
    /// 用于识别公开参数 todo 重命名
    /// </summary>
    public interface IMMDataable
    {
        object GetValue();
        void SetValue(object value);
    }

    [Serializable]
    public class MMData : IMMDataable
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
    /// 有Value 不一定有Path ，有Path 不一定有 Name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MMData<T> : MMData, IVariable<T>
    {
        [field: SerializeField]
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
    /// 可绑定的，绑定到一个组件
    /// </summary>
    public interface IBindable
    {
        string Path { get; set; }
    }

    /// <summary>
    /// Get,Set 分别设置
    /// </summary>
    [Flags]
    public enum ParsMode
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

    public interface IRefFinder
    {
        bool TryGetRefValue(string refName, out object refValue);
    }

    [Serializable]
    public class MMDataSerializationData
    {
        public string MemberName;
        public string RefName;
        /// <summary>
        /// 类型名和引用名公用的保存字段
        /// </summary>
        //public string typeNameOrRefName;
        public string Path;
        /// <summary>
        /// 必须保存类型FullName，值得真实类型，或者成员的声明类型。
        /// </summary>
        public string TypeName;
        public bool IsRef => !string.IsNullOrEmpty(RefName);
        public CollectionSerializationData Data;

        //public string TypeName { get => typeNameOrRefName; set => typeNameOrRefName = value; }
        //public bool IsRef => typeNameOrRefName.StartsWith(RefPrefix);

        //public const string RefPrefix = @"""$rid"":";
        //public string RefName
        //{
        //    get
        //    {
        //        return typeNameOrRefName.Substring(RefPrefix.Length);
        //    }
        //    set
        //    {
        //        typeNameOrRefName = $"{RefPrefix}{value}";
        //    }
        //}

        public bool TryDeserialize(out object vara, IRefFinder refFinder)
        {
            var type = Serialization.TypeCache.GetType(TypeName);
            if (type == null)
            {
                Debug.LogError($"反序列化公开参数 没有找到对应类型 TypeName:{TypeName}");
                vara = null;
                return false;
            }

            if (IsRef)
            {
                //typeof(IRefSharedable).IsAssignableFrom(type)
                //引用不要构造实例，通过外部去获取
                if (refFinder != null && refFinder.TryGetRefValue(RefName, out var refValue)
                    && type.IsAssignableFrom(refValue?.GetType()))//AutoConvert 情况下不用验证类型
                {
                    vara = refValue;
                    return true;
                }
                else
                {
                    Debug.LogError($"没有找到 类型匹配的 RefValue");
                    //即使missRef，也要反射构造一个参数，将RefName反序列化出来，方式第二次保存时
                    //RefName丢失。
                    var variable = Activator.CreateInstance(type) as IRefSharedable;
                    variable.Name = RefName;

                    if (variable is IBindable bindable)
                    {
                        bindable.Path = Path;
                    }

                    vara = variable;
                    return true;
                }
            }
            else
            {
                if (Data.TryDeserialize(out var value))
                {
                    var variable = Activator.CreateInstance(type) as IMMDataable;
                    variable.SetValue(value);

                    if (variable is IBindable bindable)
                    {
                        bindable.Path = Path;
                    }
                    vara = variable;
                    return true;
                }
            }

            vara = null;
            return false;
        }
    }

    [Serializable]
    public class VariableSerializationData : SerializationData
    {
        public string TypeName;
        public string Path;
        public CollectionSerializationData fallbackData;

        public bool TrySerialize(IRefSharedable item)
        {
            if (item is IMMDataable variable)
            {
                TypeName = variable.GetType().FullName;

                var data = variable.GetValue();
                if (item is IBindable bindable)
                {
                    Path = bindable.Path;
                }

                if (item is IRefSharedable sharedable)
                {
                    Name = sharedable.Name;
                }

                CollectionSerializationData valueData = new();
                if (valueData.TrySerialize("Value", data))
                {
                    fallbackData = valueData;
                }
                return true;
            }

            return false;
        }

        public bool TryDeserialize(out IRefSharedable value)
        {
            value = default;

            var type = Megumin.Serialization.TypeCache.GetType(TypeName);
            if (type == null)
            {
                return false;
            }
            var variable = Activator.CreateInstance(type) as IRefSharedable;
            variable.Name = Name;
            if (variable is IBindable bindable)
            {
                bindable.Path = Path;
            }

            if (variable is IMMDataable dataable)
            {
                if (fallbackData.TryDeserialize(out var data))
                {
                    dataable.SetValue(data);
                }
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


