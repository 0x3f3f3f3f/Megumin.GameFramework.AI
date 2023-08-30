﻿///********************************************************************************************************************************
///The code on this page is generated by the code generator, do not manually modify.
///CodeGenerator: Megumin.CSCodeGenerator.  Version: 1.0.1
///CodeGenericBy: Megumin.AI.BehaviorTree.Editor.NodeGenerator
///CodeGenericSourceFilePath: Packages/com.megumin.ai/Editor/BehaviorTree/Generator/NodeGeneraotr.asset
///********************************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("Physics2D_BoxCast")]
    [Category("UnityEngine/Physics2D")]
    [AddComponentMenu("BoxCast(Vector2, Vector2, Single, Vector2, Single, Int32, Single, Single)")]
    public sealed class Physics2D_BoxCast_Vector2_Vector2_Single_Vector2_Single_Int32_Single_Single_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector2 origin;
        public Megumin.Binding.RefVar_Vector2 size;
        public Megumin.Binding.RefVar_Float angle;
        public Megumin.Binding.RefVar_Vector2 direction;
        public Megumin.Binding.RefVar_Float distance;
        public Megumin.Binding.RefVar_Int layerMask;
        public Megumin.Binding.RefVar_Float minDepth;
        public Megumin.Binding.RefVar_Float maxDepth;

        [Space]
        public Megumin.Binding.RefVar<UnityEngine.RaycastHit2D> SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




