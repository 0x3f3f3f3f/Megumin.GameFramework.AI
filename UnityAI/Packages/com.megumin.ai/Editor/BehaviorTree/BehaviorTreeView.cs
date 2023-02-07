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

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreeView : GraphView, IDisposable
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
            m.mouseMove += FloatingTip.OnMouseMove;
            this.AddManipulator(m);
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            //this.AddManipulator(new DoubleClickSelection());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            //this.SetupZoom(0.5f, 2f,0.8f,1f);

            MiniMap child = new MiniMap();
            child.name = "minimap";
            this.AddElement(child);

            createNodeMenu = ScriptableObject.CreateInstance<CreateNodeSearchWindowProvider>();
            createNodeMenu.Initialize(this);

            nodeCreationRequest = (c) => SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), createNodeMenu);

            Debug.Log("+=ReloadView");
            Undo.undoRedoPerformed += ReloadView;

        }

        public void Dispose()
        {
            Debug.Log("-=ReloadView");
            Undo.undoRedoPerformed -= ReloadView;
        }

        ///不采用TheKiwiCoder 中的方式，Undo/Redo 时不能显示每一步操作名字。
        //SerializedObject treeSO;
        public TreeWapper treeWapper;
        /// <summary>
        /// 当前TreeView正在显示的tree版本,用于控制UndoRedo时，是否重新加载整个View。
        /// </summary>
        public int LoadVersion;

        internal Scope GraphViewReloadingScope = new Scope();
        public void ReloadView()
        {
            using var s = GraphViewReloadingScope.Enter();

            if (LoadVersion == treeWapper?.ChangeVersion)
            {
                Debug.Log("没有实质性改动，不要ReloadView");
                return;
            }

            DeleteElements(graphElements.ToList().Where(elem => elem is BehaviorTreeNodeView));

            this.LogFuncName();
            if (Tree == null && EditorWindow.CurrentAsset)
            {
                Tree = EditorWindow.CurrentAsset.CreateTree();
            }

            if (!treeWapper)
            {
                treeWapper = new TreeWapper();
                treeWapper.Tree = Tree;
            }

            LoadVersion = treeWapper.ChangeVersion;

            foreach (var node in Tree.AllNodes)
            {
                var nodeViwe = CreateNodeView(node);
                if (node == Tree.StartNode)
                {
                    nodeViwe.AddToClassList(StartNodeClass);
                }
                this.AddElement(nodeViwe);
            }
        }

        public Vector2 LastContextualMenuMousePosition = Vector2.one * 100;
        public BehaviorTree Tree;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            LastContextualMenuMousePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            Debug.Log(LastContextualMenuMousePosition);

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
            Debug.Log(startPort);
            return ports.ToList();
            //return base.GetCompatiblePorts(startPort, nodeAdapter);
        }

        public void CreateNode(Type type, Vector2 graphMousePosition)
        {
            if (Tree == null)
            {
                Debug.Log("new tree");
                Tree = new BehaviorTree();
                treeWapper = new TreeWapper();
                treeWapper.Tree = Tree;
            }
            UndoRecord("AddNode");
            var node = Tree.AddNewNode(type);
            if (node.Meta == null)
            {
                node.Meta = new NodeMeta();
                node.Meta.x = graphMousePosition.x;
                node.Meta.y = graphMousePosition.y;
            }
            var nodeView = CreateNodeView(node);
            this.AddElement(nodeView);
        }

        //public BehaviorTreeNodeView CreateNodeView()
        //{
        //    return CreateNodeView(null, LastContextualMenuMousePosition);
        //}

        public BehaviorTreeNodeView CreateNodeView(BTNode node)
        {
            var nodeView = new BehaviorTreeNodeView() { name = "testNode" };
            nodeView.TreeView = this;
            nodeView.title = "TestNode";
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


        public void UndoRecord(string name)
        {
            if (GraphViewReloadingScope.IsEnter)
            {
                Debug.Log($"Reloading期间不注册Undo/Redo");
            }
            else
            {
                Undo.RecordObject(treeWapper, name);
                treeWapper.ChangeVersion++;
                LoadVersion = treeWapper.ChangeVersion;

                EditorWindow.UpdateHasUnsavedChanges();
            }  
        }

        internal void InspectorShowWapper()
        {
            if (treeWapper)
            {
                Selection.activeObject = treeWapper;
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



