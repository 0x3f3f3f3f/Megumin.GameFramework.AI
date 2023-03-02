using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreeEdge : Edge
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
}
