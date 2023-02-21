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

            CreateDecoratorEntry<IConditionDecorator>(tree);
            CreateDecoratorEntry<IPreDecorator>(tree);
            CreateDecoratorEntry<IPostDecorator>(tree);
            CreateDecoratorEntry<IAbortDecorator>(tree);

            return tree;
        }

        public static void CreateDecoratorEntry<T>(List<SearchTreeEntry> tree)
            where T : IDecorator
        {
            var types = TypeCache.GetTypesDerivedFrom<T>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent(typeof(T).Name)) { level = 1 });
            foreach (var type in types)
            {
                tree.Add(new SearchTreeEntry(new GUIContent($"{type.Name}")) { level = 2, userData = type });
            }
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            NodeView?.AddDecorator(SearchTreeEntry.userData as Type);
            return true;
        }
    }
}
