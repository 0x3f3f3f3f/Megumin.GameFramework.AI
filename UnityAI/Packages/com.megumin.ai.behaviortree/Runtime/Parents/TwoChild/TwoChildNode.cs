using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
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



    public abstract class TwoChildNode<T> : BTParentNode<T>
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




