using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System;
using UnityEditor;
using Megumin.GameFramework.AI.Editor;
using System.Linq;
using PlasticPipe.PlasticProtocol.Server.Stubs;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeView : GraphView, IDisposable
    {
        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }


        public const string StartNodeClass = "startNode";
        public BehaviorTreeEditor EditorWindow { get; internal set; }
        private CreateNodeSearchWindowProvider createNodeMenu;

        public FloatingTip FloatingTip;

        public BehaviorTreeView()
        {
            GridBackground background = new GridBackground();
            Insert(0, background);

            FloatingTip = new FloatingTip(this);
            Add(FloatingTip);


            var m = new MouseMoveManipulator();
            m.mouseMove += OnMouseMove;
            this.AddManipulator(m);
            this.AddManipulator(new ContentDragger());
            //this.AddManipulator(new DoubleClickSelection());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.SetupZoom(0.2f, 4f, ContentZoomer.DefaultScaleStep * 0.75f, ContentZoomer.DefaultReferenceScale);
            //SetupZoom 会自动添加ContentZoomer，手动添加会导致maxScale无效。
            //this.AddManipulator(new ContentZoomer());

            MiniMap child = new MiniMap();
            child.name = "minimap";
            this.AddElement(child);

            createNodeMenu = ScriptableObject.CreateInstance<CreateNodeSearchWindowProvider>();
            createNodeMenu.Initialize(this);

            nodeCreationRequest = (c) => SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), createNodeMenu);

            Debug.Log("BehaviorTreeView += ReloadView | OnGraphViewChanged...");
            Undo.undoRedoPerformed += ReloadView;
            graphViewChanged += OnGraphViewChanged;
            serializeGraphElements += OnSerializeGraphElements;
            canPasteSerializedData += OnCanPasteSerializedData;
            unserializeAndPaste += OnUnserializeAndPaste;
        }

        public void Dispose()
        {
            Debug.Log("BehaviorTreeView -= ReloadView | OnGraphViewChanged...");
            Undo.undoRedoPerformed -= ReloadView;
            graphViewChanged -= OnGraphViewChanged;
            serializeGraphElements -= OnSerializeGraphElements;
            canPasteSerializedData -= OnCanPasteSerializedData;
            unserializeAndPaste -= OnUnserializeAndPaste;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (!GraphViewReloadingScope.IsEnter)
            {
                if (graphViewChange.elementsToRemove != null)
                {
                    foreach (var item in graphViewChange.elementsToRemove)
                    {
                        if (item is BehaviorTreeNodeView nodeView)
                        {
                            RemoveNodeAndView(nodeView);
                        }

                        if (item is Edge edge)
                        {
                            if (edge.input.node is BehaviorTreeNodeView childNodeView &&
                            edge.output.node is BehaviorTreeNodeView parentNodeView)
                            {
                                Debug.Log($"Remove Edge {edge.input.node.name}");
                                DisconnectChild(parentNodeView, childNodeView);
                            }
                        }
                    }
                }

                if (graphViewChange.edgesToCreate != null)
                {
                    foreach (var edge in graphViewChange.edgesToCreate)
                    {
                        if (edge.input.node is BehaviorTreeNodeView childNodeView &&
                            edge.output.node is BehaviorTreeNodeView parentNodeView)
                        {
                            Debug.Log($"Create Edge {edge.input.node.name}");
                            ConnectChild(parentNodeView, childNodeView);
                        }
                    }
                }
            }

            return graphViewChange;
        }

        public void OnMouseMove(MouseMoveEvent evt)
        {
            var newPos = evt.localMousePosition;
            if (evt.localMousePosition.x > this.worldBound.width - FloatingTip.worldBound.width - 20)
            {
                newPos.x -= FloatingTip.worldBound.width + 20;
            }
            else
            {
                newPos.x += 20;
            }

            if (evt.localMousePosition.y > this.worldBound.height - FloatingTip.worldBound.height - 20)
            {
                newPos.y -= FloatingTip.worldBound.height + 20;
            }
            else
            {
                newPos.y += 20;
            }

            FloatingTip.transform.position = newPos;
            var graphMousePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            FloatingTip.MousePosTip.text = $"localPos:{evt.localMousePosition}    \ngraphPos:{graphMousePosition}";
        }

        ///不采用TheKiwiCoder 中的方式，Undo/Redo 时不能显示每一步操作名字。
        //SerializedObject treeSO;
        public TreeWapper SOTree;
        /// <summary>
        /// 当前TreeView正在显示的tree版本,用于控制UndoRedo时，是否重新加载整个View。
        /// </summary>
        public int LoadVersion;

        internal Scope GraphViewReloadingScope = new Scope();
        public void ReloadView()
        {
            ReloadView(false);
        }

        public void ReloadView(bool force)
        {
            using var s = GraphViewReloadingScope.Enter();
            using var undom = UndoMute.Enter("ReloadView");

            if (force == false && LoadVersion == SOTree?.ChangeVersion)
            {
                Debug.Log("没有实质性改动，不要ReloadView");
                return;
            }

            if (Tree == null)
            {
                if (EditorWindow.CurrentAsset)
                {
                    Tree = EditorWindow.CurrentAsset.CreateTree();
                }
                else
                {
                    return;
                }
            }

            this.LogFuncName();
            DeleteElements(graphElements.ToList().Where(elem => elem is BehaviorTreeNodeView || elem is Edge));

            if (!SOTree)
            {
                SOTree = ScriptableObject.CreateInstance<TreeWapper>();
                SOTree.Tree = Tree;
            }

            foreach (var node in Tree.AllNodes)
            {
                var nodeViwe = CreateNodeView(node);

                //RedoUndo后node对象不是原来那个对象，暂时没找到原因。可能时ScriptObject重新反序列化导致的。
                //这里用GUID判断
                if (node.GUID == Tree.StartNode?.GUID)
                {
                    nodeViwe.AddToClassList(StartNodeClass);
                }

                this.AddElement(nodeViwe);
            }

            //连接View父子关系
            foreach (var node in Tree.AllNodes)
            {
                if (node is BTParentNode parentNode)
                {
                    var view = GetNodeByGuid(node.GUID) as BehaviorTreeNodeView;
                    foreach (var child in parentNode.children)
                    {
                        var childview = GetNodeByGuid(child.GUID) as BehaviorTreeNodeView;
                        childview.ConnectParentNodeView(view);
                    }
                }
            }

            EditorWindow.UpdateHasUnsavedChanges();
            LoadVersion = SOTree.ChangeVersion;
        }

        public Vector2 LastContextualMenuMousePosition = Vector2.one * 100;
        public BehaviorTree Tree;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            LastContextualMenuMousePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            //Debug.Log(LastContextualMenuMousePosition);

            evt.menu.AppendAction("Test", Test, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();

            if (evt.target is BehaviorTreeNodeView nodeView)
            {
                nodeView.BuildContextualMenuBeforeBase(evt);
            }

            base.BuildContextualMenu(evt);

            if (evt.target is BehaviorTreeNodeView nodeViewAfter)
            {
                nodeViewAfter.BuildContextualMenuAfterBase(evt);
            }
        }

        private void Test(DropdownMenuAction obj)
        {
            this.LogFuncName(obj);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            //Debug.Log(startPort);
            return ports.ToList();
            //return base.GetCompatiblePorts(startPort, nodeAdapter);
        }

        public TreeWapper CreateTreeSOTreeIfNull()
        {
            if (Tree == null)
            {
                Debug.Log("new tree");
                Tree = new BehaviorTree();
            }

            if (!SOTree)
            {
                SOTree = new TreeWapper();
                SOTree.Tree = Tree;
            }

            return SOTree;
        }

        public BehaviorTreeNodeView AddNodeAndView(Type type, Vector2 graphMousePosition)
        {
            UndoRecord($"AddNode  [{type.Name}]");
            var node = Tree.AddNewNode(type);
            if (node.Meta == null)
            {
                node.Meta = new NodeMeta();
                node.Meta.x = graphMousePosition.x;
                node.Meta.y = graphMousePosition.y;
            }
            var nodeView = CreateNodeView(node);
            this.AddElement(nodeView);
            return nodeView;
        }

        public bool RemoveNodeAndView(BehaviorTreeNodeView nodeView)
        {
            if (nodeView?.SONode?.Node == null)
            {
                return false;
            }

            if (SOTree?.Tree == null)
            {
                return false;
            }

            UndoRecord($"RemoveNode  [{nodeView.SONode.Node.GetType().Name}]");
            RemoveElement(nodeView);
            return Tree.RemoveNode(nodeView.SONode.Node);
        }

        public BehaviorTreeNodeView PasteNodeAndView(BehaviorTreeNodeView origbalNodeView, Vector2 offset)
        {
            if (origbalNodeView?.SONode?.Node == null)
            {
                return null;
            }

            return AddNodeAndView(origbalNodeView?.SONode?.Node.GetType(), LastContextualMenuMousePosition + offset);
        }

        //public BehaviorTreeNodeView CreateNodeView()
        //{
        //    return CreateNodeView(null, LastContextualMenuMousePosition);
        //}

        public BehaviorTreeNodeView CreateNodeView(BTNode node)
        {
            var nodeView = new BehaviorTreeNodeView() { name = "behaviorTreeNode" };
            nodeView.TreeView = this;
            nodeView.SetNode(node);
            //node.capabilities |= Capabilities.Movable;
            if (node?.Meta != null)
            {
                nodeView.SetPosition(new Rect(node.Meta.x, node.Meta.y, 100, 100));
            }
            else
            {
                nodeView.SetPosition(new Rect(LastContextualMenuMousePosition.x, LastContextualMenuMousePosition.y, 100, 100));
            }

            //node.AddToClassList("debug");
            return nodeView;
        }

        internal void InspectorShowWapper()
        {
            if (SOTree)
            {
                Selection.activeObject = SOTree;
            }
            else
            {
                Debug.Log("no tree");
            }
        }

        internal void SetStartNode(BehaviorTreeNodeView behaviorTreeNodeView)
        {
            if (behaviorTreeNodeView?.SONode?.Node == null
                || behaviorTreeNodeView.SONode.Node == Tree.StartNode)
            {
                return;
            }

            if (Tree.StartNode != null)
            {
                var oldStartNodeView = GetNodeByGuid(Tree.StartNode.GUID);
                if (oldStartNodeView != null)
                {
                    oldStartNodeView.RemoveFromClassList(StartNodeClass);
                }
            }

            this.LogFuncName();
            UndoRecord("Change Start Node");
            Tree.StartNode = behaviorTreeNodeView.SONode.Node;
            behaviorTreeNodeView.AddToClassList(StartNodeClass);
        }
    }

    public class TreeWapper : ScriptableObject
    {
        public BehaviorTree Tree;
        public int ChangeVersion;

    }
}



