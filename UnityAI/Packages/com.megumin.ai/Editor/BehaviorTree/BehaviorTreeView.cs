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
        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }

        public BehaviorTreeView()
        {
            GridBackground element = new GridBackground();
            Insert(0, element);



            //child.SetPosition(Rect.zero);
            AddNode();

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            //this.AddManipulator(new DoubleClickSelection());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            //this.SetupZoom(0.5f, 2f,0.8f,1f);

            MiniMap child = new MiniMap();
            child.name = "minimap";
            this.AddElement(child);
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
            node.SetPosition(new Rect(100, 100, 100, 100));
            //node.AddToClassList("debug");
            return node;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Test", Test, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Test2", Test2, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();
            base.BuildContextualMenu(evt);
        }

        private void Test2(DropdownMenuAction obj)
        {
           Node node = new Node();
            node.title = "TestPort";
            node.SetPosition(new Rect(100, 100, 100, 100));
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
            return ports.ToList();
            return base.GetCompatiblePorts(startPort, nodeAdapter);
        }
    }
}



