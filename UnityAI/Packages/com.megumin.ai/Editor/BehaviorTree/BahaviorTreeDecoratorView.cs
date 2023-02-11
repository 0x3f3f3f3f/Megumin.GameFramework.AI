using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BahaviorTreeDecoratorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BahaviorTreeDecoratorView, UxmlTraits> { }

        public BahaviorTreeDecoratorView()
        {
            var visualTree = Resources.Load<VisualTreeAsset>("BehaviorTreeNodeView");
            visualTree.CloneTree(this);
        }
    }
}
