using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI
{
    public class TreeElementRefAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(TreeElementRefAttribute))]
    public class BindingPathAttributeDrawer : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return UnityEditor.EditorGUI.GetPropertyHeight(property, true);
        }


        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            using (new UnityEditor.EditorGUI.DisabledGroupScope(true))
            {
                //TODO  Popup
                var obj = property.GetValue<TreeElement>();
                UnityEditor.EditorGUI.TextField(position, label, obj?.GUID);
            }
        }
    }

#endif
}
