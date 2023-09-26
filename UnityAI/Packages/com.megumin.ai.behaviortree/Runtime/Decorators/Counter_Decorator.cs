using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 计数器装饰器
    /// </summary>
    [DisplayName("Counter")]
    [HelpURL(URL.WikiDecorator + "Counter_Decorator")]
    public class Counter_Decorator : BTDecorator, IConditionDecorator, IPreDecorator, IPostDecorator, IAbortDecorator
    {
        [Space]
        public NumberOperation OnCondition = NumberOperation.None;
        public NumberOperation OnEnter = NumberOperation.Increment;
        public NumberOperation OnExit = NumberOperation.None;
        public NumberOperation OnAbort = NumberOperation.None;

        [Space]
        public RefVar_Int Count;

        public bool LastCheckResult => true;
        public bool CheckCondition(object options = null)
        {
            Count.Value = OnCondition.OperateInt(Count);
            return true;
        }

        public void BeforeNodeEnter(object options = null)
        {
            Count.Value = OnEnter.OperateInt(Count);
        }

        public Status AfterNodeExit(Status result, object options = null)
        {
            Count.Value = OnExit.OperateInt(Count);
            return result;
        }

        public void OnNodeAbort(object options = null)
        {
            Count.Value = OnAbort.OperateInt(Count);
        }
    }
}



