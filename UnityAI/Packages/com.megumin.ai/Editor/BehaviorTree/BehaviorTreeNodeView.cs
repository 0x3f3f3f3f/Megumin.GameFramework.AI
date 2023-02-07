using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System;
using UnityEditor;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreeNodeView : Node
    {

        public new class UxmlFactory : UxmlFactory<BehaviorTreeNodeView, UxmlTraits> { }

        /// <summary>
        /// 没办法，基类只接受路径。
        /// </summary>
        public BehaviorTreeNodeView()
            : base(AssetDatabase.GetAssetPath(Resources.Load<VisualTreeAsset>("BehaviorTreeNodeView")))
        {
            UseDefaultStyling();
            StyleSheet styleSheet = Resources.Load<StyleSheet>("BehaviorTreeNodeView");
            styleSheets.Add(styleSheet);
        }


        public NodeWapper NodeWapperSO;

        public BehaviorTreeView TreeView { get; internal set; }

        public override void Select(VisualElement selectionContainer, bool additive)
        {
            base.Select(selectionContainer, additive);
            Debug.Log(title);
            if (NodeWapperSO)
            {
                Selection.activeObject = NodeWapperSO;
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
        }

        internal void SetNode(BTNode node)
        {
            if (node == null)
            {
                title = "TestNode";
                name = "testNode";
            }
            else
            {
                var type = node.GetType();
                title = type.Name;
                NodeWapperSO = new NodeWapper();
                NodeWapperSO.Node = node;
                viewDataKey = node.GUID;
            }

            var inport = Port.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(int));
            var outport = Port.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(int));
            inputContainer.Add(inport);
            outputContainer.Add(outport);
        }

        internal void BuildContextualMenuBeforeBase(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("TestNode1", a => { }, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Set Start", a => SetStart(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();
        }

        internal void BuildContextualMenuAfterBase(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("TestNode2", a => { }, DropdownMenuAction.AlwaysEnabled);
        }

        public void SetStart()
        {
            TreeView.SetStartNode(this);
        }
    }

    public class NodeWapper : ScriptableObject
    {
        [SerializeReference]
        public BTNode Node;
    }
}
