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
    [DisplayName("Physics_BoxCast")]
    [Category("Unity/Physics")]
    [AddComponentMenu("BoxCast(Vector3, Vector3, Vector3, Quaternion)")]
    public sealed class Physics_BoxCast_Vector3_Vector3_Vector3_Quaternion : ConditionDecorator
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 center;
        public Megumin.Binding.RefVar_Vector3 halfExtents;
        public Megumin.Binding.RefVar_Vector3 direction;
        public Megumin.Binding.RefVar<UnityEngine.Quaternion> orientation;

        public Megumin.Binding.RefVar_Bool Result;

        public override bool CheckCondition(object options = null)
        {
            var result = UnityEngine.Physics.BoxCast(center, halfExtents, direction, orientation);

            if (Result != null)
            {
                Result.Value = result;
            }

            return result;
        }
    }
}




