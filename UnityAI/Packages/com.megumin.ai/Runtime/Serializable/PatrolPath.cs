using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Megumin.Binding;
using Random = UnityEngine.Random;

namespace Megumin.AI
{
    /// <summary>
    /// 巡逻路径
    /// </summary>
    [Serializable]
    public abstract class PatrolPath<T>
    {
        /// <summary>
        /// 每次获取新的目标点时，是否随机便宜一段距离
        /// </summary>
        public bool UseRandom = false;
        public float MaxOffset = 2;
        public float MinOffset = 0.5f;

        public T StartPoint { get; set; }

        /// <summary>
        /// 尝试获取下一个检查点
        /// </summary>
        /// <param name="mover">正在移动的对象</param>
        /// <param name="next"></param>
        /// <returns></returns>
        public abstract bool TryGetNextDestination(Transform mover, out Vector3 next);

        public virtual void Reset()
        {

        }
    }

    /// <summary>
    /// 检查点巡逻路径
    /// </summary>
    [Serializable]
    public class PatrolPathTranformList : PatrolPath<Vector3>
    {
        [Space]
        public RefVar_Transform_List DestinationList;
        int lastDesIndex = 0;
        public override bool TryGetNextDestination(Transform mover, out Vector3 next)
        {
            var list = DestinationList?.Value;
            if (list != null && list.Count != 0)
            {
                int startIndex = lastDesIndex % list.Count;
                for (int i = startIndex; i < list.Count; i++)
                {
                    lastDesIndex++;
                    Transform transform = list[i].transform;
                    if (transform)
                    {
                        next = transform.position;
                        if (UseRandom)
                        {
                            var offset = Random.insideUnitCircle * Random.Range(MinOffset, MaxOffset);
                            next += new Vector3(offset.x, 0, offset.y);
                        }
                        return true;
                    }
                }
            }
            next = default;
            return false;
        }

        public override void Reset()
        {
            lastDesIndex = 0;
        }
    }

    /// <summary>
    /// 起始点周围随机路径
    /// </summary>
    [Serializable]
    public class PatrolPathAroundStart : PatrolPath<Vector3>
    {
        [Space]
        public float MaxRange = 12f;
        public float MinRange = 4f;

        [Space]
        public float MinDistance2Current = 6f;

        public override bool TryGetNextDestination(Transform mover, out Vector3 next)
        {
            int count = 0;
            while (true)
            {
                var random = Random.insideUnitCircle * Random.Range(MinRange, MaxRange);
                next = StartPoint + new Vector3(random.x, 0, random.y);
                count++;
                if ((mover.position - next).magnitude > MinDistance2Current || count > 20)
                {
                    //随机的下一个点 必须与当前位置 距离足够远
                    return true;
                }
            }
        }
    }

}
