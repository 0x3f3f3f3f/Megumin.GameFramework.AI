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
            var isRunning = Node?.State == Status.Running;
            this.SetToClassList(UssClassConst.Running, isRunning);
            InputPort.SetToClassList(UssClassConst.Running, isRunning);
            OutputPort.SetToClassList(UssClassConst.Running, isRunning);
            //Edge 通过Port --port-color 计算颜色，但是更新上有问题
            //不知道没更新还是 更新时port颜色还没更新Todo
            foreach (var edge in InputPort.connections)
            {
                edge.SetToClassList(UssClassConst.Running, isRunning);
                edge.schedule.Execute(() => { edge.MarkDirtyRepaint(); }).ExecuteLater(1000);
            }
        }
    }
}
