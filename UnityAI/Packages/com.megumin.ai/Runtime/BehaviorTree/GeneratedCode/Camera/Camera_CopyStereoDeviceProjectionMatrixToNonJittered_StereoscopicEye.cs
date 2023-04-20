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
    [Icon("Camera Icon")]
    [DisplayName("Camera_CopyStereoDeviceProjectionMatrixToNonJittered")]
    [Category("Unity/Camera")]
    [AddComponentMenu("CopyStereoDeviceProjectionMatrixToNonJittered(StereoscopicEye)")]
    public sealed class Camera_CopyStereoDeviceProjectionMatrixToNonJittered_StereoscopicEye : BTActionNode<UnityEngine.Camera>
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.Camera.StereoscopicEye> eye;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Camera)MyAgent).CopyStereoDeviceProjectionMatrixToNonJittered(eye);
            return Status.Succeeded;
        }
    }
}




