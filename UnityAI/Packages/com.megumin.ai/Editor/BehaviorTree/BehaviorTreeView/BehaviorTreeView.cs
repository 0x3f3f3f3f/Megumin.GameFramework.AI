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
        internal CreateNodeSearchWindowProvider createNodeMenu;

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
            }

            return SOTree;
        }

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

            CreateSOWrapperIfNull();
            if (force || Tree == null)
            {
                if (EditorWindow.CurrentAsset)
                {
                    SOTree.Tree = EditorWindow.CurrentAsset.Instantiate();
                }
                else
                {
                    return;
                }
            }

            this.LogMethodName();
            DeleteElements(graphElements.ToList().Where(elem => elem is BehaviorTreeNodeView || elem is Edge));

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

            //this.RepaintInspectorWindows();

            EditorWindow.UpdateHasUnsavedChanges();
            LoadVersion = SOTree.ChangeVersion;
        }

        public Vector2 LastContextualMenuMousePosition = Vector2.one * 100;
        public BehaviorTree Tree => SOTree?.Tree;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            LastContextualMenuMousePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            //Debug.Log(LastContextualMenuMousePosition);

            //evt.menu.AppendAction("Test", Test, DropdownMenuAction.AlwaysEnabled);
            //evt.menu.AppendSeparator();

            if (evt.target is BehaviorTreeNodeView nodeView)
            {
                nodeView.BuildContextualMenuBeforeBase(evt);
            }

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

            if (evt.target is BehaviorTreeNodeView nodeViewAfter)
            {
                nodeViewAfter.BuildContextualMenuAfterBase(evt);
            }
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

            nearAddNodeType.Remove(type);
            nearAddNodeType.Add(type);

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

        /// <summary>
        /// 尝试复用旧的SOWrapper,解决Inpector面板锁定时不刷新的问题。
        /// </summary>
        internal protected Dictionary<string, NodeWrapper> NodeWrapperCache = new();
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

            this.LogMethodName();
            UndoRecord("Change Start Node");
            Tree.StartNode = behaviorTreeNodeView.SONode.Node;
            behaviorTreeNodeView.AddToClassList(StartNodeClass);
        }
    }

    public class TreeWrapper : ScriptableObject
    {
        public BehaviorTree Tree;
        public int ChangeVersion;

    }
}



