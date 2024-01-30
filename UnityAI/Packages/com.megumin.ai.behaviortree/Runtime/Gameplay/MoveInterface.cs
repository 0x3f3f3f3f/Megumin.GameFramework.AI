using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 可移动到目的地的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMoveToable<in T>
    {
        /// <summary>
        /// 设置目的地
        /// <para/> 通常改变目的地时调用一次即可，不用每帧调用。
        /// </summary>
        /// <param name="destination"></param>
        /// <returns>
        /// 是否成功设置目的地
        /// </returns>
        bool MoveTo(T destination, float stoppingDistance = 0);
    }

    /// <summary>
    /// 可向某个方向移动的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMoveInputable<in T>
    {
        /// <summary>
        /// 设置移动方向
        /// <para/> 通常每帧调用, 根据当前位置和目的地计算要移动的方向。
        /// </summary>
        /// <param name="moveDirection"></param>
        /// <returns></returns>
        void MoveInput(T moveDirection, float stoppingDistance = 0);
    }

}





