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
    [DisplayName("ParticleSystem_Clear")]
    [Category("UnityEngine/ParticleSystem")]
    [AddComponentMenu("Clear")]
    public sealed class ParticleSystem_Clear_Node : BTActionNode<UnityEngine.ParticleSystem>
    {
        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.ParticleSystem)MyAgent).Clear();
            return Status.Succeeded;
        }
    }
}



