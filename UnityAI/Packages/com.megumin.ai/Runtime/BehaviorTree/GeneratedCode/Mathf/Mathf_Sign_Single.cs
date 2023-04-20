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
    [DisplayName("Mathf_Sign")]
    [Category("Unity/Mathf")]
    [AddComponentMenu("Sign(Single)")]
    public sealed class Mathf_Sign_Single : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Float f;

        public Megumin.Binding.RefVar_Float Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Mathf.Sign(f);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




