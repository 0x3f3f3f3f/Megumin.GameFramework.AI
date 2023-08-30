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
    [Icon("d_Rigidbody Icon")]
    [DisplayName("Rigidbody_AddForceAtPosition")]
    [Category("UnityEngine/Rigidbody")]
    [AddComponentMenu("AddForceAtPosition(Vector3, Vector3)")]
    public sealed class Rigidbody_AddForceAtPosition_Vector3_Vector3_Node : BTActionNode<UnityEngine.Rigidbody>
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 force;
        public Megumin.Binding.RefVar_Vector3 position;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Rigidbody)MyAgent).AddForceAtPosition(force, position);
            return Status.Succeeded;
        }
    }
}




