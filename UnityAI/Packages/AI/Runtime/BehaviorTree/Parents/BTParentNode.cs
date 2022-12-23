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
        public int current { get; protected set; } = 0;
        public List<BTNode> children = new List<BTNode>();

        protected override void OnEnter()
        {
            base.OnEnter();
            current = 0;
        }
    }

    public class OneChildNode : BTParentNode
    {
        public BTNode child;
    }
}
