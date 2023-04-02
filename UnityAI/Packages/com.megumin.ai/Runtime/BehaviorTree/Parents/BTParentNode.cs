using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public abstract class BTParentNode : BTNode
    {
        /// <summary>
        /// 这里必须使用泛型序列化，否则Undo/Redo 时元素会丢失自己的真实类型。notconnect 多层级颜色bug
        /// </summary>
        [HideInInspector]
        [SerializeReference]
        public List<BTNode> children = new();

        /// <summary>
        /// 条件终止 动态模式
        /// </summary>
        [Tooltip("It is recommended to use AbortType instead of Dynamic.")]
        public bool Dynamic = false;

        public bool ContainsChild(BTNode node)
        {
            foreach (BTNode child in children)
            {
                if (child.GUID == node.GUID)
                {
                    return true;
                }
            }

            return false;
        }

        public bool AddChild(BTNode node)
        {
            if (ContainsChild(node))
            {
                return false;
            }
            children.Add(node);
            return true;
        }

        public bool RemoveChild(BTNode node)
        {
            if (ContainsChild(node))
            {
                children.RemoveAll(elem => elem.GUID == node.GUID);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 测试一个节点是不是自己的子代
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsDescendant(BTNode node, List<BTParentNode> parentPath = null)
        {
            if (node == null)
            {
                return false;
            }

            foreach (BTNode child in children)
            {
                if (child.GUID == node.GUID)
                {
                    parentPath?.Add(this);
                    return true;
                }

                if (child is BTParentNode parentNode)
                {
                    var result = parentNode.IsDescendant(node, parentPath);
                    if (result)
                    {
                        parentPath?.Add(this);
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public abstract class CompositeNode : BTParentNode
    {
        public int current { get; protected set; } = -1;

        protected override void OnEnter()
        {
            base.OnEnter();
            current = 0;
        }

        protected override void OnAbort()
        {
            foreach (var item in children)
            {
                if (item.State == Status.Running)
                {
                    item.Abort(this);
                }
            }
        }
    }

    public abstract class OneChildNode : BTParentNode
    {
        public BTNode Child0
        {
            get
            {
                if (children.Count > 0)
                {
                    return children[0];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (children.Count > 0)
                {
                    children[0] = value;
                }
                else
                {
                    children.Add(value);
                }
            }
        }

        protected override void OnAbort()
        {
            Child0.Abort(this);
        }
    }

    public abstract class TwoChildNode : BTParentNode
    {
        public BTNode Child0
        {
            get
            {
                if (children.Count > 0)
                {
                    return children[0];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (children.Count > 0)
                {
                    children[0] = value;
                }
                else
                {
                    children.Add(value);
                }
            }
        }

        public BTNode Child1
        {
            get
            {
                if (children.Count > 1)
                {
                    return children[1];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (children.Count > 1)
                {
                    children[1] = value;
                }
                else
                {
                    children.Insert(1, value);
                }
            }
        }

        protected override void OnAbort()
        {
            Child0.Abort(this);
        }
    }
}
