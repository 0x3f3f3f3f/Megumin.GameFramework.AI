using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTree
    {
        public virtual void Load() { }


        public TaskNode StartNode { get; set; }
        public List<TaskNode> AllNodes = new List<TaskNode>();
        private Status treestate = Status.Init;

        internal void Init(object agent)
        {
            foreach (var item in AllNodes)
            {
                item.Awake();
            }

            foreach (var item in AllNodes)
            {
                if (item.Enabled)
                {
                    item.Enable();
                }
            }

            foreach (var item in AllNodes)
            {
                if (item.Enabled && item.IsStarted)
                {
                    item.Start();
                }
            }
        }

        public Status Tick()
        {
            if (treestate == Status.Succeeded || treestate == Status.Failed)
            {
                //整个树已经执行完，不在执行
            }
            else
            {
                treestate = StartNode.Tick();
                if (treestate == Status.Succeeded || treestate == Status.Failed)
                {
                    Debug.Log($"tree complate.");
                }
            }

            return treestate;
        }
    }

    public class MyTestBehaviourTree : BehaviorTree
    {
        public override void Load()
        {
            base.Load();


            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            StartNode = seq;

        }


    }
}
