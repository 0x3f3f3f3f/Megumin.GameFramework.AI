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
    [DisplayName("Get_AudioSource_spatializePostEffects")]
    [Category("UnityEngine/AudioSource")]
    [AddComponentMenu("Get_spatializePostEffects")]
    public sealed class AudioSource_spatializePostEffects_Get_Node : BTActionNode<UnityEngine.AudioSource>
    {
        [Space]
        public Megumin.Binding.RefVar_Bool SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.AudioSource)MyAgent).spatializePostEffects;

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }

    }
}




