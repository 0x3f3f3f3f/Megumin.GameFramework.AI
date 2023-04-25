using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.ComponentModel;
using Megumin.GameFramework.AI.Editor;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    internal class CreateNodeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        BehaviorTreeView behaviorTreeView;

        public TaskCompletionSource<(Type Type, Vector2 GraphPosition)> NextTaskSource { get; internal set; }
        public Edge NextEdge { get; internal set; }

        internal void Initialize(BehaviorTreeView behaviorTreeView)
        {
            this.behaviorTreeView = behaviorTreeView;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            };

            var canAddActionNode = true;
            if (NextEdge != null)
            {
                canAddActionNode = NextEdge.output != null;
            }

            tree.AddTypesDerivedFrom<CompositeNode>("Composite");
            tree.AddTypesDerivedFrom<OneChildNode>("OneChildNode");
            tree.AddTypesDerivedFrom<TwoChildNode>("TwoChildNode");

            if (canAddActionNode)
            {
                tree.AddCateGory2<BTNode>();
                tree.AddTypesDerivedFrom<BTActionNode>("Others", checkAlreadyHas: true);
                tree.AddTypesDerivedFrom<BTActionNode>("AllAction");
            }

            //Tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node2"), 0));
            //Tree.Add(new SearchTreeEntry(new GUIContent("test")) {  level = 1});

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            Vector2 editorwindowMousePosition = context.screenMousePosition
                                                - behaviorTreeView.EditorWindow.position.position;
            var graphMousePosition = behaviorTreeView.contentViewContainer.WorldToLocal(editorwindowMousePosition);

            if (NextTaskSource == null)
            {
                behaviorTreeView.AddNodeAndView(searchTreeEntry.userData as Type, graphMousePosition);
            }
            else
            {
                var source = NextTaskSource;
                NextTaskSource = null;
                source.TrySetResult((searchTreeEntry.userData as Type, graphMousePosition));
            }

            return true;
        }
    }
}
