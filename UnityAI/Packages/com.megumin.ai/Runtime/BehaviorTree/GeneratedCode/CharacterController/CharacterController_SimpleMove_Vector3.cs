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
    [Icon("CharacterController Icon")]
    [DisplayName("CharacterController_SimpleMove")]
    [Category("Unity/CharacterController")]
    [AddComponentMenu("SimpleMove(Vector3)")]
    public sealed class CharacterController_SimpleMove_Vector3 : ConditionDecorator<UnityEngine.CharacterController>
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 speed;

        public Megumin.Binding.RefVar_Bool Result;

        public override bool CheckCondition(object options = null)
        {
            var result = ((UnityEngine.CharacterController)MyAgent).SimpleMove(speed);

            if (Result != null)
            {
                Result.Value = result;
            }

            return result;
        }
    }
}




