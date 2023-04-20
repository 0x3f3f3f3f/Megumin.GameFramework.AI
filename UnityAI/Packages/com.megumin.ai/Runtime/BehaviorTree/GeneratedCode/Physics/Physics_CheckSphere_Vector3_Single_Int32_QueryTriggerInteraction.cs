﻿///********************************************************************************************************************************
///本页代码由代码生成器生成，请勿手动修改。The code on this page is generated by the code generator, do not manually modify.
///生成器类型：$(CodeGenericType)
///配置源文件：$(CodeGenericSourceFilePath)
///********************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Icon("")]
    [DisplayName("Physics_CheckSphere")]
    [Category("Unity/Physics")]
    [AddComponentMenu("CheckSphere(Vector3, Single, Int32, QueryTriggerInteraction)")]
    public sealed class Physics_CheckSphere_Vector3_Single_Int32_QueryTriggerInteraction : ConditionDecorator
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 position;
        public Megumin.Binding.RefVar_Float radius;
        public Megumin.Binding.RefVar_Int layerMask;
        public Megumin.Binding.RefVar<UnityEngine.QueryTriggerInteraction> queryTriggerInteraction;

        public Megumin.Binding.RefVar_Bool Result;

        public override bool CheckCondition(object options = null)
        {
            var result = UnityEngine.Physics.CheckSphere(position, radius, layerMask, queryTriggerInteraction);

            if (Result != null)
            {
                Result.Value = result;
            }

            return result;
        }
    }
}




