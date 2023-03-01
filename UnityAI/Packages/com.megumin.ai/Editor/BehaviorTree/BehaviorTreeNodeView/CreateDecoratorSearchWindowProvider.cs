using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Editor;
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

            tree.AddTypesDerivedFrom<IConditionDecorator>();
            tree.AddTypesDerivedFrom<IPreDecorator>();
            tree.AddTypesDerivedFrom<IPostDecorator>();
            tree.AddTypesDerivedFrom<IAbortDecorator>();
            tree.AddCateGory2<IDecorator>();

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            NodeView?.AddDecorator(SearchTreeEntry.userData as Type);
            return true;
        }
    }
}
