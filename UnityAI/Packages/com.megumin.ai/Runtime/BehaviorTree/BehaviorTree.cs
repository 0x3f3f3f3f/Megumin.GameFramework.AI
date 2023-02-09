using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlasticGui.LaunchDiffParameters;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Serializable]
    public class BehaviorTree
    {
        public virtual void Load() { }

        public readonly Dictionary<string, object> locDic = new Dictionary<string, object>();
        public BTNode StartNode { get; set; }
        public BehaviorTreeAsset Asset { get; internal set; }

        [SerializeReference]
        public List<BTNode> AllNodes = new();

        public Dictionary<string, BTNode> GuidDic { get; } = new();

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

        public BTNode AddNode(BTNode node)
        {
            AllNodes.Add(node);
            GuidDic[node.GUID] = node;
            return node;
        }
        public bool RemoveNode(BTNode node)
        {
            GuidDic.Remove(node.GUID);
            return AllNodes.Remove(node);
        }


        public T AddNode<T>() where T : BTNode, new()
        {
            var node = new T();
            node.GUID = Guid.NewGuid().ToString();
            node.InstanceID = Guid.NewGuid().ToString();
            AddNode(node);
            return node;
        }

        internal BTNode AddNewNode(Type type)
        {
            if (type.IsSubclassOf(typeof(BTNode)))
            {
                var node = Activator.CreateInstance(type) as BTNode;
                if (node != null)
                {
                    node.GUID = Guid.NewGuid().ToString();
                    AddNode(node);
                }
                return node;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public BTNode GetNodeByGuid(string guid)
        {
            if (GuidDic.TryGetValue(guid, out var node))
            {
                return node;
            }
            return default;
        }

        public bool TryGetNodeByGuid(string guid, out BTNode node)
        {
            return GuidDic.TryGetValue(guid, out node);
        }

        public bool TryGetNodeByGuid<T>(string guid, out T node)
            where T : BTNode
        {
            if (GuidDic.TryGetValue(guid, out var tempNode))
            {
                if (tempNode is T castNode)
                {
                    node = castNode;
                    return true;
                }
            }
            node = null;
            return false;
        }
    }
}
