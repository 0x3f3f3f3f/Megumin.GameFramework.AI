using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System;
using UnityEditor;
using System.ComponentModel;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeNodeView : Node
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
            this.AddToClassList("behaviorTreeNode");
        }


        public NodeWapper SONode;

        public BehaviorTreeView TreeView { get; internal set; }

        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }

        public override void Select(VisualElement selectionContainer, bool additive)
        {
            base.Select(selectionContainer, additive);
            Debug.Log(title);
            if (SONode)
            {
                Selection.activeObject = SONode;
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            if (SONode.Node.Meta == null)
            {
                SONode.Node.Meta = new NodeMeta();
            }

            //this.LogFuncName();
            TreeView.UndoRecord($"SetPosition    [{SONode.Node.GetType().Name}]");
            SONode.Node.Meta.x = newPos.x;
            SONode.Node.Meta.y = newPos.y;

            //父节点重新排序
            foreach (var edge in InputPort.connections)
            {
                if (edge.output.node is BehaviorTreeNodeView nodeView
                    && nodeView.SONode?.Node is BTParentNode parentNode)
                {
                    TreeView.SortChild(parentNode);
                }
            }
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
                SONode = ScriptableObject.CreateInstance<NodeWapper>();
                SONode.View = this;
                SONode.Node = node;
                SONode.name = type.Name;
                viewDataKey = node.GUID;
                this.AddToClassList(type.Name);
            }

            InputPort = Port.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(byte));

            Port.Capacity multiOutputPort = node is OneChildNode ? Port.Capacity.Single : Port.Capacity.Multi;
            OutputPort = Port.Create<Edge>(Orientation.Vertical, Direction.Output, multiOutputPort, typeof(byte));

            inputContainer.Add(InputPort);
            outputContainer.Add(OutputPort);

            if (node is ActionTaskNode actionTaskNode)
            {
                outputContainer.AddToClassList("unDisplay");
            }
            else
            {
                outputContainer.RemoveFromClassList("unDisplay");
            }
        }

        internal void BuildContextualMenuBeforeBase(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("TestNode1", a => { }, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Set Start", a => SetStart(), GetSetStartStatus);
            evt.menu.AppendSeparator();

            evt.menu.AppendAction("Open Node Script", a => OpenNodeScript(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Open Node View Script", a => OpenNodeViewScript(), DropdownMenuAction.AlwaysDisabled);
            evt.menu.AppendSeparator();
        }

        public DropdownMenuAction.Status GetSetStartStatus(DropdownMenuAction arg)
        {
            if (SONode?.Node == null)
            {
                return DropdownMenuAction.Status.Disabled;
            }

            var isStart = TreeView?.SOTree?.Tree.IsStartNodeByGuid(SONode.Node.GUID) ?? false;
            if (isStart)
            {
                return DropdownMenuAction.Status.Checked | DropdownMenuAction.Status.Disabled;
            }
            else
            {
                return DropdownMenuAction.Status.Normal;
            }
        }

        private void OpenNodeScript()
        {
            //Todo Cache /unity background tasks
            var scriptGUIDs = AssetDatabase.FindAssets($"t:script");

            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                var type = SONode.Node.GetType();
                var code = script.text;
                if (code.Contains($"class {type.Name}")
                    && code.Contains(type.Namespace))
                {
                    AssetDatabase.OpenAsset(script, 0, 0);
                }
            }
        }

        private void OpenNodeViewScript()
        {

        }

        internal void BuildContextualMenuAfterBase(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("TestNode2", a => { }, DropdownMenuAction.AlwaysEnabled);
        }

        public void SetStart()
        {
            TreeView.SetStartNode(this);
        }

        public Edge ConnectParentNodeView(BehaviorTreeNodeView parent)
        {
            return ConnectParentNodeView<Edge>(parent);
        }

        public T ConnectParentNodeView<T>(BehaviorTreeNodeView parent)
            where T : Edge, new()
        {
            var edge = InputPort.ConnectTo<T>(parent.OutputPort);
            TreeView.AddElement(edge);
            return edge;
        }
    }

    public class NodeWapper : ScriptableObject
    {
        [SerializeReference]
        public BTNode Node;

        public BehaviorTreeNodeView View { get; internal set; }

        [Editor]
        public void Test()
        {
            if (View.outputContainer.ClassListContains("unDisplay"))
            {
                View.outputContainer.RemoveFromClassList("unDisplay");
            }
            else
            {
                View.outputContainer.AddToClassList("unDisplay");
            }
        }
    }
}
