using System;
using System.Collections.Generic;
using System.Linq;
using Megumin.GameFramework.AI.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeView : GraphView, IDisposable
    {
        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }

        public BehaviorTreeEditor EditorWindow { get; internal set; }
        internal CreateNodeSearchWindowProvider createNodeMenu;

        public FloatingTip FloatingTip;

        public BehaviorTreeView()
        {
            GridBackground background = new GridBackground();
            Insert(0, background);

            var m = new MouseMoveManipulator();
            m.mouseMove += OnMouseMove;
            this.AddManipulator(m);
            this.AddManipulator(new ContentDragger());
            //this.AddManipulator(new DoubleClickSelection());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.SetupZoom(0.2f, 5f, ContentZoomer.DefaultScaleStep * 0.75f, ContentZoomer.DefaultReferenceScale);
            //SetupZoom 会自动添加ContentZoomer，手动添加会导致maxScale无效。
            //this.AddManipulator(new ContentZoomer());

            CreateMinimap();

            CreateBlackboard();

            CreateFloatingTip();

            CreateUIBuilderDebugNode();

            createNodeMenu = ScriptableObject.CreateInstance<CreateNodeSearchWindowProvider>();
            createNodeMenu.Initialize(this);

            nodeCreationRequest = (c) =>
            {
                createNodeMenu.NextTaskSource = null;
                createNodeMenu.NextEdge = null;
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), createNodeMenu);
            };

            if (BehaviorTreeEditor.EditorLog)
            {
                Debug.Log("BehaviorTreeView += ReloadView | OnGraphViewChanged...");
            }

            Undo.undoRedoPerformed += ReloadView;
            graphViewChanged += OnGraphViewChanged;
            serializeGraphElements += OnSerializeGraphElements;
            canPasteSerializedData += OnCanPasteSerializedData;
            unserializeAndPaste += OnUnserializeAndPaste;
            viewTransformChanged += OnViewTransformChanged;
        }

        public void Dispose()
        {
            if (BehaviorTreeEditor.EditorLog)
            {
                Debug.Log("BehaviorTreeView -= ReloadView | OnGraphViewChanged...");
            }

            Undo.undoRedoPerformed -= ReloadView;
            graphViewChanged -= OnGraphViewChanged;
            serializeGraphElements -= OnSerializeGraphElements;
            canPasteSerializedData -= OnCanPasteSerializedData;
            unserializeAndPaste -= OnUnserializeAndPaste;
            DestoryCacheSOWrapper();
        }

        /// <summary>
        /// 在项目重新编译时销毁缓存SO。解决Inspector锁定时，显示错误的SO信息。
        /// </summary>
        internal void DestoryCacheSOWrapper()
        {
            UnityEngine.Object.DestroyImmediate(SOTree);
            foreach (var sonode in NodeWrapperCache)
            {
                UnityEngine.Object.DestroyImmediate(sonode.Value);
            }
            NodeWrapperCache.Clear();

            foreach (var sodecorator in DecoratorWrapperCache)
            {
                UnityEngine.Object.DestroyImmediate(sodecorator.Value);
            }
            DecoratorWrapperCache.Clear();
        }

        void CreateUIBuilderDebugNode()
        {
            if (Application.isPlaying)
            {
                return;
            }
            using var undom = UndoMute.Enter("CreateUIBuilderDebugNode");

            var testParent = new Sequence();
            testParent.GUID = Guid.NewGuid().ToString();
            testParent.AddDecorator<Loop>();
            testParent.AddDecorator<Cooldown>();

            var testParentView = CreateNodeView(testParent, true);
            //在UIBuilder中显示，在BehaviorTreeEditor中不显示。
            testParentView.AddToClassList("uiBuilderDebugNode");
            this.AddElement(testParentView);

            {
                var node2 = new Log();
                node2.Meta = new NodeMeta() { x = 100, y = 300 };
                node2.GUID = Guid.NewGuid().ToString();
                var nodeView = CreateNodeView(node2, true);
                //在UIBuilder中显示，在BehaviorTreeEditor中不显示。
                nodeView.AddToClassList("uiBuilderDebugNode");
                nodeView.AddToClassList("startNode");
                this.AddElement(nodeView);

                nodeView.ConnectParentNodeView(testParentView);
            }

            {
                var succeededTest = new Wait();
                succeededTest.Meta = new NodeMeta() { x = 300, y = 500 };
                succeededTest.GUID = Guid.NewGuid().ToString();
                var nodeView = CreateNodeView(succeededTest, true);
                //在UIBuilder中显示，在BehaviorTreeEditor中不显示。
                nodeView.AddToClassList("uiBuilderDebugNode");
                nodeView.AddToClassList(UssClassConst.succeeded);
                this.AddElement(nodeView);
                var edge = nodeView.ConnectParentNodeView(testParentView);
                edge.AddToClassList(UssClassConst.succeeded);
            }

            {
                var runningTest = new Wait();
                runningTest.Meta = new NodeMeta() { x = 500, y = 300 };
                runningTest.GUID = Guid.NewGuid().ToString();
                var nodeView = CreateNodeView(runningTest, true);
                //在UIBuilder中显示，在BehaviorTreeEditor中不显示。
                nodeView.AddToClassList("uiBuilderDebugNode");
                nodeView.AddToClassList(UssClassConst.running);
                this.AddElement(nodeView);
                var edge = nodeView.ConnectParentNodeView(testParentView);
                edge.AddToClassList(UssClassConst.running);
            }

            {
                var failedTest = new Wait();
                failedTest.Meta = new NodeMeta() { x = 700, y = 500 };
                failedTest.GUID = Guid.NewGuid().ToString();
                var nodeView = CreateNodeView(failedTest, true);
                //在UIBuilder中显示，在BehaviorTreeEditor中不显示。
                nodeView.AddToClassList("uiBuilderDebugNode");
                nodeView.AddToClassList(UssClassConst.failed);
                this.AddElement(nodeView);
                var edge = nodeView.ConnectParentNodeView(testParentView);
                edge.AddToClassList(UssClassConst.failed);
            }
        }

        public void CreateMinimap()
        {
            MiniMap miniMap = new();
            this.Add(miniMap);
            var size = new Vector2(96, 72);
            miniMap.SetPosition(new Rect(Vector2.zero, size));
        }

        public void CreateBlackboard()
        {
            Blackboard = new(this);
            this.Add(Blackboard);
        }

        public void CreateFloatingTip()
        {
            FloatingTip = new(this);
            Add(FloatingTip);
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

        private void OnViewTransformChanged(GraphView graphView)
        {
            //Debug.Log(contentViewContainer.transform.scale);
            if (BehaviorTreeEditor.FriendlyZoomSetting)
            {
                var level = contentViewContainer.transform.scale.x;
                this.SetToClassList(UssClassConst.zoomOut, level < 0.75f);
                this.SetToClassList(UssClassConst.zoomIn, level > 2.25f);
            }
            else
            {
                this.SetToClassList(UssClassConst.zoomOut, false);
                this.SetToClassList(UssClassConst.zoomIn, false);
            }
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
        public TreeWrapper SOTree;
        /// <summary>
        /// 当前TreeView正在显示的tree版本,用于控制UndoRedo时，是否重新加载整个View。
        /// </summary>
        public int LoadVersion;

        internal Scope GraphViewReloadingScope = new Scope();

        public TreeWrapper CreateSOWrapperIfNull(bool forceRecreate = false)
        {
            if (!SOTree || forceRecreate)
            {
                SOTree = this.CreateSOWrapper<TreeWrapper>();
            }
            return SOTree;
        }

        public TreeWrapper CreateTreeSOTreeIfNull()
        {
            SOTree = CreateSOWrapperIfNull();

            if (Tree == null)
            {
                Debug.Log("new tree");
                SOTree.Tree = new BehaviorTree();
                SOTree.Tree.GUID = Guid.NewGuid().ToString();
                Blackboard?.ReloadView(true);
            }

            return SOTree;
        }

        public void ReloadView()
        {
            ReloadView(false);
        }

        static readonly InitOption editorInitOption = new()
        {
            AsyncInit = false,
            SharedMeta = false,
            LazyInitSubtree = true,
            UseGenerateCode = false,
        };

        public void ReloadView(bool force)
        {
            EditorWindow.UpdateTitle();
            EditorWindow.UpdateHasUnsavedChanges();

            using var s = GraphViewReloadingScope.Enter();
            using var undom = UndoMute.Enter("ReloadView");

            if (force == false && LoadVersion == SOTree?.ChangeVersion)
            {
                Debug.Log("没有实质性改动，不要ReloadView");
                return;
            }

            CreateSOWrapperIfNull();
            if (force || Tree == null)
            {
                if (EditorWindow.IsDebugMode)
                {
                    SOTree.Tree = EditorWindow.DebugInstance;
                }
                else
                {
                    if (EditorWindow.CurrentAsset != null)
                    {
                        SOTree.Tree = EditorWindow.CurrentAsset.Instantiate(editorInitOption);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            SOTree.name = Tree.Asset?.name;

            if (BehaviorTreeEditor.EditorLog)
            {
                this.LogMethodName();
            }

            DeleteElements(graphElements.ToList().Where(elem => elem is BehaviorTreeNodeView || elem is Edge));

            //赋值viewDataKey 后会自动保存布局和缩放。
            viewDataKey = Tree.GUID;

            foreach (var node in Tree.AllNodes)
            {
                var nodeViwe = CreateNodeView(node);
                this.AddElement(nodeViwe);
            }

            //连接View父子关系
            foreach (var node in Tree.AllNodes)
            {
                if (node is BTParentNode parentNode)
                {
                    var view = GetNodeByGuid(node.GUID) as BehaviorTreeNodeView;
                    foreach (var child in parentNode.Children)
                    {
                        var childview = GetNodeByGuid(child.GUID) as BehaviorTreeNodeView;
                        childview.ConnectParentNodeView(view);
                    }
                }
            }

            //重载后WrappSONode 会重新创建，Inspector显示的对象已经过时。
            //if (Selection.activeObject is NodeWrapper oldActiveNode)
            //{
            //    var view = GetNodeByGuid(oldActiveNode.Node.GUID) as BehaviorTreeNodeView;
            //    Selection.activeObject = view.SONode;
            //}

            //for (int i = 0; i < Selection.objects.Length; i++)
            //{
            //    var obj = Selection.objects[i];
            //    if (obj is NodeWrapper oldNode)
            //    {
            //        var view = GetNodeByGuid(oldNode.Node.GUID) as BehaviorTreeNodeView;
            //        Selection.objects[i] = view.SONode;
            //    }
            //}

            Blackboard?.ReloadView(force);

            //this.RepaintInspectorWindows();

            LoadVersion = SOTree.ChangeVersion;
        }

        public void ReloadAllNodeView()
        {
            foreach (var item in graphElements)
            {
                if (item is BehaviorTreeNodeView nodeView)
                {
                    nodeView.ReloadView();
                }
            }
        }

        /// <summary>
        /// 延迟一下在FrameAll，可能调用是布局还没有计算完完成
        /// </summary>
        internal async void DelayFrameAll()
        {
            await this.Delay(15);
            FrameAll();
        }

        public Vector2 LastContextualMenuMousePosition = Vector2.one * 100;
        public BehaviorTree Tree => SOTree?.Tree;

        public BehaviorTreeBlackboardView Blackboard { get; private set; }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            LastContextualMenuMousePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);

            //this.LogMethodName(LastContextualMenuMousePosition,panel,this.panel.contextualMenuManager);
            //this.LogMethodName(evt.ToStringReflection(), "\n", evt.triggerEvent.ToStringReflection());

            base.BuildContextualMenu(evt);

            //if (nearAddNodeType.Count > 0)
            //{
            //    for (int i = nearAddNodeType.Count - 1; i >= 0; i--)
            //    {
            //        var type = nearAddNodeType[i];
            //        evt.menu.AppendAction($"Create Node/{type.Name}", a => AddNodeAndView(type), DropdownMenuAction.AlwaysEnabled);
            //    }
            //    evt.menu.AppendSeparator();
            //}
        }

        private void Test(DropdownMenuAction obj)
        {
            this.LogMethodName(obj);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            //Debug.Log(startPort);
            return ports.ToList();
            //return base.GetCompatiblePorts(startPort, nodeAdapter);
        }

        /// <summary>
        /// 最近添加的node
        /// </summary>
        static List<Type> nearAddNodeType = new();

        public BehaviorTreeNodeView AddNodeAndView(Type type)
        {
            return AddNodeAndView(type, LastContextualMenuMousePosition);
        }

        public BehaviorTreeNodeView AddNodeAndView(Type type, Vector2 graphMousePosition)
        {
            BTNode node = AddNewNode(type, graphMousePosition);
            var nodeView = CreateNodeView(node);
            this.AddElement(nodeView);

            nearAddNodeType.Remove(type);
            nearAddNodeType.Add(type);

            return nodeView;
        }

        public BTNode AddNewNode(Type type, Vector2 graphMousePosition)
        {
            UndoRecord($"AddNode  [{type.Name}]");
            var node = Tree.AddNewNode(type);
            if (node.Meta == null)
            {
                node.Meta = new NodeMeta();
                node.Meta.x = graphMousePosition.x;
                node.Meta.y = graphMousePosition.y;
            }

            return node;
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

        /// <summary>
        /// 尝试复用旧的SOWrapper,解决Inpector面板锁定时不刷新的问题。
        /// </summary>
        /// <remarks>
        /// 这里不要用静态。同一个treeAsset可能同时打开不同的EditorWindow，并临时进行不同的更改。
        /// 如果使用全局静态，会导致Guid Key冲突。
        /// </remarks>
        internal protected Dictionary<string, NodeWrapper> NodeWrapperCache = new();
        internal protected Dictionary<string, DecoratorWrapper> DecoratorWrapperCache = new();

        public BehaviorTreeNodeView CreateNodeView(BTNode node, bool fakeNode = false)
        {
            var nodeView = new BehaviorTreeNodeView() { name = "behaviorTreeNode" };
            nodeView.TreeView = this;
            nodeView.SetNode(node, fakeNode);
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

            //if (Tree.StartNode != null)
            //{
            //    var oldStartNodeView = GetNodeByGuid(Tree.StartNode.GUID);
            //    if (oldStartNodeView != null)
            //    {
            //        oldStartNodeView.RemoveFromClassList(StartNodeClass);
            //    }
            //}

            this.LogMethodName();
            UndoRecord("Change Start Node");
            Tree.StartNode = behaviorTreeNodeView.SONode.Node;
            Tree.UpdateNodeIndexDepth();
            //需要重载整个View. 节点的unotConnected 属性会大面积改变。
            //ReloadView();
            //behaviorTreeNodeView.AddToClassList(StartNodeClass);

            ReloadAllNodeView();
        }

        internal void RefreshAllNodeEnabled()
        {
            foreach (var item in graphElements)
            {
                if (item is BehaviorTreeNodeView nodeView)
                {
                    nodeView.RefeshEnabled();
                }
            }
        }
    }
}



