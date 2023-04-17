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
        Edge edgeFilter;
        BehaviorTreeView behaviorTreeView;
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

            if (edgeFilter == null)
                CreateStandardNodeMenu(tree);
            else
                CreateEdgeNodeMenu(tree);

            return tree;
        }

        private void CreateEdgeNodeMenu(List<SearchTreeEntry> tree)
        {
            throw new NotImplementedException();
        }

        private void CreateStandardNodeMenu(List<SearchTreeEntry> tree)
        {
            tree.AddTypesDerivedFrom<CompositeNode>("Composite");
            tree.AddTypesDerivedFrom<OneChildNode>("OneChildNode");
            tree.AddTypesDerivedFrom<TwoChildNode>("TwoChildNode");
            tree.AddCateGory2<BTNode>();
            tree.AddTypesDerivedFrom<BTActionNode>("AllAction");
            //Tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node2"), 0));
            //Tree.Add(new SearchTreeEntry(new GUIContent("test")) {  level = 1});
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            Vector2 editorwindowMousePosition = context.screenMousePosition
                                                - behaviorTreeView.EditorWindow.position.position;
            var graphMousePosition = behaviorTreeView.contentViewContainer.WorldToLocal(editorwindowMousePosition);

            behaviorTreeView.AddNodeAndView(searchTreeEntry.userData as Type, graphMousePosition);

            return true;
        }
    }
}
