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
    [DisplayName("Quaternion_Euler")]
    [Category("UnityEngine/Quaternion")]
    [AddComponentMenu("Euler(Vector3)")]
    public sealed class Quaternion_Euler_Vector3_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 euler;

        [Space]
        public Megumin.Binding.RefVar<UnityEngine.Quaternion> SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Quaternion.Euler(euler);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}



