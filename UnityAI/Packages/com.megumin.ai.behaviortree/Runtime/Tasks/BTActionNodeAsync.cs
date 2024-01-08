using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    ///// <summary>
    ///// 异步接口 override <see cref="PlayAsync"/>
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public class BTActionNodeAsync<T> : BTActionNode<T>
    //{
    //    private ValueTask<bool> runTask;

    //    protected override void OnEnter(object options = null)
    //    {
    //        base.OnEnter(options);
    //        runTask = PlayAsync();
    //    }


    //    protected override Status OnTick(BTNode from, object options = null)
    //    {
    //        if (runTask.IsCompleted)
    //        {
    //            return runTask.Result ? Status.Succeeded : Status.Failed;
    //        }
    //        return Status.Running;
    //    }

    //    public virtual ValueTask<bool> PlayAsync()
    //    {
    //        return new ValueTask<bool>(result: true);
    //    }
    //}
}



