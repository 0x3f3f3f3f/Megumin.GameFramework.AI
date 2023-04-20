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
    [Icon("Rigidbody Icon")]
    [DisplayName("Rigidbody_GetRelativePointVelocity")]
    [Category("Unity/Rigidbody")]
    [AddComponentMenu("GetRelativePointVelocity(Vector3)")]
    public sealed class Rigidbody_GetRelativePointVelocity_Vector3 : BTActionNode<UnityEngine.Rigidbody>
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 relativePoint;

        public Megumin.Binding.RefVar_Vector3 Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Rigidbody)MyAgent).GetRelativePointVelocity(relativePoint);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




