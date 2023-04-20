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
    [DisplayName("Animator_GetCurrentAnimatorStateInfo")]
    [Category("Unity/Animator")]
    [AddComponentMenu("GetCurrentAnimatorStateInfo(Int32)")]
    public sealed class Animator_GetCurrentAnimatorStateInfo_Int32 : BTActionNode<UnityEngine.Animator>
    {
        [Space]
        public Megumin.Binding.RefVar_Int layerIndex;

        public Megumin.Binding.RefVar<UnityEngine.AnimatorStateInfo> Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Animator)MyAgent).GetCurrentAnimatorStateInfo(layerIndex);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




