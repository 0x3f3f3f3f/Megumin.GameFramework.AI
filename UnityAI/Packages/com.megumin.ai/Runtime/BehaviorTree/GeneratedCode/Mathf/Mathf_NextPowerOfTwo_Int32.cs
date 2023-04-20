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
    [DisplayName("Mathf_NextPowerOfTwo")]
    [Category("Unity/Mathf")]
    [AddComponentMenu("NextPowerOfTwo(Int32)")]
    public sealed class Mathf_NextPowerOfTwo_Int32 : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Int value;

        public Megumin.Binding.RefVar_Int Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Mathf.NextPowerOfTwo(value);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




