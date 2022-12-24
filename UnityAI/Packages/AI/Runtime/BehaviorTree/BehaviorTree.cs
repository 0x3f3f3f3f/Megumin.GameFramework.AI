using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlasticGui.LaunchDiffParameters;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BehaviorTree
    {
        public virtual void Load() { }

        public readonly Dictionary<string,object> locDic = new Dictionary<string, object>();
        public BTNode StartNode { get; set; }
        public List<BTNode> AllNodes = new List<BTNode>();
        private Status treestate = Status.Init;

        public void Reset()
        {
            treestate = Status.Init;
        }

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

            // Start在第一次Tick时调用一次
            //foreach (var item in AllNodes)
            //{
            //    if (item.Enabled && item.IsStarted)
            //    {
            //        item.Start();
            //    }
            //}
        }

        /// <summary>
        /// Todo 抽象出runner ，分别 root - leaf 驱动，last leaf， 异步。三种方式根调用不一样。但是都需要Tick。
        /// </summary>
        /// <returns></returns>
        public Status Tick()
        {
            if (treestate == Status.Succeeded || treestate == Status.Failed)
            {
                //整个树已经执行完，不在执行
            }
            else
            {
                if (StartNode.Enabled == false)
                {
                    Debug.Log($"StartNode is not Enabled!");
                    return Status.Failed;
                }

                if (StartNode.State != Status.Running)
                {
                    //已经运行的节点不在检查
                    var enterType = StartNode.CanEnter();
                    if (enterType == EnterType.False)
                    {
                        return Status.Failed;
                    }

                    if (enterType == EnterType.Ignore)
                    {
                        Debug.Log($"StartNode is Ignore");
                        return Status.Failed;
                    }
                }

                treestate = StartNode.Tick();
                if (treestate == Status.Succeeded || treestate == Status.Failed)
                {
                    Debug.Log($"tree complate. {treestate}");
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
            LoadLast();
        }

        private void LoadLast()
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            var loop = new Loop();
            seq.Derators = new object[] { loop };
            //var loop = new Repeater();
            //loop.child = seq;

            var check = new CheckBool();
            var remap = new Remap();
            log.Derators = new object[] { check, remap };
            StartNode = seq;
        }

        private void Load4()
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            var loop = new Repeater();
            loop.child = seq;

            var check = new CheckBool();
            var remap = new Remap();
            log.Derators = new object[] { check, remap };
            StartNode = loop;
        }

        private void Load3()
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            var loop = new Repeater();
            loop.child = seq;

            var check = new CheckBool();
            log.Derators = new object[] { check };
            StartNode = loop;
        }

        private void Load2()
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            var loop = new Repeater();
            loop.child = seq;

            StartNode = loop;
        }

        private void Load1()
        {
            var wait = new Wait();
            var log = new Log();
            var seq = new Sequence();
            seq.children.Add(wait);
            seq.children.Add(log);

            StartNode = seq;
        }
    }
}
