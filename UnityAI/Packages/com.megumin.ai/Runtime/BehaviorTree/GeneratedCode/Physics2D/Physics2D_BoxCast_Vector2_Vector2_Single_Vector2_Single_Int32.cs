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
    [DisplayName("Physics2D_BoxCast")]
    [Category("Unity/Physics2D")]
    [AddComponentMenu("BoxCast(Vector2, Vector2, Single, Vector2, Single, Int32)")]
    public sealed class Physics2D_BoxCast_Vector2_Vector2_Single_Vector2_Single_Int32 : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector2 origin;
        public Megumin.Binding.RefVar_Vector2 size;
        public Megumin.Binding.RefVar_Float angle;
        public Megumin.Binding.RefVar_Vector2 direction;
        public Megumin.Binding.RefVar_Float distance;
        public Megumin.Binding.RefVar_Int layerMask;

        public Megumin.Binding.RefVar<UnityEngine.RaycastHit2D> Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




