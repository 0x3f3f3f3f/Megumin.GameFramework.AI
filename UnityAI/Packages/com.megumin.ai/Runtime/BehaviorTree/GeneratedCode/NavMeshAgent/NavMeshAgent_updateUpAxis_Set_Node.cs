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
    [Icon("d_NavMeshAgent Icon")]
    [DisplayName("Set_NavMeshAgent_updateUpAxis")]
    [Category("UnityEngine/NavMeshAgent")]
    [AddComponentMenu("Set_updateUpAxis")]
    public sealed class NavMeshAgent_updateUpAxis_Set_Node : BTActionNode<UnityEngine.AI.NavMeshAgent>
    {
        [Space]
        public Megumin.Binding.RefVar_Bool Value;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.AI.NavMeshAgent)MyAgent).updateUpAxis = Value;
            return Status.Succeeded;
        }

    }
}




