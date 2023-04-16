using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public enum InputType
    {
        Default,
        Down,
        Up
    }

    public class KeyCodeEvent : ConditionDecorator, IConditionDecorator
    {
        public KeyCode KeyCode = KeyCode.Space;
        public InputType InputType = InputType.Default;

        protected override bool OnCheckCondition(object options = null)
        {
            if (InputType == InputType.Default)
            {
                return Input.GetKeyDown(KeyCode);
            }
            else if (InputType == InputType.Down)
            {
                return Input.GetKeyDown(KeyCode);
            }
            else if (InputType == InputType.Up)
            {
                return Input.GetKeyUp(KeyCode);
            }

            return false;
        }
    }
}
