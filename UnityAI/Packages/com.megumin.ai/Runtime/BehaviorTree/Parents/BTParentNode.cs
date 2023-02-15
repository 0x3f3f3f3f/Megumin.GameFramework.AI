using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BTParentNode : BTNode
    {
        [HideInInspector]
        public List<BTNode> children = new();
    }

    public class CompositeNode : BTParentNode
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
                    item.Abort();
                }
            }
        }
    }

    public class OneChildNode : BTParentNode
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
            Child0.Abort();
        }
    }
}
