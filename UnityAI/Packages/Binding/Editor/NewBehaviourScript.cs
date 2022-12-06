using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Megumin.Binding.Editor
{
    [CustomPropertyDrawer(typeof(SerializeValue), true)]
    public class SerializeValueEditor : PropertyDrawer
    {
        //public override VisualElement CreatePropertyGUI(SerializedProperty property)
        //{
        //    VisualElement element = new VisualElement();
        //    element.Add(new Label("Hello"));
        //    return element;
        //}

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.Label(position, "Test");
            //base.OnGUI(position, property, label);
        }
    }
}
