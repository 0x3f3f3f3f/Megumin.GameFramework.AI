using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class DoubleClickSelector : MouseManipulator
    {
        public static bool WasSelectableDescendantHitByMouse(GraphElement currentTarget, MouseDownEvent evt)
        {
            VisualElement targetElement = evt.target as VisualElement;

            if (targetElement == null || currentTarget == targetElement)
                return false;

            VisualElement descendant = targetElement;

            while (descendant != null && currentTarget != descendant)
            {
                GraphElement selectableDescendant = descendant as GraphElement;
                if (selectableDescendant.WasHitByMouse(evt))
                {
                    return true;
                }
                descendant = descendant.parent;
            }
            return false;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            base.target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            base.target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected virtual void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.currentTarget is GraphElement graphElement)
            {
                if (CanStartManipulation(evt)
                    && graphElement.IsSelectable()
                    && graphElement.HitTest(evt.localMousePosition)
                    && !WasSelectableDescendantHitByMouse(graphElement, evt))
                {
                    OnClick(graphElement, evt);

                }
            }
        }

        double time;

        public DoubleClickSelector(Action<MouseDownEvent> onDoubleClick, double doubleClickDuration = 0.3)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse, modifiers = EventModifiers.Shift });
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse, modifiers = EventModifiers.Alt });

            activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse });
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse, modifiers = EventModifiers.Shift });
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse, modifiers = EventModifiers.Alt });

            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse, modifiers = EventModifiers.Command });
            }
            else
            {
                activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse, modifiers = EventModifiers.Control });
            }

            this.DoubleClickDuration = doubleClickDuration;
            OnDoubleClickCallback = onDoubleClick;
        }

        public double DoubleClickDuration { get; set; } = 0.3;
        public Action<MouseDownEvent> OnDoubleClickCallback { get; set; }

        protected virtual void OnClick(GraphElement graphElement, MouseDownEvent evt)
        {
            var selection = graphElement.GetFirstAncestorOfType<ISelection>();
            double duration = EditorApplication.timeSinceStartup - time;
            if (duration < DoubleClickDuration)
            {
                time = -1;
                OnDoubleClick(graphElement, evt);
            }

            time = EditorApplication.timeSinceStartup;
            // Do not stop the propagation as it is common case for a parent start to move the selection on a mouse down.
        }

        protected virtual void OnDoubleClick(GraphElement graphElement, MouseDownEvent evt)
        {
            OnDoubleClickCallback?.Invoke(evt);
        }
    }
}
