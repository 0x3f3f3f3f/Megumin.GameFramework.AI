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
        bool isRunning = false;
        Status lastTickState = Status.Init;
        internal void OnPostTick()
        {
            if (Node == null)
            {
                return;
            }

            //this.LogMethodName();
            isRunning = Node.State == Status.Running;
            if (isRunning)
            {
                RefreshDetail();
            }
            else
            {
                var isAbort = Node.FailedCode == FailedCode.Abort;
                this.SetToClassList(UssClassConst.isAbort, isAbort);
            }

            if (lastTickState != Node.State)
            {
                OnStateChange();
                UpdateCompletedState();
                if (isRunning)
                {
                    ChangeToRunning();
                }
            }

            lastTickState = Node.State;

            //foreach (var item in AllDecoratorView)
            //{
            //    item.OnPostTick();
            //}
        }

        private void OnStateChange()
        {
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
                bool edgeRunning = false; 
                if (edge.output.node is BehaviorTreeNodeView nodeView)
                {
                    if (nodeView.Node != null && nodeView.Node.State == Status.Running)
                    {
                        edgeRunning = true;
                    }
                }

                edge.SetToClassList(UssClassConst.running, isRunning && edgeRunning);
                //edge.schedule.Execute(() => { edge.SetToClassList(UssClassConst.running, isRunning); }).ExecuteLater(10);
            }
        }

        private void ChangeToRunning()
        {
            //进入Running 第一次Tick

            if (Node is SubTree subTree)
            {
                BehaviorTreeManager.TreeDebugger.AddDebugInstanceTree(subTree.BehaviourTree);
            }
        }

        private void UpdateCompletedState()
        {
            RefreshDetail();

            bool hasChanged = false;
            var isSucceeded = Node.State == Status.Succeeded;
            hasChanged |= this.SetToClassList(UssClassConst.succeeded, isSucceeded);
            var isFailed = Node.State == Status.Failed;
            hasChanged |= this.SetToClassList(UssClassConst.failed, isFailed);
            var isAbort = Node.FailedCode == FailedCode.Abort;
            hasChanged |= this.SetToClassList(UssClassConst.isAbort, isAbort);
            //if (hasChanged)
            //{
            //    var res = this.Delay(3000);
            //    UpdateCompletedState();
            //}
        }
    }
}
