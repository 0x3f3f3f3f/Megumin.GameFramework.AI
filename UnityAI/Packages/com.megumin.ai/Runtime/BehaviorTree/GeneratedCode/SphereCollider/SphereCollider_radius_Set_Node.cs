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
    [Icon("d_SphereCollider Icon")]
    [DisplayName("Set_SphereCollider_radius")]
    [Category("UnityEngine/SphereCollider")]
    [AddComponentMenu("Set_radius")]
    public sealed class SphereCollider_radius_Set_Node : BTActionNode<UnityEngine.SphereCollider>
    {
        [Space]
        public Megumin.Binding.RefVar_Float Value;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.SphereCollider)MyAgent).radius = Value;
            return Status.Succeeded;
        }

    }
}




