using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 在其他行为树模型中，也可以称为装饰器节点。
    /// 为了避免和附加的装饰器混淆，这里不命名为装饰器。
    /// </summary>
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



    public abstract class OneChildNode<T> : BTParentNode<T>
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
            if (Child0?.State == Status.Running)
            {
                Child0?.Abort(this, options);
            }
        }
    }
}




