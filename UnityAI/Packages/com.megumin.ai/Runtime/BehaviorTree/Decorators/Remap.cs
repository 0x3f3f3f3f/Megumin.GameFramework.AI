using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 改变行为的结果。强制成功，强制失败，结果取反Inverter
    /// </summary>
    public class Remap : BTDecorator, IPostDecorator
    {
        bool invers = true;
        public Status AfterNodeExit(Status result, BTNode bTNode)
        {
            var newResult = result;
            if (invers)
            {
                switch (result)
                {
                    case Status.Succeeded:
                        newResult = Status.Failed;
                        break;
                    case Status.Failed:
                        newResult = Status.Succeeded;
                        break;
                }
            }

            Debug.LogError($"{result}--{bTNode}--{newResult}");
            return newResult;
        }
    }

    /// <summary>
    /// 条件节点可能在Action节点后面，所以需要一个后置附件节点来修改返回值。
    /// 并且需要 设置执行条件，仅当某些条件时才执行。
    /// https://robohub.org/wp-content/uploads/2021/08/bt_mobile_robot_03.png
    /// 如果父节点是Selector，那么条件节点可能在Action节点后面，Action结果是Failed时才执行条件节点。
    /// 如果父节点是Sequence，那么条件节点可能在Action节点后面，Action结果是Succeeded时才执行条件节点。 
    /// 与之对应，后置附件节点需要设置执行条件，才能与标准行为树保持一致。
    /// </summary>
    class ConditionRemap
    {

    }
}
