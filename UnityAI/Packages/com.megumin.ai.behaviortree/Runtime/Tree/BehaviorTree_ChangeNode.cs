using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public partial class BehaviorTree
    {
        [Space]
        [SerializeReference]
        public List<BTNode> AllNodes = new();
        public Dictionary<string, BTNode> GuidDic { get; } = new();

        public BTNode AddNode(BTNode node)
        {
            if (AllNodes.Contains(node))
            {

            }
            else
            {
                version++;
                node.Tree = this;
                AllNodes.Add(node);
                GuidDic[node.GUID] = node;
            }

            return node;
        }

        public bool RemoveNode(BTNode node)
        {
            if (node == null)
            {
                return false;
            }

            if (AllNodes.Contains(node))
            {
                version++;
                GuidDic.Remove(node.GUID);
                AllNodes.Remove(node);
                if (node.Tree == this)
                {
                    node.Tree = null;
                }
                return true;
            }

            return false;
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

        /// <summary>
        /// 默认作为最后一个子节点，排序步骤在其他环节
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="child"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Connect(BTParentNode parentNode, BTNode child, int? index = null)
        {
            if (parentNode.ContainsChild(child))
            {
                return false;
            }

            version++;
            if (index.HasValue && index.Value >= 0)
            {
                parentNode.Children.Insert(index.Value, child);
            }
            else
            {
                parentNode.Children.Add(child);
            }

            AddNode(child);
            return true;
        }

        public bool Disconnect(BTParentNode parentNode, BTNode child)
        {
            if (parentNode.ContainsChild(child))
            {
                version++;
                parentNode.Children.RemoveAll(elem => elem.GUID == child.GUID);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获得子节点在父节点中的索引
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public int IndexOfParentNode(BTParentNode parentNode, BTNode child)
        {
            var index = 0;
            foreach (var node in parentNode.Children)
            {
                if (node.GUID == child.GUID)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public void OnChildIndexChanged(BTParentNode parentNode)
        {
            version++;
        }
    }
}


