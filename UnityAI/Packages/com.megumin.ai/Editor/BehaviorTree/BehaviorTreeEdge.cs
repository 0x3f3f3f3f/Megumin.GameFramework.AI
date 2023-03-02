using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    internal class BehaviorTreeEdge : Edge
    {
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
                default:
                    break;
            }
        }
    }
}
