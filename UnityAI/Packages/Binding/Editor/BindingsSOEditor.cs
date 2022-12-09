using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Megumin.Binding;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Megumin.Binding.Editor;

//[CustomEditor(typeof(BindingsSO))]
//public class BindingsSOEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        var so = target as BindingsSO;
//        base.OnInspectorGUI();
//        if (GUILayout.Button("Test"))
//        {
//            Bind(so);
//        }
//    }

//    private async void Bind(BindingsSO so)
//    {
//        so.BindStr = await BindingEditor.GetBindStr<int>();
//        EditorUtility.SetDirty(so);
//    }



//}


