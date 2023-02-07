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
    public class FloatingTip : VisualElement
    {
        public Label MousePosTip;
        public Label CustomTip;

        public FloatingTip() : this(null)
        {

        }

        public FloatingTip(GraphView graphView)
        {
            this.GraphView = graphView;

            name = "floatingTip";
            AddToClassList("floatingTip");

            MousePosTip = new Label() { name = "mousePos" };
            MousePosTip.AddToClassList("mousePos");
            MousePosTip.AddToClassList("mouseTip");
            Add(MousePosTip);
            CustomTip = new Label() { name = "customTip" };
            CustomTip.AddToClassList("customTip");
            CustomTip.AddToClassList("mouseTip");
            CustomTip.AddToClassList("unDisplay");
            Add(CustomTip);
        }

        public GraphView GraphView { get; set; }

        public void SetTip(string tip)
        {
            CustomTip.text = tip;
            if (string.IsNullOrEmpty(CustomTip.text))
            {
                CustomTip.AddToClassList("unDisplay");
            }
            else
            {
                CustomTip.RemoveFromClassList("unDisplay");
            }
        }

        public void AppendTip(string tip)
        {
            var newTip = CustomTip.text + "\n" + tip;
            SetTip(newTip);
        }

        public void Show(bool value)
        {
            if (!value)
            {
                AddToClassList("unDisplay");
            }
            else
            {
                RemoveFromClassList("unDisplay");
            }
        }
    }
}
