using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeEdge : Edge
    {
        /// <summary>
        /// 设置一个--edge-colorMode参数，允许Edge不通过根据Port计算颜色，独立设置一个颜色
        /// <para/>支持：inputColor,outputColor,defaultColor
        /// </summary>
        static CustomStyleProperty<string> colorMode = new CustomStyleProperty<string>("--edge-colorMode");
        public string ColorMode { get; set; }
        protected override void OnCustomStyleResolved(ICustomStyle styles)
        {
            base.OnCustomStyleResolved(styles);
            string value = null;
            if (styles.TryGetValue(colorMode, out value))
            {
                ColorMode = value;
            }

            float value2 = -1;
            if (styles.TryGetValue(flowDistanceProperty, out value2))
            {
                FlowDistance = value2;
            }

            MyUpdateEdgeControlColorsAndWidth();
            UpdateFlowPointCount();
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
            UpdateFlowPointCount();
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
        public float FlowDistance { get; set; } = -1;

        public virtual void UpdateFlowPointCount()
        {
            if (FlowDistance <= 0)
            {
                return;
            }

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
                flowPoint.name = "flowPoint";
                flowPoint.AddToClassList(UssClassConst.flowPoint);
                this.Add(flowPoint);
                FlowPoint.Add(flowPoint);
                flowPoint.transform.position = Vector2.one * 20;
            }
        }

        public List<VisualElement> FlowPoint { get; } = new();

        public virtual void UpdateFlow()
        {

        }
    }
}
