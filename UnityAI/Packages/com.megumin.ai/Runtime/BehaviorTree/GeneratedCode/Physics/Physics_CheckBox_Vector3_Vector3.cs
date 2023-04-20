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
    [DisplayName("Physics_CheckBox")]
    [Category("Unity/Physics")]
    [AddComponentMenu("CheckBox(Vector3, Vector3)")]
    public sealed class Physics_CheckBox_Vector3_Vector3 : ConditionDecorator
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 center;
        public Megumin.Binding.RefVar_Vector3 halfExtents;

        public Megumin.Binding.RefVar_Bool Result;

        public override bool CheckCondition(object options = null)
        {
            var result = UnityEngine.Physics.CheckBox(center, halfExtents);

            if (Result != null)
            {
                Result.Value = result;
            }

            return result;
        }
    }
}




