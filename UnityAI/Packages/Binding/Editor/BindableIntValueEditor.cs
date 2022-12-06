using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Megumin.Binding.Editor
{
    [CustomPropertyDrawer(typeof(BindableIntValue))]
    public class BindableIntValueEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override async void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
            var bindp = property.FindPropertyRelative("BindingString");
            if (GUILayout.Button("Bind"))
            {
                bindp.stringValue = DateTime.UtcNow.ToString();
                var str = await BindingEditor.GetBindStr<int>();
                bindp.stringValue = str;
            }

        }
    }
}
