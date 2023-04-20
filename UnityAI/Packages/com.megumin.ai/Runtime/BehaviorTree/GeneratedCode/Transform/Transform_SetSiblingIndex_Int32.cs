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
    [Icon("Transform Icon")]
    [DisplayName("Transform_SetSiblingIndex")]
    [Category("Unity/Transform")]
    [AddComponentMenu("SetSiblingIndex(Int32)")]
    public sealed class Transform_SetSiblingIndex_Int32 : BTActionNode<UnityEngine.Transform>
    {
        [Space]
        public Megumin.Binding.RefVar_Int index;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Transform)MyAgent).SetSiblingIndex(index);
            return Status.Succeeded;
        }
    }
}




