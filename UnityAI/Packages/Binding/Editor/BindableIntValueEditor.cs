using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Megumin.Binding.Editor
{
    [CustomPropertyDrawer(typeof(BindableValue<>), true)]
    [CustomPropertyDrawer(typeof(BindableValueInt))]
    public class BindableIntValueEditor : PropertyDrawer
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
                var bindp = property.FindPropertyRelative("BindingPath");
                var xoffset = property.FindPropertyRelative("xOffset");
                int xo = xoffset.intValue;
                var yoffset = property.FindPropertyRelative("yOffset");
                int yo = yoffset.intValue;
                //if (results.TryGetValue(property.propertyPath, out var str))
                //{
                //    results.Remove(property.propertyPath);
                //    bindp.stringValue = str;
                //}
                var test = property.FindPropertyRelative("Key");

                //Rect rect = GUILayoutUtility.GetLastRect();
                //test.stringValue = rect.ToString();
                //rect = new Rect(position.x, GetPropertyHeight(property, label) + 46, position.width, 20);
                //EditorGUI.DrawRect(rect, Color.green);
                if (property.isExpanded)
                {
                    //if (GUILayout.Button($"{property.propertyPath}_Bind"))
                    //{
                    //    NewMethod(property.propertyPath);
                    //}

                    if (bindp.GetBindintString(GUILayout.Button($"{bindp.propertyPath}_Bind"), out var res))
                    {
                        bindp.stringValue = res;
                    }
                }
            }

        }

        
    }
}
