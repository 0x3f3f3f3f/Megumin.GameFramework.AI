using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class BTParentNode : BTNode
    {

    }

    public class CompositeNode : BTParentNode
    {
        public int current { get; protected set; } = -1;
        public List<BTNode> children = new List<BTNode>();

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
        public BTNode child;

        protected override void OnAbort()
        {
            child.Abort();
        }
    }
}
