using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    internal class CreateNodeMenuWindow : ScriptableObject, ISearchWindowProvider
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
            tree.Add(new SearchTreeEntry(new GUIContent("test111"))
            {
                level = 1,
                userData = "测试"
            });
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            behaviorTreeView.AddNode(context.screenMousePosition);
            Debug.Log(searchTreeEntry.userData);
            return true;
        }
    }
}
