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
    [DisplayName("Input_GetTouch")]
    [Category("Unity/Input")]
    [AddComponentMenu("GetTouch(Int32)")]
    public sealed class Input_GetTouch_Int32 : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Int index;

        public Megumin.Binding.RefVar<UnityEngine.Touch> Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Input.GetTouch(index);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




