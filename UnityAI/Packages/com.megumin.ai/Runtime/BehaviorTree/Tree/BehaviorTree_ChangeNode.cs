using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
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


        public bool Connect(BTParentNode parentNode, BTNode child)
        {
            if (parentNode.ContainsChild(child))
            {
                return false;
            }

            version++;
            parentNode.Children.Add(child);
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
    }
}


