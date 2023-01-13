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
            var node = CreateNode();
            this.AddElement(node);

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

        public Node CreateNode()
        {
            Node child1 = new Node() { name = "testNode" };
            child1.title = "TestNode";
            //node.capabilities |= Capabilities.Movable;
            child1.SetPosition(new Rect(100, 100, 100, 100));
            child1.AddToClassList("debug");
            return child1;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Test", Test, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();
            base.BuildContextualMenu(evt);
        }

        private void Test(DropdownMenuAction obj)
        {
            BehaviorTreeNodeView node = new  BehaviorTreeNodeView() { name = "testBehaviorTreeNodeView" };
            node.title = "TestNodeView";
            //node.capabilities |= Capabilities.Movable;
            node.SetPosition(new Rect(100, 100, 100, 100));
            node.AddToClassList("debug");

            this.AddElement(node);
        }
    }
}



