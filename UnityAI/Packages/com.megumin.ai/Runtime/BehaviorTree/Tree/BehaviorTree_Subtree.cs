using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public partial class BehaviorTree
    {
        public BehaviorTree RootTree { get; set; }
        public BehaviorTree ParentTree { get; set; }
        public BTNode ParentTreeHostNode { get; set; }

        public bool IsRootTree => ParentTreeHostNode == null;
        public bool IsSubtree => ParentTreeHostNode != null;

        public BehaviorTree InstantiateSubTree(IBehaviorTreeAsset behaviorTreeAsset, BTNode hostNode)
        {
            var subtree = behaviorTreeAsset.Instantiate(InitOption, RefFinder);
            subtree.ParentTree = this;
            subtree.RootTree = RootTree;
            subtree.ParentTreeHostNode = hostNode;
            subtree.RunOption = RunOption;
            subtree.InstanceName = InstanceName;
            return subtree;
        }
    }
}




