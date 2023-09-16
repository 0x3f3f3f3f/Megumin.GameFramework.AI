using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Reflection;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("MouseEvent")]
    [SerializationAlias("Megumin.AI.BehaviorTree.MouseEvent")]
    public class MouseEvent_Decorator : ConditionDecorator, IConditionDecorator
    {
        public int MouseButton = 0;
        public InputType InputType = InputType.Default;

        protected override bool OnCheckCondition(object options = null)
        {
            if (InputType == InputType.Default)
            {
                return Input.GetMouseButton(MouseButton);
            }
            else if (InputType == InputType.Down)
            {
                return Input.GetMouseButtonDown(MouseButton);
            }
            else if (InputType == InputType.Up)
            {
                return Input.GetMouseButtonUp(MouseButton);
            }

            return false;
        }
    }
}
