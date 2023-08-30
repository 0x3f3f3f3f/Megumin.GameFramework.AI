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
    [Icon("d_Transform Icon")]
    [DisplayName("Transform_SetPositionAndRotation")]
    [Category("UnityEngine/Transform")]
    [AddComponentMenu("SetPositionAndRotation(Vector3, Quaternion)")]
    public sealed class Transform_SetPositionAndRotation_Vector3_Quaternion_Node : BTActionNode<UnityEngine.Transform>
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 position;
        public Megumin.Binding.RefVar<UnityEngine.Quaternion> rotation;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Transform)MyAgent).SetPositionAndRotation(position, rotation);
            return Status.Succeeded;
        }
    }
}




