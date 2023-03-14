using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

        Dictionary<string, ParseBindingResult> parseResult = new Dictionary<string, ParseBindingResult>();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                var ex = EditorGUI.PropertyField(position, property, label, true);
                var bindp = property.FindPropertyRelative("bindingPath");
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
                    if (parseResult.TryGetValue(property.propertyPath, out var presult))
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("MatchResult");
                        GUILayout.Label(presult.ToString());
                        GUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button($"{property.propertyPath}_TestParse"))
                    {
                        //通过property取得实例对象
                        //https://gist.github.com/douduck08/6d3e323b538a741466de00c30aa4b61f

                        var obj = property.serializedObject.targetObject;
                        object data = null;
                        if (property.propertyPath.EndsWith("]"))
                        {
                            data = property.managedReferenceValue;
                        }
                        else
                        {
                            data = this.fieldInfo.GetValue(obj);
                        }

                        if (data == null)
                        {

                        }

                        if (data is IBindingParseable parseable)
                        {
                            GameObject gameObject = obj as GameObject;
                            if (obj is Component component)
                            {
                                gameObject = component.gameObject;
                            }
                            parseResult[property.propertyPath]
                                = parseable.ParseBinding(gameObject, true);
                            parseable.DebugParseResult();
                        }

                        //fieldInfo = this.fieldInfo; 
                        //var field2 = this.fieldInfo;
                        //var v = field2.GetValue(property.serializedObject.targetObject);
                        //var index = property.enumValueIndex;

                        ////Debug.Log(property.serializedObject.targetObject);

                        //Type type = obj.GetType();
                        //var fieldInfo = type.GetField(property.propertyPath);
                        //var fValue = fieldInfo.GetValue(obj);

                        //if (fValue is IBindingParseable parseable)
                        //{
                        //    GameObject gameObject = obj as GameObject;
                        //    if (obj is Component component)
                        //    {
                        //        gameObject = component.gameObject;
                        //    }
                        //    parseable.ParseBinding(gameObject, true);
                        //    parseable.DebugParseResult();
                        //}
                    }
                }
            }

        }


    }
}
