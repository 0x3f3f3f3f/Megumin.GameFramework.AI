using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public abstract class BTParentNode : BTNode
    {
        /// <summary>
        /// 这里必须使用泛型序列化，否则Undo/Redo 时元素会丢失自己的真实类型。notconnect 多层级颜色bug
        /// </summary>
        [HideInInspector]
        [SerializeReference]
        [FormerlySerializedAs("children")]
        public List<BTNode> Children = new();

        /// <summary>
        /// 条件终止 动态模式
        /// </summary>
        [Tooltip("It is recommended to use AbortType instead of Dynamic.")]
        public bool Dynamic = false;

        public bool ContainsChild(BTNode node)
        {
            foreach (BTNode child in Children)
            {
                if (child.GUID == node.GUID)
                {
                    return true;
                }
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

            foreach (BTNode child in Children)
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
        public int CurrentIndex { get; protected set; } = -1;

        protected override void OnEnter(object options = null)
        {
            CurrentIndex = 0;
        }

        protected override void OnAbort(object options = null)
        {
            foreach (var item in Children)
            {
                if (item.State == Status.Running)
                {
                    item.Abort(this, options);
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
                if (Children.Count > 0)
                {
                    return Children[0];
                }
                else
                {
                    return null;
                }
            }
        }

        protected override void OnAbort(object options = null)
        {
            Child0.Abort(this, options);
        }
    }

    public abstract class TwoChildNode : BTParentNode
    {
        public BTNode Child0
        {
            get
            {
                if (Children.Count > 0)
                {
                    return Children[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public BTNode Child1
        {
            get
            {
                if (Children.Count > 1)
                {
                    return Children[1];
                }
                else
                {
                    return null;
                }
            }
        }

        protected override void OnAbort(object options = null)
        {
            Child0.Abort(this, options);
        }
    }
}
