using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class SubTree : BTActionNode, IDetailable, IBuildContextualMenuable, ISubtreeTreeElement, IBindingParseable
    {
        public BehaviorTreeAsset_1_1 BehaviorTreeAsset;

        [field: NonSerialized]
        public BehaviorTree BehaviourTree { get; set; }

        protected override Status OnTick(BTNode from)
        {
            if (BehaviourTree == null)
            {
                BehaviourTree = Tree.InstantiateSubTree(BehaviorTreeAsset, this);
                BehaviourTree.BindAgent(Tree.Agent);
                BehaviourTree.ParseAllBindable(Tree.Agent);
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

        public void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            bool hasAsset = BehaviorTreeAsset;
            evt.menu.AppendAction("EditorTree",
                a => EditorTree(),
                hasAsset ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

            evt.menu.AppendSeparator();
        }

        protected void EditorTree()
        {

#if UNITY_EDITOR
            if (BehaviorTreeAsset)
            {
                UnityEditor.AssetDatabase.OpenAsset(BehaviorTreeAsset);
            }
#endif

            if (Application.isPlaying && BehaviourTree != null)
            {
                BehaviorTreeManager.TreeDebugger?.AddDebugInstanceTree(BehaviourTree);
            }
        }

        object ISubtreeTreeElement.TreeAsset => BehaviorTreeAsset;

        public override void BindAgent(object agent)
        {
            base.BindAgent(agent);
            BehaviourTree?.BindAgent(agent);
        }

        public ParseBindingResult ParseBinding(object bindInstance, bool force = false)
        {
            BehaviourTree?.ParseAllBindable(Agent);
            return ParseBindingResult.Both;
        }

        public string DebugParseResult()
        {
            return "";
        }
    }
}
