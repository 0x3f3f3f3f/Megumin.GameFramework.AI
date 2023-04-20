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
    [DisplayName("Physics_Linecast")]
    [Category("Unity/Physics")]
    [AddComponentMenu("Linecast(Vector3, Vector3, Int32)")]
    public sealed class Physics_Linecast_Vector3_Vector3_Int32 : ConditionDecorator
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 start;
        public Megumin.Binding.RefVar_Vector3 end;
        public Megumin.Binding.RefVar_Int layerMask;

        public Megumin.Binding.RefVar_Bool Result;

        public override bool CheckCondition(object options = null)
        {
            var result = UnityEngine.Physics.Linecast(start, end, layerMask);

            if (Result != null)
            {
                Result.Value = result;
            }

            return result;
        }
    }
}




