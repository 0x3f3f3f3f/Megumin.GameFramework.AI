using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTreeRunner : MonoBehaviour, IComparable<BehaviorTreeRunner>, ILogSetting
    {
        //[field: SerializeField]
        public BehaviorTree BehaviourTree { get; protected set; }
        public BehaviorTreeAsset_1_0_1 BehaviorTreeAsset;
        public TickMode TickMode = TickMode.Update;
        public int Order = 0;
        public bool AutoEnable = true;

        public bool EnableLog = false;
        bool ILogSetting.Enabled => EnableLog;

        private void OnEnable()
        {
            this.LogMethodName();
            if (AutoEnable)
            {
                EnableTree();
            }
        }

        private void OnDisable()
        {
            DisableTree();
        }

        public void EnableTree()
        {
            if (BehaviourTree == null && BehaviorTreeAsset)
            {
                BehaviourTree = BehaviorTreeAsset.Instantiate();
                BehaviourTree.LogSetting = this;
                BehaviourTree.Init(gameObject);
            }

            if (BehaviourTree != null)
            {
                BehaviorTreeManager.Instance.AddTree(this);
            }
        }

        public void DisableTree()
        {
            BehaviorTreeManager.Instance.RemoveTree(this);
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

        public int CompareTo(BehaviorTreeRunner other)
        {
            return Order.CompareTo(other.Order);
        }

        public void TickTree()
        {
            if (enabled)
            {
                BehaviourTree.Tick();
            }
        }

    }
}