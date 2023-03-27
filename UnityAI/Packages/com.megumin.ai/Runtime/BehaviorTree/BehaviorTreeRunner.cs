using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        //[field: SerializeField]
        public BehaviorTree BehaviourTree { get; protected set; }
        public BehaviorTreeAsset_1_0_1 BehaviorTreeAsset;
        public TickMode TickMode = TickMode.Update;

        public bool AutoEnable = true;
        [field: SerializeField]
        public OperationTree OnEnabled { get; set; } = OperationTree.Enable;

        [field: SerializeField]
        public OperationTree OnDisabled { get; set; } = OperationTree.Disable;
        //Todo
        //完成时重新开始
        //后台线程异步实例化
        //预加载子树
        public RunOption RunOption;


        private void OnEnable()
        {
            if (BehaviourTree != null)
            {
                if (OnEnabled.HasFlag(OperationTree.Enable)
                    || OnEnabled.HasFlag(OperationTree.Resume))
                {
                    DisableTree();
                }
            }

            if (AutoEnable)
            {
                EnableTree();
            }
        }

        private void OnDisable()
        {
            if (BehaviourTree != null)
            {
                if (OnDisabled.HasFlag(OperationTree.Disable)
                    || OnDisabled.HasFlag(OperationTree.Pause))
                {
                    DisableTree();
                }
            }
        }

        public void EnableTree()
        {
            if (BehaviourTree == null && BehaviorTreeAsset)
            {
                RefFinder refFinder = null;

                if (Override != null)
                {
                    refFinder = new RefFinder();
                    foreach (var item in Override.Table)
                    {
                        if (string.IsNullOrEmpty(item?.RefName))
                        {
                            continue;
                        }
                        refFinder.RefDic[item.RefName] = item;
                    }

                    if (refFinder.RefDic.Count == 0)
                    {
                        refFinder = null;
                    }
                }

                BehaviourTree = BehaviorTreeAsset.Instantiate(false, refFinder);
                BehaviourTree.RunOption = RunOption;
                BehaviourTree.Init(gameObject);
            }

            if (BehaviourTree != null)
            {
                BehaviorTreeManager.Instance.AddTree(BehaviourTree, TickMode);
                BehaviourTree.IsRunning = true;
            }
        }

        public void DisableTree()
        {
            if (BehaviourTree != null)
            {
                BehaviorTreeManager.Instance.RemoveTree(BehaviourTree);
                BehaviourTree.IsRunning = false;
            }
        }

        [Editor]
        public void EditorTree()
        {
            UnityEditor.AssetDatabase.OpenAsset(BehaviorTreeAsset);
        }

        [Editor]
        public void ResetTree()
        {
            BehaviourTree.Reset();
        }

        [Editor]
        public void Rebind()
        {
            BehaviourTree.ParseAllBindable(gameObject, true);
        }

        [Editor]
        public void LogVariable(string name = "TestStringVariable")
        {
            if (BehaviourTree.Variable.TryGetParam<string>(name, out var variable))
            {
                Debug.Log(variable.Value);
            }
        }

        public VariableTable Override = new();

        [Editor]
        public void OverrideVariable()
        {
            Override.Table.Add(new RefVariable_string());
            if (BehaviorTreeAsset)
            {
                foreach (var item in BehaviorTreeAsset.variables)
                {

                }
            }
        }

        private void OnValidate()
        {
            if (BehaviourTree?.IsRunning == true)
            {
                //调试时tickmode改变
                EnableTree();
            }
        }
    }
}


