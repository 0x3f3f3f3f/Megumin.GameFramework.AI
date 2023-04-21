﻿///********************************************************************************************************************************
///本页代码由代码生成器生成，请勿手动修改。The code on this page is generated by the code generator, do not manually modify.
///生成器类型：Megumin.GameFramework.AI.BehaviorTree.Editor.NodeGeneraotr
///配置源文件：$(CodeGenericSourceFilePath)
///********************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [DisplayName("Physics_SphereCast")]
    [Category("Unity/Physics")]
    [AddComponentMenu("SphereCast(Ray, Single, Single, Int32, QueryTriggerInteraction)")]
    public sealed class Physics_SphereCast_Ray_Single_Single_Int32_QueryTriggerInteraction : ConditionDecorator
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.Ray> ray;
        public Megumin.Binding.RefVar_Float radius;
        public Megumin.Binding.RefVar_Float maxDistance;
        public Megumin.Binding.RefVar_Int layerMask;
        public Megumin.Binding.RefVar<UnityEngine.QueryTriggerInteraction> queryTriggerInteraction;

        [Space]
        public Megumin.Binding.RefVar_Bool SaveValueTo;

        public override bool CheckCondition(object options = null)
        {
            var result = UnityEngine.Physics.SphereCast(ray, radius, maxDistance, layerMask, queryTriggerInteraction);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return result;
        }
    }
}



