using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public partial class BehaviorTree
    {
        static readonly Unity.Profiling.ProfilerMarker tickProfilerMarker = new("Tick");

        public long LastTickFrame { get; protected set; } = -10000000;
        public double LastTickTime { get; protected set; } = -10000000;
        public double LastTickRealTime { get; protected set; } = -10000000;

        /// <summary>
        /// Todo 抽象出runner ，分别 root - leaf 驱动，last leaf， 异步。三种方式根调用不一样。但是都需要Tick。
        /// </summary>
        /// <returns></returns>
        public Status Tick(object options = null)
        {
            using var profiler = tickProfilerMarker.Auto();

            //根树检查ReStart
            if (IsRootTree)
            {
                if (treestate == Status.Succeeded)
                {
                    if (RunOption != null && (RunOption.OnSucceeded & OperationTree.ReStart) != 0)
                    {
                        treestate = Status.Init;
                    }
                    else
                    {
                        return Status.Succeeded;
                    }
                }

                if (treestate == Status.Failed)
                {
                    if (RunOption != null && (RunOption.OnFailed & OperationTree.ReStart) != 0)
                    {
                        treestate = Status.Init;
                    }
                    else
                    {
                        return Status.Failed;
                    }
                }
            }

            //根树检查更新间隔
            var currentframeCount = Time.frameCount;
            var currentTime = Time.timeAsDouble;
            var currentRealTime = Time.realtimeSinceStartupAsDouble;

            if (IsRootTree && RunOption != null)
            {
                //根树检查RunOption
                if (RunOption.FrameInterval.Enabled)
                {
                    if (currentframeCount - LastTickFrame < RunOption.FrameInterval.Value)
                    {
                        //小于设置的间隔，忽略本次tick。
                        return treestate;
                    }
                }

                if (RunOption.TimeInterval.Enabled)
                {
                    if (currentTime - LastTickTime < RunOption.TimeInterval.Value)
                    {
                        //小于设置的间隔，忽略本次tick。
                        return treestate;
                    }
                }

                if (RunOption.RealtimeInterval.Enabled)
                {
                    if (currentRealTime - LastTickRealTime < RunOption.RealtimeInterval.Value)
                    {
                        //小于设置的间隔，忽略本次tick。
                        return treestate;
                    }
                }
            }

            LastTickFrame = currentframeCount;
            LastTickTime = currentTime;
            LastTickRealTime = currentRealTime;

            RemoveLifeEndEventData();

            return CoreTick(null, options);
        }

        internal protected Status CoreTick(BTNode from = null, object options = null)
        {
            treestate = TickStartNode(from, options);
            TotalTickCount++;

            if (treestate == Status.Succeeded)
            {
                CompletedCount++;
                SucceededCount++;
                Debug.Log($"tree complate. {treestate}");
            }
            else if (treestate == Status.Failed)
            {
                CompletedCount++;
                FailedCount++;
                Debug.Log($"tree complate. {treestate}");
            }
            return treestate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from">如果是子树时存在父节点</param>
        /// <returns></returns>
        protected Status TickStartNode(BTNode from, object options = null)
        {
            if (StartNode == null)
            {
                return Status.Failed;
            }

            if (StartNode.Enabled == false)
            {
                Debug.Log($"StartNode is not Enabled!");
                return Status.Failed;
            }
            return StartNode.Tick(from, options);
        }

    }
}


