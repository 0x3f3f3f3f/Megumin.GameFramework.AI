using System;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.Serialization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
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
        protected string refName;
        public string RefName { get => refName; set => refName = value; }
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
        public CollectionSerializationData fallbackData;

        //public string TypeName { get => typeNameOrRefName; set => typeNameOrRefName = fallbackValue; }
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
        //        typeNameOrRefName = $"{RefPrefix}{fallbackValue}";
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
                if (fallbackData.TryDeserialize(out var fallbackValue))
                {
                    var variable = Activator.CreateInstance(type) as IVariable;

                    if (variable is IBindableFallback fallback)
                    {
                        fallback.SetFallbackValue(fallbackValue);
                    }
                    else
                    {
                        variable.SetValue(fallbackValue);
                    }

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
            if (value is IRefable sharedable && !string.IsNullOrEmpty(sharedable.RefName))
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
                var fallbackValue = variable.GetValue();
                if (variable is IBindableFallback fallback)
                {
                    fallbackValue = fallback.GetFallbackValue();
                }

                CollectionSerializationData fallbackData = new();
                if (fallbackData.TrySerialize("fallbackValue", fallbackValue))
                {
                    this.fallbackData = fallbackData;
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

                if (item is IBindableFallback fallback)
                {
                    data = fallback.GetFallbackValue();
                }

                if (item is IBindable bindable)
                {
                    Path = bindable.BindingPath;
                }

                if (item is IRefable sharedable)
                {
                    Name = sharedable.RefName;
                }

                CollectionSerializationData fallbackData = new();
                if (fallbackData.TrySerialize("fallbackValue", data))
                {
                    this.fallbackData = fallbackData;
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
            var refable = Activator.CreateInstance(type) as IRefable;
            refable.RefName = Name;
            if (refable is IBindable bindable)
            {
                bindable.BindingPath = Path;
            }

            if (refable is IVariable variable)
            {
                if (fallbackData.TryDeserialize(out var fallbackValue))
                {
                    if (refable is IBindableFallback fallback)
                    {
                        fallback.SetFallbackValue(fallbackValue);
                    }
                    else
                    {
                        variable.SetValue(fallbackValue);
                    }
                }
            }

            value = refable;
            return true;
        }
    }

    public interface ITreeElementWrapper
    {
        VariableTable GetVariableTable();
    }

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(RefVariable<>), true)]
    public class Pro : PropertyDrawer
    {
        public override UnityEngine.UIElements.VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var field = new PropertyField();
            field.BindProperty(property);
            return field;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        List<string> option = new List<string>();
        List<string> optionDisplay = new List<string>();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                var refName = property.FindPropertyRelative("refName");
                if (refName != null)
                {
                    float popupWidth = position.width - EditorGUIUtility.labelWidth;
                    var buttonPosition = position;
                    buttonPosition.width = popupWidth;
                    buttonPosition.height = 18;
                    buttonPosition.x += position.width - popupWidth;

                    //var obj = property.managedReferenceValue;
                    option.Clear();
                    optionDisplay.Clear();
                    option.Add("Ref: None");
                    optionDisplay.Add("Ref: None");

                    var wrapper = property.serializedObject.targetObject;
                    VariableTable table = null;
                    if (wrapper is ITreeElementWrapper treeWrapper)
                    {
                        table = treeWrapper.GetVariableTable();
                        foreach (var item in table.Table)
                        {
                            option.Add(item.RefName);
                            if (item is IVariableSpecializedType specializedType)
                            {
                                optionDisplay.Add($"{item.RefName} : [{specializedType.SpecializedType.Name}]");
                            }
                            else
                            {
                                optionDisplay.Add(item.RefName);
                            }
                        }
                    }

                    var index = 0;
                    var currentRefNameValue = refName.stringValue;
                    if (option.Contains(refName.stringValue))
                    {
                        for (int i = 0; i < option.Count; i++)
                        {
                            if (currentRefNameValue == option[i])
                            {
                                index = i;
                            }
                        }
                    }

                    string[] strings = optionDisplay.ToArray();
                    EditorGUI.BeginChangeCheck();
                    index = EditorGUI.Popup(buttonPosition, index, strings);
                    if (EditorGUI.EndChangeCheck())
                    {
                        //var obj = property.GetValue<object>();
                        Undo.RecordObject(property.serializedObject.targetObject, "Change Ref");
                        if (index == 0)
                        {
                            //设置未null。
                            property.SetValue<object>(null);
                        }
                        else
                        {
                            var variable = table.Table[index - 1];
                            property.SetValue<object>(variable);
                        }
                    }
                }

                //先绘制下拉选单，后绘制整个属性，不然会和折叠功能冲突
                var ex = EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
#endif
}
