using System;
using System.Collections.Generic;
using System.Linq;
using Megumin.GameFramework.AI.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeNodeView
    {
        internal void OnPostTick()
        {
            //this.LogMethodName();
            this.SetToClassList(UssClassConst.Running, Node?.State == Status.Running);
            foreach (var edge in InputPort.connections)
            {
                edge.SetToClassList(UssClassConst.Running, Node?.State == Status.Running);
            }
        }
    }
}
