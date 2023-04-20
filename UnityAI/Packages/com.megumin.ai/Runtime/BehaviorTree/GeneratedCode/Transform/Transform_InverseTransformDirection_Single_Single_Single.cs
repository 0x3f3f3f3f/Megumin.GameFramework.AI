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
    [DisplayName("Transform_InverseTransformDirection")]
    [Category("Unity/Transform")]
    [AddComponentMenu("InverseTransformDirection(Single, Single, Single)")]
    public sealed class Transform_InverseTransformDirection_Single_Single_Single : BTActionNode<UnityEngine.Transform>
    {
        [Space]
        public Megumin.Binding.RefVar_Float x;
        public Megumin.Binding.RefVar_Float y;
        public Megumin.Binding.RefVar_Float z;

        public Megumin.Binding.RefVar_Vector3 Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Transform)MyAgent).InverseTransformDirection(x, y, z);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




