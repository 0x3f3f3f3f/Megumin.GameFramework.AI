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
    [Icon("d_ParticleSystem Icon")]
    [DisplayName("ParticleSystem_Simulate")]
    [Category("UnityEngine/ParticleSystem")]
    [AddComponentMenu("Simulate(Single, Boolean, Boolean)")]
    public sealed class ParticleSystem_Simulate_Single_Boolean_Boolean_Node : BTActionNode<UnityEngine.ParticleSystem>
    {
        [Space]
        public Megumin.Binding.RefVar_Float t;
        public Megumin.Binding.RefVar_Bool withChildren;
        public Megumin.Binding.RefVar_Bool restart;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.ParticleSystem)MyAgent).Simulate(t, withChildren, restart);
            return Status.Succeeded;
        }
    }
}




