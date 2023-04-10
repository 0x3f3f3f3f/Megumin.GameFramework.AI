using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System;
using UnityEditor;
using Megumin.GameFramework.AI.Editor;
using System.Linq;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    //调试相关代码
    public partial class BehaviorTreeView
    {
        internal void OnPostTick()
        {
            var list = graphElements.ToList();
            foreach (var item in graphElements)
            {
                if (item is BehaviorTreeNodeView nodeView)
                {
                    nodeView.OnPostTick();
                }

                if (item is BehaviorTreeDecoratorView decoratorView)
                {
                    decoratorView.OnPostTick();
                }
            }
        }
    }
}
