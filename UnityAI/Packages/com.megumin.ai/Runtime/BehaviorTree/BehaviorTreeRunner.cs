using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using Megumin.Serialization;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        //[field: SerializeField]
        public BehaviorTree BehaviourTree { get; protected set; }
        public BehaviorTreeAsset_1_1 BehaviorTreeAsset;
        public TickMode TickMode = TickMode.Update;

        public bool AutoEnable = true;
        [field: SerializeField]
        public OperationTree OnEnabled { get; set; } = OperationTree.Enable;

        [field: SerializeField]
        public OperationTree OnDisabled { get; set; } = OperationTree.Disable;
        public InitOption InitOption;
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
                Override?.ParseBinding(gameObject, true);

                BehaviourTree = BehaviorTreeAsset.Instantiate(InitOption, refFinder);
                BehaviourTree.RunOption = RunOption;
                BehaviourTree.BindAgent(gameObject);
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

        public void ResetTree()
        {
            BehaviourTree.Reset();
        }

        public void ReParseBinding()
        {
            Override?.ParseBinding(gameObject, true);
            BehaviourTree?.ParseAllBindable(gameObject, true);
        }

        public void LogVariables()
        {
            if (Override != null)
            {
                foreach (var item in Override.Table)
                {
                    if (item is IBindingParseable parseable)
                    {
                        parseable.DebugParseResult();
                    }
                    else
                    {
                        Debug.Log(item);
                    }
                }
            }

            if (BehaviourTree != null)
            {
                foreach (var item in BehaviourTree.Variable.Table)
                {
                    if (item is IBindingParseable parseable)
                    {
                        parseable.DebugParseResult();
                    }
                    else
                    {
                        Debug.Log(item);
                    }
                }
            }
        }

        public VariableTable Override = new();

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


