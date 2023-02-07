using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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
            {
                var types = TypeCache.GetTypesDerivedFrom<ActionTaskNode>();
                tree.Add(new SearchTreeGroupEntry(new GUIContent("Action")) { level = 1 });
                foreach (var type in types)
                {
                    tree.Add(new SearchTreeEntry(new GUIContent($"{type.Name}")) { level = 2, userData = type });
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
                tree.Add(new SearchTreeGroupEntry(new GUIContent("Composite")) { level = 1 });
                foreach (var type in types)
                {
                    tree.Add(new SearchTreeEntry(new GUIContent($"{type.Name}")) { level = 2, userData = type });
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<OneChildNode>();
                tree.Add(new SearchTreeGroupEntry(new GUIContent("OneChildNode")) { level = 1 });
                foreach (var type in types)
                {
                    tree.Add(new SearchTreeEntry(new GUIContent($"{type.Name}")) { level = 2, userData = type });
                }
            }

            //Tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node2"), 0));
            //Tree.Add(new SearchTreeEntry(new GUIContent("test")) {  level = 1});
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var windowRoot = behaviorTreeView.EditorWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(
                windowRoot.parent, context.screenMousePosition - behaviorTreeView.EditorWindow.position.position);
            var graphMousePosition = behaviorTreeView.contentViewContainer.WorldToLocal(windowMousePosition);

            behaviorTreeView.AddNodeAndView(searchTreeEntry.userData as Type, graphMousePosition);

            return true;
        }
    }
}
