using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Megumin.GameFramework.AI
{
    public class TreeElementSetterAttribute : PropertyAttribute
    {
    }

    public interface ITreeElementWrapper
    {
        void GetAllElementsDerivedFrom(Type baseType, List<ITreeElement> elems);
    }

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(TreeElementSetterAttribute))]
    public class TreeElementSetterAttributeDrawer : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (typeof(ITreeElement).IsAssignableFrom(fieldInfo.FieldType))
            {
                return 18f;
            }
            else
            {
                return UnityEditor.EditorGUI.GetPropertyHeight(property, true);
            }
        }

        List<string> option = new List<string>();
        string[] displayedOptions = null;
        List<ITreeElement> elems = null;
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (typeof(ITreeElement).IsAssignableFrom(fieldInfo.FieldType))
            {
                if (elems == null)
                {
                    option.Clear();
                    option.Add("Ref: None");
                    elems = new();
                    var wrapper = property.serializedObject.targetObject;
                    if (wrapper is ITreeElementWrapper elemWrapper)
                    {
                        elemWrapper.GetAllElementsDerivedFrom(fieldInfo.FieldType, elems);

                        foreach (var item in elems)
                        {
                            option.Add(item.ToString());
                        }
                    }
                    displayedOptions = option.ToArray();
                }

                var obj = property.GetValue<ITreeElement>();
                var index = 0;
                if (obj != null)
                {
                    for (int i = 0; i < elems.Count; i++)
                    {
                        if (elems[i].GUID == obj.GUID)
                        {
                            index = i + 1;
                        }
                    }
                }

                //EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                EditorGUI.BeginChangeCheck();
                index = EditorGUI.Popup(position, label.text, index, displayedOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    //var obj = property.GetValue<object>();
                    Undo.RecordObject(property.serializedObject.targetObject, "Change ITreeElement Ref");
                    if (index == 0)
                    {
                        //设置为null。
                        property.SetValue<object>(null);
                    }
                    else
                    {
                        property.SetValue<object>(elems[index - 1]);
                    }
                }
            }
            else
            {
                //类型不是ITreeElement,回退到普通GUI
                UnityEditor.EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }

#endif
}
