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
    public class BehaviorTreeView : GraphView,IDisposable
    {
        public BehaviorTreeEditor EditorWindow { get; internal set; }
        private CreateNodeSearchWindowProvider createNodeMenu;

        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }

        public BehaviorTreeView()
        {
            GridBackground background = new GridBackground();
            Insert(0, background);


            this.AddManipulator(new MouseMoveManipulator());
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

            Undo.undoRedoPerformed += ReloadView;
        }

        public void Dispose()
        {
            Undo.undoRedoPerformed -= ReloadView;
        }

        private void ReloadView()
        {
            this.LogFuncName();
        }

        public Vector2 LastContextualMenuMousePosition = Vector2.one * 100;
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            LastContextualMenuMousePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            Debug.Log(LastContextualMenuMousePosition);

            evt.menu.AppendAction("Test", Test, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();
            base.BuildContextualMenu(evt);
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
            var node = Activator.CreateInstance(type);
            var nodeView = CreateNodeView(node, graphMousePosition);
            this.AddElement(nodeView);
        }

        public BehaviorTreeNodeView CreateNodeView()
        {
            return CreateNodeView(null, LastContextualMenuMousePosition);
        }

        public BehaviorTreeNodeView CreateNodeView(object node, Vector2 nodePosition)
        {
            var nodeView = new BehaviorTreeNodeView() { name = "testNode" };
            nodeView.title = "TestNode";
            nodeView.SetNode(node);
            //node.capabilities |= Capabilities.Movable;
            nodeView.SetPosition(new Rect(nodePosition.x, nodePosition.y, 100, 100));
            //node.AddToClassList("debug");
            return nodeView;
        }
    }
}



