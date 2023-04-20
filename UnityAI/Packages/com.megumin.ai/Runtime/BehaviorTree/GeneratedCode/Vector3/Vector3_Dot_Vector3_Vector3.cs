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
    [DisplayName("Vector3_Dot")]
    [Category("Unity/Vector3")]
    [AddComponentMenu("Dot(Vector3, Vector3)")]
    public sealed class Vector3_Dot_Vector3_Vector3 : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 lhs;
        public Megumin.Binding.RefVar_Vector3 rhs;

        public Megumin.Binding.RefVar_Float Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Vector3.Dot(lhs, rhs);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




