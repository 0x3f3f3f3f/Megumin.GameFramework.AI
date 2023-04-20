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
    [Icon("CharacterController Icon")]
    [DisplayName("CharacterController_Move")]
    [Category("Unity/CharacterController")]
    [AddComponentMenu("Move(Vector3)")]
    public sealed class CharacterController_Move_Vector3 : BTActionNode<UnityEngine.CharacterController>
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 motion;

        public Megumin.Binding.RefVar<UnityEngine.CollisionFlags> Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.CharacterController)MyAgent).Move(motion);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




