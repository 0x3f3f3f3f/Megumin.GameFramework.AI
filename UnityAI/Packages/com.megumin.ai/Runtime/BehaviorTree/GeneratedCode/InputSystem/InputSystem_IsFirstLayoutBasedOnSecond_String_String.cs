﻿///********************************************************************************************************************************
///本页代码由代码生成器生成，请勿手动修改。The code on this page is generated by the code generator, do not manually modify.
///生成器类型：$(CodeGenericType)
///配置源文件：$(CodeGenericSourceFilePath)
///********************************************************************************************************************************

#if ENABLE_INPUT_SYSTEM

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Icon("")]
    [DisplayName("InputSystem_IsFirstLayoutBasedOnSecond")]
    [Category("Unity/InputSystem")]
    [AddComponentMenu("IsFirstLayoutBasedOnSecond(String, String)")]
    public sealed class InputSystem_IsFirstLayoutBasedOnSecond_String_String : ConditionDecorator
    {
        [Space]
        public Megumin.Binding.RefVar_String firstLayoutName;
        public Megumin.Binding.RefVar_String secondLayoutName;

        public Megumin.Binding.RefVar_Bool Result;

        public override bool CheckCondition(object options = null)
        {
            var result = UnityEngine.InputSystem.InputSystem.IsFirstLayoutBasedOnSecond(firstLayoutName, secondLayoutName);

            if (Result != null)
            {
                Result.Value = result;
            }

            return result;
        }
    }
}

#endif




