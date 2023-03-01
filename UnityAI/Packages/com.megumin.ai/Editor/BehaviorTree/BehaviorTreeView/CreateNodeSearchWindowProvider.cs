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

            {
                AddCateGoryNode(tree);
            }

            //Tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node2"), 0));
            //Tree.Add(new SearchTreeEntry(new GUIContent("test")) {  level = 1});
        }

        public static void AddCateGoryNode(List<SearchTreeEntry> tree)
        {
            //Category 特性
            var types = TypeCache.GetTypesDerivedFrom<BTNode>();
            var pairs = from type in types
                        let attri = type.GetCustomAttribute<CategoryAttribute>()
                        where attri != null
                        orderby attri.Category
                        orderby type.Name
                        select (attri, type);

            HashSet<string> alreadyAddPathName = new();

            foreach (var item in pairs)
            {
                var type = item.type;
                var levelString = item.attri.Category.Split('/');
                var pathName = "";
                for (int i = 0; i < levelString.Length; i++)
                {
                    var levelName = levelString[i];
                    pathName += levelName;
                    if (!alreadyAddPathName.Contains(pathName))
                    {
                        alreadyAddPathName.Add(pathName);
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(levelName)) { level = 1 + i });
                    }
                }

                tree.Add(new SearchTreeEntry(new GUIContent($"      {type.Name}")) { level = 1 + levelString.Length, userData = type });
            }
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
