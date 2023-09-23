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
    /// <summary>
    /// 持续循环知道返回想要的结果。
    /// </summary>
    [DisplayName("LoopUntil")]
    [SerializationAlias("Megumin.AI.BehaviorTree.LoopUntil")]
    [HelpURL(URL.WikiDecorator + "LoopUntil_Decorator")]
    public class LoopUntil_Decorator : BTDecorator, IPostDecorator
    {
        public Status Result = Status.Succeeded;
        public Status AfterNodeExit(Status result, object options = null)
        {
            if (result == Result)
            {
                return result;
            }
            return Status.Running;
        }
    }
}
