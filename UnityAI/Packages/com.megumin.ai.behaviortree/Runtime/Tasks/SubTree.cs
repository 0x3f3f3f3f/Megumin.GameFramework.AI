using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 子树节点
    /// 引用一个外部行为树，当作子节点执行。  
    /// 运行时右键EditorTree，即可进入子树debug功编辑器。
    /// </summary>
    [Category("Action")]
    [HelpURL(URL.WikiTask + "SubTree")]
    public class SubTree : BTActionNode, IDetailable, IBuildContextualMenuable, ISubtreeTreeElement, IBindingParseable
    {
        public BehaviorTreeAsset_1_1 BehaviorTreeAsset;

        [field: NonSerialized]
        public BehaviorTree BehaviourTree { get; set; }

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (BehaviourTree == null)
            {
                BehaviourTree = Tree.InstantiateSubTree(BehaviorTreeAsset, this);
                BehaviourTree.BindAgent(Tree.Agent);
                BehaviourTree.ParseAllBindable(Tree.Agent);
            }

            return BehaviourTree.Tick(from);
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
                BehaviorTreeManager.Debugger?.AddDebugInstanceTree(BehaviourTree);
            }
        }

        object ISubtreeTreeElement.TreeAsset => BehaviorTreeAsset;

        public override void BindAgent(object agent)
        {
            base.BindAgent(agent);
            BehaviourTree?.BindAgent(agent);
        }

        public CreateDelegateResult ParseBinding(object bindInstance, bool force = false, object options = null)
        {
            BehaviourTree?.ParseAllBindable(Agent, force, options);
            return CreateDelegateResult.Both;
        }

        public string DebugParseResult()
        {
            return "";
        }
    }
}
