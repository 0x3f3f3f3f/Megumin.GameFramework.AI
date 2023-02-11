using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreeDecoratorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BehaviorTreeDecoratorView, UxmlTraits> { }

        public BehaviorTreeDecoratorView()
        {
            //var visualTree = Resources.Load<VisualTreeAsset>("BehaviorTreeDecoratorView");
            //visualTree.CloneTree(this);
        }
    }
}
