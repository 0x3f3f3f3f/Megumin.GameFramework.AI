﻿using System;
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
            this.SetToClassList(UssClassConst.running, isRunning);
            InputPort.SetToClassList(UssClassConst.running, isRunning);
            OutputPort.SetToClassList(UssClassConst.running, isRunning);
            //Edge 通过Port --port-color 计算颜色，但是更新上有问题
            //Edge 在树中更靠前，OnCustomStyleResolved 比 Port先执行，
            //这时Port的Running颜色还没有更新，会导致计算错误
            //解决方法：
            //Edge设置一个colorMode参数，允许Edge不通过根据Port计算颜色，独立设置一个颜色

            foreach (var edge in InputPort.connections)
            {
                edge.SetToClassList(UssClassConst.running, isRunning);
                //edge.schedule.Execute(() => { edge.SetToClassList(UssClassConst.running, isRunning); }).ExecuteLater(10);
            }

            UpdateCompletedState();
        }

        private async void UpdateCompletedState()
        {
            bool hasChanged = false;
            var isSucceeded = Node?.State == Status.Succeeded;
            hasChanged |= this.SetToClassList(UssClassConst.succeeded, isSucceeded);
            var isFailed = Node?.State == Status.Failed;
            hasChanged |= this.SetToClassList(UssClassConst.failed, isFailed);

            //if (hasChanged)
            //{
            //    var res = this.Delay(3000);
            //    UpdateCompletedState();
            //}
        }
    }
}
