using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public abstract class CompositeNode : BTParentNode, IDynamicable
    {
        /// <summary>
        /// 条件终止 动态模式
        /// </summary>
        [field: Tooltip("It is recommended to use AbortType instead of Dynamic.")]
        [field: SerializeField]
        public bool Dynamic { get; set; } = false;

        public int CurrentIndex { get; protected set; } = -1;

        protected override void OnEnter(BTNode from, object options = null)
        {
            CurrentIndex = 0;
        }

        protected override Status OnEnter2(BTNode from, object options = null)
        {
            ///没有子节点时，忽略当前节点。
            if (Children.Count == 0)
            {
                return GetIgnoreResult(from);
            }
            return State;
        }

        protected override void OnAbort(BTNode from, object options = null)
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

    public abstract class CompositeNode<T> : BTParentNode<T>
    {
        public int CurrentIndex { get; protected set; } = -1;

        protected override void OnEnter(BTNode from, object options = null)
        {
            CurrentIndex = 0;
        }

        protected override Status OnEnter2(BTNode from, object options = null)
        {
            ///没有子节点时，忽略当前节点。
            if (Children.Count == 0)
            {
                return GetIgnoreResult(from);
            }
            return State;
        }

        protected override void OnAbort(BTNode from, object options = null)
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
}



