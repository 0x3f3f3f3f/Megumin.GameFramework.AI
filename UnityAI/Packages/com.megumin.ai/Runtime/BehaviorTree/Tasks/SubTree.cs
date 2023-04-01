using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class SubTree : ActionTaskNode, IDetailable
    {
        public BehaviorTreeAsset_1_0_1 BehaviorTreeAsset;

        [field: NonSerialized]
        public BehaviorTree BehaviourTree { get; set; }

        protected override Status OnTick(BTNode from)
        {
            if (BehaviourTree == null)
            {
                BehaviourTree = Tree.InstantiateSubTree(BehaviorTreeAsset, this);
                BehaviourTree.BindAgent(Tree.Agent);
            }

            return BehaviourTree.TickSubTree(from);
        }

        public string GetDetail()
        {
            if (BehaviorTreeAsset)
            {
                return BehaviorTreeAsset.name;
            }
            else
            {
                return "Null";
            }
        }
    }
}
