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
    [Icon("Camera Icon")]
    [DisplayName("Camera_SetStereoProjectionMatrix")]
    [Category("Unity/Camera")]
    [AddComponentMenu("SetStereoProjectionMatrix(StereoscopicEye, Matrix4x4)")]
    public sealed class Camera_SetStereoProjectionMatrix_StereoscopicEye_Matrix4x4 : BTActionNode<UnityEngine.Camera>
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.Camera.StereoscopicEye> eye;
        public Megumin.Binding.RefVar<UnityEngine.Matrix4x4> matrix;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Camera)MyAgent).SetStereoProjectionMatrix(eye, matrix);
            return Status.Succeeded;
        }
    }
}



