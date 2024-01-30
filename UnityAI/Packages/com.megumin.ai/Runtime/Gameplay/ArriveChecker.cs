using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Megumin.AI
{
    /// <summary>
    /// 检查对象是否到达目标地点
    /// </summary>
    [Serializable]
    public class ArriveChecker
    {
        /// <summary>
        /// 手动设置的最大停止距离
        /// </summary>
        [SerializeField]
        [Range(0, 5)]
        private float stopingDistance = 0.25f;

        [Tooltip("Auto check Collider Radius.")]
        public bool UseColliderRadius = false;

        [ReadOnlyInInspector]
        public float radiusDistance = 0;

        /// <summary>
        /// 最终停止距离
        /// </summary>
        [ReadOnlyInInspector]
        public float FinalStopingDistance = 0;

        public float CalStopingDistance(GameObject me, GameObject target = null)
        {
            if (UseColliderRadius)
            {
                radiusDistance = 0;

                if (TryGetRadius(me, out var myRaidus))
                {
                    //自身半径
                    radiusDistance += myRaidus;
                }

                if (TryGetRadius(target, out var targetRaidus))
                {
                    //目标对象半径
                    radiusDistance += targetRaidus;
                }

                //不要紧挨着，最少扩大5%，0.1f;
                radiusDistance += Math.Max(0.1f, radiusDistance * 0.05f);
            }

            FinalStopingDistance = Math.Max(radiusDistance, stopingDistance);
            return FinalStopingDistance;
        }

        public static implicit operator float(ArriveChecker checker)
        {
            return checker?.FinalStopingDistance ?? 0;
        }

        /// <summary>
        /// 搜索3层组件，尝试找到对象的半径
        /// </summary>
        /// <param name="go"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool TryGetRadius(GameObject go, out float radius)
        {
            if (go != null)
            {
                foreach (var item in new TransformBFS(go.transform, 3))
                {
                    if (item.TryGetComponent<CharacterController>(out var characterController))
                    {
                        radius = characterController.radius;
                        return true;
                    }
                    else if (item.TryGetComponent<CapsuleCollider>(out var capsuleCollider))
                    {
                        if (capsuleCollider.isTrigger == false)
                        {
                            radius = capsuleCollider.radius;
                            return true;
                        }
                    }
                    else if (item.TryGetComponent<SphereCollider>(out var sphereCollider))
                    {
                        if (sphereCollider.isTrigger == false)
                        {
                            radius = sphereCollider.radius;
                            return true;
                        }
                    }
                }
            }

            radius = 0;
            return false;
        }

        [Space]
        [Tooltip("计算距离时，根据设置忽略指定轴向的距离，例如忽略Y轴就设置为 new Vector3(1, 0, 1)")]
        public Vector3 DistanceScale = new(1, 0, 1);

        public bool IsArrive(Transform transform,
                             Vector3 destination,
                             out float distance)
        {
            if (transform)
            {
                var to = destination - transform.position;

                to.Scale(DistanceScale);

                distance = to.magnitude;

                if (distance <= stopingDistance)
                {
                    return true;
                }
            }
            else
            {
                distance = float.MaxValue;
            }

            return false;
        }

        public bool IsArrive(Transform transform,
                             Vector3 destination)
        {
            return IsArrive(transform, destination, out var _);
        }
    }
}

