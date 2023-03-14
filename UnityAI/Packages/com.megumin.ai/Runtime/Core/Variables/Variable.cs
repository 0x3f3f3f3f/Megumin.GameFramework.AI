using System;
using Megumin.Binding;
using Megumin.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace Megumin.GameFramework.AI
{
    //IRefable 不要放在binding包里，和bind功能相关性不高，和参数表功能相关性更高

    /// <summary>
    /// 可以存放在参数表的，可以在多个节点共享的
    /// </summary>
    public interface IRefable
    {
        string RefName { get; set; }
    }

    [Serializable]
    public class RefVariable<T> : BindingVariable<T>, IRefable
    {
        [field: SerializeField]
        public string RefName { get; set; }
    }

    /// <summary>
    /// 可以自动类型转换的
    /// </summary>
    public interface IAutoConvertable
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
        public IRefable RefVar { get; set; }
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
        public string BindingPath;
        /// <summary>
        /// 必须保存类型FullName，值得真实类型，或者成员的声明类型。
        /// </summary>
        public string TypeName;
        public bool IsRef => !string.IsNullOrEmpty(RefName);
        public CollectionSerializationData Data;

        //public string TypeName { get => typeNameOrRefName; set => typeNameOrRefName = date; }
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
        //        typeNameOrRefName = $"{RefPrefix}{date}";
        //    }
        //}
        const string NullString = "$null";
        public bool TryDeserialize(out object vara, IRefFinder refFinder)
        {
            if (TypeName == NullString)
            {
                vara = null;
                return true;
            }

            var type = Serialization.TypeCache.GetType(TypeName);
            if (type == null)
            {
                Debug.LogError($"反序列化公开参数 没有找到对应类型 TypeName:{TypeName}");
                vara = null;
                return false;
            }

            if (IsRef)
            {
                //typeof(IRefable).IsAssignableFrom(type)
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
                    var variable = Activator.CreateInstance(type) as IRefable;
                    variable.RefName = RefName;

                    if (variable is IBindable bindable)
                    {
                        bindable.BindingPath = BindingPath;
                    }

                    vara = variable;
                    return true;
                }
            }
            else
            {
                if (Data.TryDeserialize(out var value))
                {
                    var variable = Activator.CreateInstance(type) as IVariable;
                    variable.SetValue(value);

                    if (variable is IBindable bindable)
                    {
                        bindable.BindingPath = BindingPath;
                    }
                    vara = variable;
                    return true;
                }
            }

            vara = null;
            return false;
        }

        public bool TrySerialize(string name, object value)
        {
            MemberName = name;

            if (value == null)
            {
                TypeName = NullString;
                return true;
            }

            TypeName = value.GetType().FullName;
            if (value is IRefable sharedable)
            {
                RefName = sharedable.RefName;
                return true;
            }

            if (value is IBindable bindable)
            {
                BindingPath = bindable.BindingPath;
            }

            if (value is IVariable variable)
            {
                var data = variable.GetValue();
                CollectionSerializationData valueData = new();
                if (valueData.TrySerialize("Value", data))
                {
                    Data = valueData;
                }
            }

            return true;
        }
    }

    [Serializable]
    public class VariableSerializationData : SerializationData
    {
        public string TypeName;
        public string Path;
        public CollectionSerializationData fallbackData;

        public bool TrySerialize(IRefable item)
        {
            if (item is IVariable variable)
            {
                TypeName = variable.GetType().FullName;

                var data = variable.GetValue();
                if (item is IBindable bindable)
                {
                    Path = bindable.BindingPath;
                }

                if (item is IRefable sharedable)
                {
                    Name = sharedable.RefName;
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

        public bool TryDeserialize(out IRefable value)
        {
            value = default;

            var type = Megumin.Serialization.TypeCache.GetType(TypeName);
            if (type == null)
            {
                return false;
            }
            var variable = Activator.CreateInstance(type) as IRefable;
            variable.RefName = Name;
            if (variable is IBindable bindable)
            {
                bindable.BindingPath = Path;
            }

            if (variable is IVariable dataable)
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


