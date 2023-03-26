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
    public class TreeElementRefAttribute : PropertyAttribute
    {
    }

    public interface ITreeElementWrapper
    {
        void GetAllRefDerivedFrom(Type baseType, List<ITreeElement> refables);
    }

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(TreeElementRefAttribute))]
    public class TreeElementRefAttributeDrawer : UnityEditor.PropertyDrawer
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
        List<ITreeElement> elems = new();
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (typeof(ITreeElement).IsAssignableFrom(fieldInfo.FieldType))
            {
                option.Clear();
                elems.Clear();
                option.Add("Ref: None");

                var wrapper = property.serializedObject.targetObject;
                if (wrapper is ITreeElementWrapper elemWrapper)
                {
                    elemWrapper.GetAllRefDerivedFrom(fieldInfo.FieldType, elems);

                    foreach (var item in elems)
                    {
                        option.Add(item.ToString());
                    }
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

                string[] strings = option.ToArray();
                //EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                EditorGUI.BeginChangeCheck();
                index = EditorGUI.Popup(position, label.text, index, strings);
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
