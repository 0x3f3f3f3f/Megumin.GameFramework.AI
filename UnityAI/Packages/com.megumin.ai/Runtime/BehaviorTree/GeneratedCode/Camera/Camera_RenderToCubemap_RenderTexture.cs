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
    [DisplayName("Camera_RenderToCubemap")]
    [Category("Unity/Camera")]
    [AddComponentMenu("RenderToCubemap(RenderTexture)")]
    public sealed class Camera_RenderToCubemap_RenderTexture : ConditionDecorator<UnityEngine.Camera>
    {
        [Space]
        public Megumin.Binding.RefVar_RenderTexture cubemap;

        public Megumin.Binding.RefVar_Bool Result;

        public override bool CheckCondition(object options = null)
        {
            var result = ((UnityEngine.Camera)MyAgent).RenderToCubemap(cubemap);

            if (Result != null)
            {
                Result.Value = result;
            }

            return result;
        }
    }
}




