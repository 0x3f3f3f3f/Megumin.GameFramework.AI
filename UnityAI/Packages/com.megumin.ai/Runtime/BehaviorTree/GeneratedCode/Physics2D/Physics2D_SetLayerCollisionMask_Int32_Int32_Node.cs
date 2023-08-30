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
    [DisplayName("Physics2D_SetLayerCollisionMask")]
    [Category("UnityEngine/Physics2D")]
    [AddComponentMenu("SetLayerCollisionMask(Int32, Int32)")]
    public sealed class Physics2D_SetLayerCollisionMask_Int32_Int32_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Int layer;
        public Megumin.Binding.RefVar_Int layerMask;

        protected override Status OnTick(BTNode from, object options = null)
        {
            UnityEngine.Physics2D.SetLayerCollisionMask(layer, layerMask);
            return Status.Succeeded;
        }
    }
}




