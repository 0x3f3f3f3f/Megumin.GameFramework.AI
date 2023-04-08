using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class MouseMoveManipulator : MouseManipulator
    {
        public Action<MouseMoveEvent> mouseMove;

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);

            //target.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            //Debug.Log($"{evt.localMousePosition}   {evt.ToStringReflection()}" );
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            mouseMove?.Invoke(evt);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);

            //target.UnregisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }
    }

    internal class TestMouseManipulator : MouseManipulator
    {
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);

            target.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            this.LogMethodName();
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            this.LogMethodName();
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);

            target.UnregisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }
    }

    /// <summary>
    /// 隔绝鼠标事件
    /// </summary>
    /// <typeparam name="TEventType"></typeparam>
    public class StopPropagationMouseManipulator<TEventType> : MouseManipulator
        where TEventType : EventBase<TEventType>, IMouseEvent, new()
    {

        public StopPropagationMouseManipulator()
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        public StopPropagationMouseManipulator(params MouseButton[] buttons)
        {
            foreach (var item in buttons.ToHashSet())
            {
                activators.Add(new ManipulatorActivationFilter { button = item });
            }
        }

        public bool TrickleDownPhase { get; set; }
        public bool NoTrickleDownPhase { get; set; } = true;

        protected override void RegisterCallbacksOnTarget()
        {
            Register<TEventType>();
        }

        protected void Register<T>()
             where T : EventBase<T>, IMouseEvent, new()
        {
            if (TrickleDownPhase)
            {
                target.RegisterCallback<T>(OnEvent, TrickleDown.TrickleDown);
            }

            if (NoTrickleDownPhase)
            {
                target.RegisterCallback<T>(OnEvent, TrickleDown.NoTrickleDown);
            }
        }

        protected void OnEvent<T>(T evt)
            where T : EventBase, IMouseEvent
        {
            if (CanStartManipulation(evt))
            {
                //Debug.Log($"StopPropagation {evt.ToStringReflection()}");
                evt.StopPropagation();
            }
        }

        protected void Unregister<T>()
            where T : EventBase<T>, IMouseEvent, new()
        {
            if (TrickleDownPhase)
            {
                target.UnregisterCallback<T>(OnEvent, TrickleDown.TrickleDown);
            }

            if (NoTrickleDownPhase)
            {
                target.UnregisterCallback<T>(OnEvent, TrickleDown.NoTrickleDown);
            }
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            Unregister<TEventType>();
        }
    }

    ///<inheritdoc/>
    public class StopPropagationMouseManipulator<T1, T2> : StopPropagationMouseManipulator<T1>
        where T1 : EventBase<T1>, IMouseEvent, new()
        where T2 : EventBase<T2>, IMouseEvent, new()
    {
        public StopPropagationMouseManipulator() : base()
        {
        }

        public StopPropagationMouseManipulator(params MouseButton[] buttons) : base(buttons)
        {
        }

        protected override void RegisterCallbacksOnTarget()
        {
            base.RegisterCallbacksOnTarget();
            Register<T2>();
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            base.UnregisterCallbacksFromTarget();
            Unregister<T2>();
        }
    }
}




