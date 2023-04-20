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
    [Icon("Animation Icon")]
    [DisplayName("Animation_CrossFade")]
    [Category("Unity/Animation")]
    [AddComponentMenu("CrossFade(String, Single)")]
    public sealed class Animation_CrossFade_String_Single : BTActionNode<UnityEngine.Animation>
    {
        [Space]
        public Megumin.Binding.RefVar_String animation;
        public Megumin.Binding.RefVar_Float fadeLength;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Animation)MyAgent).CrossFade(animation, fadeLength);
            return Status.Succeeded;
        }
    }
}




