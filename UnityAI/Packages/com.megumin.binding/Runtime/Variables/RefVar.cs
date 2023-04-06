using System;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace Megumin.Binding
{
    /// <summary>
    /// 可以被引用的，可以共享的
    /// </summary>
    public interface IRefable
    {
        /// <summary>
        /// 如果是被引用的实例，那么代表自身的名字。
        /// 如果是引用其他实例的实例，那么代表要引用对象的名字。
        /// 如果一个实例是引用其他对象，那么就不必给他自身一个名字，是没有意义的。
        /// </summary>
        /// <remarks>
        /// 如果位于参数表中，名字表示是自身的名字。
        /// 如果位于节点字段成员或其他位置，名字表示引用的对象，运行时替换成引用的对象。
        /// </remarks>
        string RefName { get; set; }
    }

    public interface IRefVariableFinder
    {
        IEnumerable<IRefable> GetVariableTable();
        bool TryGetParam(string name, out IRefable variable);
        void Export(IRefable currentValue);
    }

    /// <summary>
    /// 可以引用的，可绑定参数。
    /// <para>
    /// 在Unity2023之前，在节点直接声明泛型，会导致无法公开到参数表示，使用特化类型。
    /// TODO,特化类型和参数表类型移动到当前包。
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// RefVariable，因为很常用，所以名字尽可能短一点
    /// </remarks>
    [Serializable]
    public class RefVar<T> : BindingVar<T>, IRefable
    {
        [field: SerializeField]
        protected string refName;
        public string RefName { get => refName; set => refName = value; }

        public static implicit operator RefVar<T>(T value)
        {
            return new RefVar<T> { value = value };
        }

        public static implicit operator T(RefVar<T> var)
        {
            if (var == null)
            {
                return default;
            }
            return var.Value;
        }
    }

    [Serializable]
    public class RefVariableSerializationData
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

            var type = Reflection.TypeCache.GetType(TypeName);
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

            var type = Megumin.Reflection.TypeCache.GetType(TypeName);
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

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(RefVar<>), true)]
    internal class RefVariableDrawer : PropertyDrawer
    {
        public override UnityEngine.UIElements.VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            //var container = new VisualElement();

            //foreach (SerializedProperty item in property)
            //{
            //    if (item.name == "Ref Name")
            //    {
            //        continue;
            //    }

            //    var memberField = new PropertyField();
            //    memberField.BindProperty(item);
            //    memberField.style.flexGrow = 1;
            //    container.Add(memberField);
            //}

            var field = new PropertyField();
            field.BindProperty(property);
            field.AddToClassList("refVar");

            //https://forum.unity.com/threads/propertyfield-name-contain-a-period-how-to-write-uss-selector.1412943
            StyleSheet styleSheet = Resources.Load<StyleSheet>("RefVar");
            field.styleSheets.Add(styleSheet);

            //此时元素还没有生成
            //var foldoutToggle = field.Q<Toggle>(className: "unity-foldout__toggle");
            //foldoutToggle.value = true;
            //foldoutToggle.style.display = DisplayStyle.None;
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
                var wrapper = property.serializedObject.targetObject;
                if (wrapper is IRefVariableFinder treeWrapper)
                {
                    OnGUISelector(position, property, label, treeWrapper);
                }
                else
                {
                    UnityEditor.EditorGUI.PropertyField(position, property, label, true);
                }
            }
        }

        public void OnGUISelector(Rect position,
                                  SerializedProperty property,
                                  GUIContent label,
                                  IRefVariableFinder treeWrapper)
        {
            //先绘制下拉选单，后绘制整个属性，不然会和折叠功能冲突

            var refName = property.FindPropertyRelative("refName");
            var index = 0;
            if (refName != null)
            {
                float exportButtonWidth = 46f;
                var exportButtonPos = position;
                exportButtonPos.xMin = exportButtonPos.xMax - exportButtonWidth;
                exportButtonPos.height = 18;

                float popupWidth = position.xMax - EditorGUIUtility.labelWidth - exportButtonWidth - 7;
                var selectPopupPos = position;
                selectPopupPos.width = popupWidth;
                selectPopupPos.x += EditorGUIUtility.labelWidth - 13;
                selectPopupPos.height = 18;

                //var obj = property.managedReferenceValue;
                option.Clear();
                optionDisplay.Clear();
                option.Add("Ref: None");
                optionDisplay.Add("Ref: None");

                var table = treeWrapper.GetVariableTable();
                foreach (var item in table)
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
                index = EditorGUI.Popup(selectPopupPos, index, strings);
                if (EditorGUI.EndChangeCheck())
                {
                    //var obj = property.GetValue<object>();
                    Undo.RecordObject(property.serializedObject.targetObject, "Change Ref");
                    if (index == 0)
                    {
                        //设置为null。
                        property.SetValue<object>(null);
                    }
                    else
                    {
                        if (treeWrapper.TryGetParam(option[index], out var variable))
                        {
                            property.SetValue<object>(variable);
                        }
                    }
                }

                IRefable currentValue = null;
                var enableExport = false;
                if (index == 0 && !string.IsNullOrEmpty(refName.stringValue))
                {
                    currentValue = property.GetValue<IRefable>();
                    if (currentValue != null)
                    {
                        enableExport = true;
                    }
                }

                using (new UnityEditor.EditorGUI.DisabledGroupScope(enableExport == false))
                {
                    if (GUI.Button(exportButtonPos, "Export"))
                    {
                        treeWrapper.Export(currentValue);
                    }
                }
            }

            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            using (new UnityEditor.EditorGUI.DisabledGroupScope(index != 0))
            {
                //这里如果是引用的对象，暂时不让在Inspector里修改，对象可能会被多个地方引用
                //让他在参数表中修改
                UnityEditor.EditorGUI.PropertyField(position, property, true);
            }
        }
    }

#endif
}



