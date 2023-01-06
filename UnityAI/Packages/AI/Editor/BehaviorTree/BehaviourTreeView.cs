using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviourTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

        public BehaviourTreeView()
        {
            GridBackground element = new GridBackground();
            Insert(0, element);

            MiniMap child = new MiniMap();
            child.name = "minimap";
            this.Add(child);

            //child.SetPosition(Rect.zero);
            AddNode();

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            //this.AddManipulator(new DoubleClickSelection());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            //this.SetupZoom(0.5f, 2f,0.8f,1f);
        }

        public void AddNode()
        {
            Node child1 = new Node() { name = "testNode" };
            child1.title = "TestNode";
            this.Add(child1);
            //child1.capabilities |= Capabilities.Movable;
            child1.SetPosition(new Rect(100, 100, 100, 100));
        }
    }
}