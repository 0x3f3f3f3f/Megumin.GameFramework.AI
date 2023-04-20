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
    [DisplayName("Camera_ResetWorldToCameraMatrix")]
    [Category("Unity/Camera")]
    [AddComponentMenu("ResetWorldToCameraMatrix")]
    public sealed class Camera_ResetWorldToCameraMatrix : BTActionNode<UnityEngine.Camera>
    {

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Camera)MyAgent).ResetWorldToCameraMatrix();
            return Status.Succeeded;
        }
    }
}




