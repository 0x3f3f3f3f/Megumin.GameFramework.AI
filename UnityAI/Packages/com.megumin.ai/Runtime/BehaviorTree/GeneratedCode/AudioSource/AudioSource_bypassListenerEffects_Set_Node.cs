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
    [Icon("d_AudioSource Icon")]
    [DisplayName("Set_AudioSource_bypassListenerEffects")]
    [Category("UnityEngine/AudioSource")]
    [AddComponentMenu("Set_bypassListenerEffects")]
    public sealed class AudioSource_bypassListenerEffects_Set_Node : BTActionNode<UnityEngine.AudioSource>
    {
        [Space]
        public Megumin.Binding.RefVar_Bool Value;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.AudioSource)MyAgent).bypassListenerEffects = Value;
            return Status.Succeeded;
        }

    }
}




