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
    [DisplayName("Animator_PlayInFixedTime")]
    [Category("Unity/Animator")]
    [AddComponentMenu("PlayInFixedTime(Int32, Int32, Single)")]
    public sealed class Animator_PlayInFixedTime_Int32_Int32_Single : BTActionNode<UnityEngine.Animator>
    {
        [Space]
        public Megumin.Binding.RefVar_Int stateNameHash;
        public Megumin.Binding.RefVar_Int layer;
        public Megumin.Binding.RefVar_Float fixedTime;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Animator)MyAgent).PlayInFixedTime(stateNameHash, layer, fixedTime);
            return Status.Succeeded;
        }
    }
}




