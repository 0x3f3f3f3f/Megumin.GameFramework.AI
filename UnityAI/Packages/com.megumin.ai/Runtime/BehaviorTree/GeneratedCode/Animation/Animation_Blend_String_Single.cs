﻿///********************************************************************************************************************************
///本页代码由代码生成器生成，请勿手动修改。The code on this page is generated by the code generator, do not manually modify.
///生成器类型：Megumin.GameFramework.AI.BehaviorTree.Editor.NodeGeneraotr
///配置源文件：$(CodeGenericSourceFilePath)
///********************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Icon("Animation Icon")]
    [DisplayName("Animation_Blend")]
    [Category("Unity/Animation")]
    [AddComponentMenu("Blend(String, Single)")]
    public sealed class Animation_Blend_String_Single : BTActionNode<UnityEngine.Animation>
    {
        [Space]
        public Megumin.Binding.RefVar_String animation;
        public Megumin.Binding.RefVar_Float targetWeight;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Animation)MyAgent).Blend(animation, targetWeight);
            return Status.Succeeded;
        }
    }
}



