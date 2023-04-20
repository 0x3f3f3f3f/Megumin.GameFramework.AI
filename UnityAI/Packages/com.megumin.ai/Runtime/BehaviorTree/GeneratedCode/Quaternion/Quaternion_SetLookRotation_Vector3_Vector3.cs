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
    [DisplayName("Quaternion_SetLookRotation")]
    [Category("Unity/Quaternion")]
    [AddComponentMenu("SetLookRotation(Vector3, Vector3)")]
    public sealed class Quaternion_SetLookRotation_Vector3_Vector3 : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.Quaternion> MyAgent;

        [Space]
        public Megumin.Binding.RefVar_Vector3 view;
        public Megumin.Binding.RefVar_Vector3 up;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Quaternion)MyAgent).SetLookRotation(view, up);
            return Status.Succeeded;
        }
    }
}




