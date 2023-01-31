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

        public readonly Dictionary<string, object> locDic = new Dictionary<string, object>();
        public BTNode StartNode { get; set; }
        public BehaviorTreeAsset Asset { get; internal set; }

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
                if (StartNode == null)
                {
                    return Status.Failed;
                }

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

        internal BTNode AddNode(BTNode node)
        {
            AllNodes.Add(node);
            return node;
        }

        public T AddNode<T>() where T : BTNode, new()
        {
            var node = new T();
            node.GUID = Guid.NewGuid().ToString();
            node.InstanceID= Guid.NewGuid().ToString();
            AllNodes.Add(node);
            return node;
        }
    }
}
