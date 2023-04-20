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
    [DisplayName("Vector2_ClampMagnitude")]
    [Category("Unity/Vector2")]
    [AddComponentMenu("ClampMagnitude(Vector2, Single)")]
    public sealed class Vector2_ClampMagnitude_Vector2_Single : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector2 vector;
        public Megumin.Binding.RefVar_Float maxLength;

        public Megumin.Binding.RefVar_Vector2 Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Vector2.ClampMagnitude(vector, maxLength);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




