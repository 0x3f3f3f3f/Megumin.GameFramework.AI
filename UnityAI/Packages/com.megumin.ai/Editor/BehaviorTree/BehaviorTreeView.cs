using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreeView : GraphView
    {
        public BehaviorTreeEditor EditorWindow { get; internal set; }
        private CreateNodeMenuWindow createNodeMenu;

        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }

        public BehaviorTreeView()
        {
            GridBackground element = new GridBackground();
            Insert(0, element);

            //child.SetPosition(Rect.zero);
            AddNode();

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

            createNodeMenu = ScriptableObject.CreateInstance<CreateNodeMenuWindow>();
            createNodeMenu.Initialize(this);

            nodeCreationRequest = (c) => SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), createNodeMenu);


        }

        private void AddNode()
        {
            var node = CreateNode();
            this.AddElement(node);
        }

        public Node CreateNode()
        {
            var node = new BehaviorTreeNodeView() { name = "testNode" };
            node.title = "TestNode";
            //node.capabilities |= Capabilities.Movable;
            node.SetPosition(new Rect(LastContextualMenuMousePosition.x, LastContextualMenuMousePosition.y, 100, 100));
            //node.AddToClassList("debug");
            return node;
        }

        public Vector2 LastContextualMenuMousePosition = Vector2.one * 100;



        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            LastContextualMenuMousePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            Debug.Log(LastContextualMenuMousePosition);

            evt.menu.AppendAction("Test", Test, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Test2", Test2, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();
            base.BuildContextualMenu(evt);
        }

        private void Test2(DropdownMenuAction obj)
        {
            Node node = new Node();
            node.title = "TestPort";
            node.SetPosition(new Rect(LastContextualMenuMousePosition.x, LastContextualMenuMousePosition.y, 100, 100));
            var inport = Port.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(object));
            var outport = Port.Create<Edge>(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(object));
            node.inputContainer.Add(inport);
            node.outputContainer.Add(outport);
            this.AddElement(node);
        }

        private void Test(DropdownMenuAction obj)
        {
            AddNode();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            Debug.Log(startPort);
            return ports.ToList();
            //return base.GetCompatiblePorts(startPort, nodeAdapter);
        }

        internal void AddNode(Vector2 screenMousePosition)
        {

        }

        
    }
}



