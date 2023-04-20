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
    [Icon("Animator Icon")]
    [DisplayName("Animator_IsParameterControlledByCurve")]
    [Category("Unity/Animator")]
    [AddComponentMenu("IsParameterControlledByCurve(String)")]
    public sealed class Animator_IsParameterControlledByCurve_String : ConditionDecorator<UnityEngine.Animator>
    {
        [Space]
        public Megumin.Binding.RefVar_String name;

        public Megumin.Binding.RefVar_Bool Result;

        public override bool CheckCondition(object options = null)
        {
            var result = ((UnityEngine.Animator)MyAgent).IsParameterControlledByCurve(name);

            if (Result != null)
            {
                Result.Value = result;
            }

            return result;
        }
    }
}




