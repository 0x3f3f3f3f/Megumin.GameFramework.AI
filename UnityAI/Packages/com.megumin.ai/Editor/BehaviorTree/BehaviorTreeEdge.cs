using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeEdge : Edge
    {

        public BehaviorTreeEdge()
        {
            flowTask = schedule.Execute(() => UpdateFlow()).Every(1000 / 60);
            flowTask.Pause();
        }

        /// <summary>
        /// 设置一个--edge-colorMode参数，允许Edge不通过根据Port计算颜色，独立设置一个颜色
        /// <para/>支持：inputColor,outputColor,defaultColor
        /// </summary>
        static CustomStyleProperty<string> colorMode = new CustomStyleProperty<string>("--edge-colorMode");
        protected IVisualElementScheduledItem flowTask;

        public string ColorMode { get; set; }
        protected override void OnCustomStyleResolved(ICustomStyle styles)
        {
            base.OnCustomStyleResolved(styles);

            if (styles.TryGetValue(colorMode, out var value))
            {
                ColorMode = value;
            }
            else
            {
                ColorMode = null;
            }

            OnCustomStyleResolvedFlow(styles);

            MyUpdateEdgeControlColorsAndWidth();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            MyUpdateEdgeControlColorsAndWidth();
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            MyUpdateEdgeControlColorsAndWidth();
        }

        public override bool UpdateEdgeControl()
        {
            var result = base.UpdateEdgeControl();
            MyUpdateEdgeControlColorsAndWidth();

            if (result)
            {
                UpdateFlowPointCount();
                UpdateFlow();
            }
            return result;
        }

        void MyUpdateEdgeControlColorsAndWidth()
        {
            if (selected)
            {
                return;
            }

            if (isGhostEdge)
            {
                return;
            }

            switch (ColorMode)
            {
                case "inputColor":
                    edgeControl.outputColor = edgeControl.inputColor;
                    break;
                case "outputColor":
                    edgeControl.inputColor = edgeControl.outputColor;
                    break;
                case "defaultColor":
                    edgeControl.inputColor = defaultColor;
                    edgeControl.outputColor = defaultColor;
                    break;
                default:
                    break;
            }
        }
    }

    public partial class BehaviorTreeEdge
    {
        //flow
        //https://forum.unity.com/threads/how-to-add-flow-effect-to-edges-in-graphview.1326012/
        //没有使用连接中的算法，
        //连接中的算法只支持一个点
        //不考虑flow point 的颜色，改为uss控制。

        static CustomStyleProperty<float> flowDistanceProperty = new CustomStyleProperty<float>("--edge-flowDistance");
        static CustomStyleProperty<float> flowSpeedProperty = new CustomStyleProperty<float>("--edge-flowSpeed");
        public float FlowDistance { get; set; } = -1;
        public float FlowSpeed { get; set; } = 0;

        public virtual float DefaultFlowDistance { get; set; } = -1;
        public virtual float DefaultFlowSpeed { get; set; } = 0;


        protected void OnCustomStyleResolvedFlow(ICustomStyle styles)
        {
            if (styles.TryGetValue(flowDistanceProperty, out var value2))
            {
                FlowDistance = value2;
            }
            else
            {
                FlowDistance = DefaultFlowDistance;
            }

            if (styles.TryGetValue(flowSpeedProperty, out var value3))
            {
                FlowSpeed = value3;
            }
            else
            {
                FlowSpeed = DefaultFlowSpeed;
            }

            UpdateFlowPointCount();
            UpdateFlow();
        }

        public virtual void UpdateFlowPointCount()
        {
            if (FlowDistance <= 0)
            {
                flowTask.Pause();
                return;
            }

            if (FlowSpeed != 0)
            {
                flowTask.Resume();
            }
            else
            {
                flowTask.Pause();
            }

            //TODO: 不知道为什么有时候edgeControl 会变的很大，导致totleLenght，多创建很多点，
            //      只是会影响性能，不会有表现错误，先不用管
            var totleLenght = 0f;
            for (int i = 1; i < edgeControl.controlPoints?.Length; i++)
            {
                var prev = edgeControl.controlPoints[i - 1];
                var cur = edgeControl.controlPoints[i];
                totleLenght += Vector2.Distance(prev, cur);
            }

            var count = (int)(totleLenght / FlowDistance);
            for (int i = FlowPoint.Count; i < count; i++)
            {
                //自动扩容增加点。
                VisualElement flowPoint = new VisualElement();
                flowPoint.style.position = Position.Absolute;
                flowPoint.pickingMode = PickingMode.Ignore;
                flowPoint.name = "flowPoint";
                flowPoint.AddToClassList(UssClassConst.flowPoint);
                this.Add(flowPoint);
                FlowPoint.Add(flowPoint);
            }
        }

        public List<VisualElement> FlowPoint { get; } = new();

        public virtual void UpdateFlow()
        {
            Profiler.BeginSample("BehaviorTreeEdge.UpdateFlow");

            if (edgeControl?.controlPoints.Length > 1 || FlowDistance <= 0)
            {
                float offset = ((float)EditorApplication.timeSinceStartup * FlowSpeed * FlowDistance) % FlowDistance;

                int controlPointsCursor = 0;
                float passedEdgeLength = 0f;
                ///当前阶段线段长度
                float currentPhaseLength = 0f;

                ///计算第一阶段线段长度
                currentPhaseLength = Vector2.Distance(edgeControl.controlPoints[0], edgeControl.controlPoints[1]);

                for (int i = 0; i < FlowPoint.Count; i++)
                {
                    //计算当前点位置
                    var point = FlowPoint[i];

                    //到起始点的长度
                    var distance2Start = offset + i * FlowDistance;

                    //到上一个控制点的长度
                    var distance2ControlPointsCursor = distance2Start - passedEdgeLength;

                    if (distance2ControlPointsCursor >= currentPhaseLength && currentPhaseLength > 0)
                    {
                        //超过当前线段，移动到下一个线段
                        controlPointsCursor += 1;
                        passedEdgeLength += currentPhaseLength;
                        distance2ControlPointsCursor = distance2Start - passedEdgeLength;

                        if (edgeControl.controlPoints.Length - 1 > controlPointsCursor)
                        {
                            currentPhaseLength =
                                Vector2.Distance(edgeControl.controlPoints[controlPointsCursor],
                                                 edgeControl.controlPoints[controlPointsCursor + 1]);
                        }
                        else
                        {
                            //已经到达Edge末尾
                            currentPhaseLength = -1;
                        }
                    }

                    if (currentPhaseLength <= 0 || distance2ControlPointsCursor <= 0)
                    {
                        //无效长度隐藏控制点
                        point.style.display = DisplayStyle.None;
                    }
                    else
                    {
                        point.style.display = DisplayStyle.Flex;
                        var from = edgeControl.controlPoints[controlPointsCursor];
                        var to = edgeControl.controlPoints[controlPointsCursor + 1];

                        var position = Vector2.MoveTowards(from, to, distance2ControlPointsCursor);

                        //计算半径偏移
                        var center = point.layout.center;
                        if (center.sqrMagnitude > 0.01f)
                        {
                            position -= center;
                        }
                        else
                        {
                            //NaN 会导致整个graphView 不正常
                            //Debug.Log(point.layout);
                        }

                        point.transform.position = position;
                    }
                }
            }
            else
            {
                //没有足够控制点
                foreach (var point in FlowPoint)
                {
                    point.style.display = DisplayStyle.None;
                }
            }

            Profiler.EndSample();
        }
    }
}
