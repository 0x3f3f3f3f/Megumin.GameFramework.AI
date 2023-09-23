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
    /// 反转结果装饰器
    /// </summary>
    [DisplayName("Inverter")]
    [SerializationAlias("Megumin.AI.BehaviorTree.Inverter")]
    [HelpURL(URL.WikiDecorator + "Inverter_Decorator")]
    public class Inverter_Decorator : BTDecorator, IPostDecorator
    {
        public Status AfterNodeExit(Status result, object options = null)
        {
            //可能会出现结果是Running的情况，比如有LoopUntil在此之前执行，将结果改成了Running。
            //所以只处理成功和失败两种。
            if (result == Status.Failed)
            {
                return Status.Succeeded;
            }
            else if (result == Status.Succeeded)
            {
                return Status.Failed;
            }

            return result;
        }
    }
}
