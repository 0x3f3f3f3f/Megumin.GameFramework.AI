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
    [DisplayName("Rigidbody_AddForce")]
    [Category("UnityEngine/Rigidbody")]
    [AddComponentMenu("AddForce(Single, Single, Single)")]
    public sealed class Rigidbody_AddForce_Single_Single_Single_Node : BTActionNode<UnityEngine.Rigidbody>
    {
        [Space]
        public Megumin.Binding.RefVar_Float x;
        public Megumin.Binding.RefVar_Float y;
        public Megumin.Binding.RefVar_Float z;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Rigidbody)MyAgent).AddForce(x, y, z);
            return Status.Succeeded;
        }
    }
}




