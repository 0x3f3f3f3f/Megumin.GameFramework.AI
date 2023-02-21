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
    internal class CreateDecoratorSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        public BehaviorTreeNodeView NodeView { get; internal set; }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Decorator"), 0),
            };

            {
                var types = TypeCache.GetTypesDerivedFrom<IConditionDecirator>();
                tree.Add(new SearchTreeGroupEntry(new GUIContent("Condition Decorator")) { level = 1 });
                foreach (var type in types)
                {
                    tree.Add(new SearchTreeEntry(new GUIContent($"{type.Name}")) { level = 2, userData = type });
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<IPreDecirator>();
                tree.Add(new SearchTreeGroupEntry(new GUIContent("IPreDecirator")) { level = 1 });
                foreach (var type in types)
                {
                    tree.Add(new SearchTreeEntry(new GUIContent($"{type.Name}")) { level = 2, userData = type });
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<IPostDecirator>();
                tree.Add(new SearchTreeGroupEntry(new GUIContent("IPostDecirator")) { level = 1 });
                foreach (var type in types)
                {
                    tree.Add(new SearchTreeEntry(new GUIContent($"{type.Name}")) { level = 2, userData = type });
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<IAbortDecirator>();
                tree.Add(new SearchTreeGroupEntry(new GUIContent("IAbortDecirator")) { level = 1 });
                foreach (var type in types)
                {
                    tree.Add(new SearchTreeEntry(new GUIContent($"{type.Name}")) { level = 2, userData = type });
                }
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            NodeView?.AddDecorator(SearchTreeEntry.userData as Type);
            return true;
        }
    }
}
