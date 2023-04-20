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
    [DisplayName("Camera_SetTargetBuffers")]
    [Category("Unity/Camera")]
    [AddComponentMenu("SetTargetBuffers(RenderBuffer, RenderBuffer)")]
    public sealed class Camera_SetTargetBuffers_RenderBuffer_RenderBuffer : BTActionNode<UnityEngine.Camera>
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.RenderBuffer> colorBuffer;
        public Megumin.Binding.RefVar<UnityEngine.RenderBuffer> depthBuffer;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Camera)MyAgent).SetTargetBuffers(colorBuffer, depthBuffer);
            return Status.Succeeded;
        }
    }
}




