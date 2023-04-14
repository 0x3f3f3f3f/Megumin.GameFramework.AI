using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEditor;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    [CustomEditor(typeof(BehaviorTreeRunner), true, isFallback = false)]
    public class BehaviorTreeRunnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var behaviorTreeRunner = (BehaviorTreeRunner)target;
            base.OnInspectorGUI();

            bool hasAsset = behaviorTreeRunner.BehaviorTreeAsset;
            if (GUILayout.Button("EditorTree"))
            {
                if (Application.isPlaying && behaviorTreeRunner.BehaviourTree != null)
                {
                    var editor = BehaviorTreeEditor.GetWindow(behaviorTreeRunner.BehaviourTree);
                    if (editor.CurrentAsset == null)
                    {
                        //todo 新窗口 退出playmode 销毁
                        //editor.Close();
                    }

                    editor.SetTreeAsset(behaviorTreeRunner.BehaviorTreeAsset);
                    editor.BeginDebug(behaviorTreeRunner.BehaviourTree);
                    editor.Focus();
                }
                else
                {
                    if (hasAsset)
                    {
                        BehaviorTreeEditor.OnOpenAsset(behaviorTreeRunner.BehaviorTreeAsset);
                    }
                }
            }

            if (GUILayout.Button("ReStart"))
            {
                behaviorTreeRunner.ReStart();
            }

            if (GUILayout.Button("ReParseBinding"))
            {
                behaviorTreeRunner.ReParseBinding();
            }

            if (GUILayout.Button("LogVariables"))
            {
                behaviorTreeRunner.LogVariables();
            }

            if (GUILayout.Button("OverrideVariable"))
            {
                if (hasAsset)
                {
                    GenericMenu bindMenu = new GenericMenu();
                    foreach (var item in behaviorTreeRunner.BehaviorTreeAsset.variables)
                    {
                        var isalreadOverride = behaviorTreeRunner.OverrideVariables.Table.Any(elem => elem.RefName == item.Name);
                        if (isalreadOverride)
                        {
                            bindMenu.AddDisabledItem(new GUIContent(item.Name));
                        }
                        else
                        {
                            bindMenu.AddItem(new GUIContent(item.Name), false, () =>
                            {
                                Debug.Log(item.Name);
                                if (item.TryInstantiate<IRefable>(out var value))
                                {
                                    behaviorTreeRunner.OverrideVariables.Table.Add(value);
                                }
                            });
                        }

                    }

                    foreach (var item in behaviorTreeRunner.BehaviorTreeAsset.UnityObjectRef)
                    {
                        var isalreadOverride = behaviorTreeRunner.OverrideUnityObjectRef.Any(elem => elem.Name == item.Name);
                        if (isalreadOverride)
                        {
                            bindMenu.AddDisabledItem(new GUIContent(item.Name));
                        }
                        else
                        {
                            bindMenu.AddItem(new GUIContent(item.Name), false, () =>
                            {
                                Debug.Log(item.Name);
                                behaviorTreeRunner.OverrideUnityObjectRef.Add(item);
                            });
                        }
                    }

                    bindMenu.ShowAsContext();
                }
            }
        }
    }
}


