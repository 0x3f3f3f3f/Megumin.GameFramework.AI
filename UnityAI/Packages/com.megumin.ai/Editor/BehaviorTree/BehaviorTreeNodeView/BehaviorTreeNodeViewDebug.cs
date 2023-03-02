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
            //Edge 在树中更靠前，OnCustomStyleResolved 比 Port先执行，
            //这时Port的Running颜色还没有更新，会导致计算错误
            //解决方法：
            //Edge设置一个colorMode参数，允许Edge不通过根据Port计算颜色，独立设置一个颜色

            foreach (var edge in InputPort.connections)
            {
                edge.SetToClassList(UssClassConst.Running, isRunning);
                //edge.schedule.Execute(() => { edge.SetToClassList(UssClassConst.Running, isRunning); }).ExecuteLater(10);
            }
        }
    }
}
