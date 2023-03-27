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
                if (hasAsset)
                {
                    BehaviorTreeEditor.OnOpenAsset(behaviorTreeRunner.BehaviorTreeAsset);
                }
            }

            if (GUILayout.Button("ResetTree"))
            {
                behaviorTreeRunner.ResetTree();
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
                        var isalreadOverride = behaviorTreeRunner.Override.Table.Any(elem => elem.RefName == item.Name);
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
                                    behaviorTreeRunner.Override.Table.Add(value);
                                }
                            });
                        }

                    }
                    bindMenu.ShowAsContext();
                }
            }
        }
    }
}


