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
    [DisplayName("Physics_SphereCast")]
    [Category("Unity/Physics")]
    [AddComponentMenu("SphereCast(Ray, Single, Single)")]
    public sealed class Physics_SphereCast_Ray_Single_Single : ConditionDecorator
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.Ray> ray;
        public Megumin.Binding.RefVar_Float radius;
        public Megumin.Binding.RefVar_Float maxDistance;

        public Megumin.Binding.RefVar_Bool Result;

        public override bool CheckCondition(object options = null)
        {
            var result = UnityEngine.Physics.SphereCast(ray, radius, maxDistance);

            if (Result != null)
            {
                Result.Value = result;
            }

            return result;
        }
    }
}




