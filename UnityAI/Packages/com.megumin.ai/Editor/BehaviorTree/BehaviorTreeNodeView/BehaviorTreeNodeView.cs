using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System;
using UnityEditor;
using System.ComponentModel;
using Megumin.GameFramework.AI.Editor;
using System.Linq;

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

            decoratorContainer = this.Q<VisualElement>("decorator");
            //decoratorContainer.AddManipulator(new TestMouseManipulator());
            //为了屏蔽 装饰器内 框选误选中node。 组织事件向父级传播。
            //TODO，不知道为啥要同时停止MouseDownEvent  MouseUpEvent 才有效。
            decoratorContainer.AddManipulator(new StopPropagationMouseManipulator<MouseDownEvent, MouseUpEvent>());
            //decoratorContainer.RegisterCallback<MouseUpEvent>(evt =>
            //{
            //    this.LogMethodName("MouseUpEvent WillStop");
            //    evt.StopImmediatePropagation();
            //}, TrickleDown.NoTrickleDown);

            //decoratorContainer.RegisterCallback<MouseDownEvent>(evt =>
            //{
            //    this.LogMethodName("MouseDownEvent WillStop");
            //    //evt.StopPropagation();
            //    evt.StopImmediatePropagation();
            //}, TrickleDown.NoTrickleDown);

            DecoretorListView = decoratorContainer.Q<ListView>();
            DecoretorListView.makeItem += ListViewMakeDecoratorView;
            DecoretorListView.bindItem += ListViewBindDecorator;
            DecoretorListView.onItemsChosen += DecoretorListView_onItemsChosen;
            DecoretorListView.itemIndexChanged += DecoretorListView_itemIndexChanged;



        }

        public NodeWrapper SONode;

        public BehaviorTreeView TreeView { get; internal set; }

        public Port InputPort { get; private set; }
        public Port OutputPort { get; private set; }
        public VisualElement decoratorContainer { get; }
        public ListView DecoretorListView { get; }
        public BTNode Node { get; private set; }

        public override void OnSelected()
        {
            //this.LogMethodName(title);
            base.OnSelected();
            if (SONode)
            {
                Selection.activeObject = SONode;
            }
        }

        public override void OnUnselected()
        {
            //this.LogMethodName(title);
            base.OnUnselected();

            //取消选中时保留显示

            //this.LogMethodName(title);
            //if (Selection.activeObject == SONode)
            //{
            //    Selection.activeObject = null;
            //}
        }

        public override void SetPosition(Rect newPos)
        {
            //对位置取整，不然保存的时候会有小数。 会导致拖拽时抖动，放弃。
            //newPos = new Rect((int)newPos.x, (int)newPos.y, newPos.width,newPos.height);

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


        public NodeWrapper CreateSOWrapperIfNull(BTNode node, bool forceRecreate = false)
        {
            var soWrapper = SONode;
            if (!soWrapper)
            {
                if (TreeView.NodeWrapperCache.TryGetValue(node.GUID, out var cacheWrapper))
                {
                    //创建新的SO对象在 Inpector锁定显示某个节点时，会出现无法更新的问题。
                    //尝试复用旧的SOWrapper

                    //Debug.Log("尝试复用旧的SOWrapper");
                    soWrapper = cacheWrapper;
                }
            }

            if (!soWrapper || forceRecreate)
            {
                soWrapper = this.CreateSOWrapper<NodeWrapper>();
                TreeView.NodeWrapperCache[node.GUID] = soWrapper;
            }
            return soWrapper;
        }

        public const string StartNodeClass = "startNode";

        internal void SetNode(BTNode node, bool fakeNode = false)
        {
            viewDataKey = node?.GUID;
            if (fakeNode)
            {
                Node = node;
            }
            ReloadView(false, fakeNode);
        }

        public void CreatePort(BTNode node)
        {
            if (InputPort == null)
            {
                InputPort = new BehaviorTreePortView(Direction.Input, Port.Capacity.Single);
                inputContainer.Add(InputPort);
            }

            if (OutputPort == null)
            {
                Port.Capacity multiOutputPort = node is OneChildNode ? Port.Capacity.Single : Port.Capacity.Multi;
                OutputPort = new BehaviorTreePortView(Direction.Output, multiOutputPort);
                outputContainer.Add(OutputPort);
            }

            outputContainer.SetToClassList("unDisplay", node is ActionTaskNode);
        }

        /// <summary>
        /// 因为UndoRedo时内存实例对象会改变，所以每次通过guid取得新的实例。
        /// </summary>
        /// <param name="forceReCreateSoWrapper"></param>
        public virtual void ReloadView(bool forceReCreateSoWrapper = false, bool fakeNode = false)
        {
            //清除旧的class typeName
            RemoveFromClassList(Node?.GetType().Name ?? "NullNode");

            var node = Node;
            if (fakeNode)
            {
                //测试用NodeView 不要从Tree实例中获取节点。因为不存在。
            }
            else
            {
                node = TreeView?.Tree?.GetNodeByGuid(viewDataKey);
                this.Node = node;
            }

            var type = node?.GetType();
            var typeName = type?.Name ?? "NullNode";
            title = typeName;
            name = typeName;
            SONode = CreateSOWrapperIfNull(node, forceReCreateSoWrapper);

            SONode.View = this;
            SONode.Node = node;
            SONode.name = typeName;

            RefreshDecoratorListView();

            //是不是开始节点
            var isStartNode = TreeView.SOTree?.Tree?.IsStartNodeByGuid(node.GUID) ?? false;
            this.SetToClassList(StartNodeClass, isStartNode);

            var houdai = TreeView.SOTree?.Tree?.IsStartNodeDescendant(node) ?? false;
            //未连接的节点，保存但是运行时没有作用。
            this.SetToClassList("notConnected", !houdai && !isStartNode);

            CreatePort(node);
            AddToClassList(typeName);

            UpdateNodeType();
        }

        void UpdateNodeType()
        {
            RemoveFromClassList(nameof(ActionTaskNode));
            RemoveFromClassList(nameof(BTParentNode));

            switch (SONode?.Node)
            {
                case ActionTaskNode _:
                    AddToClassList(nameof(ActionTaskNode));
                    break;
                case BTParentNode _:
                    AddToClassList(nameof(BTParentNode));
                    break;
                default:
                    break;
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //this.LogMethodName(evt.ToStringReflection(), "\n", evt.triggerEvent.ToStringReflection());

            evt.menu.AppendAction("TestNode1", a => { }, DropdownMenuAction.AlwaysEnabled);
            //this.LogMethodName();
            base.BuildContextualMenu(evt);

            evt.menu.AppendAction("Open Node Script", a => OpenNodeScript(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Open Node View Script", a => OpenNodeViewScript(), DropdownMenuAction.AlwaysDisabled);
            evt.menu.AppendSeparator();

            evt.menu.AppendAction("Set Start", a => SetStart(), GetSetStartStatus);
            evt.menu.AppendSeparator();

            BuildContextualMenuDecorator(evt);
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

    public class NodeWrapper : ScriptableObject
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
