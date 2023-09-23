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
    public enum InputType
    {
        Default,
        Down,
        Up
    }

    /// <summary>
    /// 检查按键
    /// </summary>
    [DisplayName("KeyCodeEvent")]
    [SerializationAlias("Megumin.AI.BehaviorTree.KeyCodeEvent")]
    [HelpURL(URL.WikiDecorator + "KeyCodeEvent_Decorator")]
    public class KeyCodeEvent_Decorator : ConditionDecorator, IConditionDecorator
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
