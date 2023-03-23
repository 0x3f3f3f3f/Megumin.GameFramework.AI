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

        public bool EnableLog = true;
        bool ILogSetting.Enabled => EnableLog;

        private void InitTree()
        {
            if (BehaviourTree == null && BehaviorTreeAsset)
            {
                BehaviourTree = BehaviorTreeAsset.Instantiate();
                BehaviourTree.LogSetting = this;
                BehaviourTree.Init(gameObject);
            }
        }

        private void OnEnable()
        {
            if (AutoEnable)
            {
                InitTree();
                EnableTree();
            }
        }

        // Use this for initialization
        void Start()
        {
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

        public void EnableTree()
        {
            BehaviorTreeManager.Instance.AddTree(this);
        }

        public int CompareTo(BehaviorTreeRunner other)
        {
            return Order.CompareTo(other.Order);
        }

        public void TickTree()
        {
            BehaviourTree.Tick();
        }

    }
}