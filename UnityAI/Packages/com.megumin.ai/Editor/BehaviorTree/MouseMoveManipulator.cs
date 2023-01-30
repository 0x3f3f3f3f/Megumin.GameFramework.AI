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
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            Debug.Log($"{evt.localMousePosition}   {evt.ToStringReflection()}" );
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            mouseMove?.Invoke(evt);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }
    }
}
