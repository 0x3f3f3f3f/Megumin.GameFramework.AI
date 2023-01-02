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
            this.Add(child);
            //child.SetPosition(Rect.zero);
            Node child1 = new Node() { name = "testNode" };
            this.Add(child1);
            child1.capabilities |= Capabilities.Movable;
            child1.SetPosition(new Rect(100,100,100,100));
        }
    }
}