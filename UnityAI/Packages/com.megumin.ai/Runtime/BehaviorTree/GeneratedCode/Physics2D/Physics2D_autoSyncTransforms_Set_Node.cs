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
    [DisplayName("Set_Physics2D_autoSyncTransforms")]
    [Category("UnityEngine/Physics2D")]
    [AddComponentMenu("Set_autoSyncTransforms")]
    public sealed class Physics2D_autoSyncTransforms_Set_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Bool Value;

        protected override Status OnTick(BTNode from, object options = null)
        {
            UnityEngine.Physics2D.autoSyncTransforms = Value;
            return Status.Succeeded;
        }

    }
}




