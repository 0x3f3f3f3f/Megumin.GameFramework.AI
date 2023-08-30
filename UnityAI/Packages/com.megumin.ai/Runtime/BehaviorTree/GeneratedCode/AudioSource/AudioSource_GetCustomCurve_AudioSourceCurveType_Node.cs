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
    [DisplayName("AudioSource_GetCustomCurve")]
    [Category("UnityEngine/AudioSource")]
    [AddComponentMenu("GetCustomCurve(AudioSourceCurveType)")]
    public sealed class AudioSource_GetCustomCurve_AudioSourceCurveType_Node : BTActionNode<UnityEngine.AudioSource>
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.AudioSourceCurveType> type;

        [Space]
        public Megumin.Binding.RefVar_AnimationCurve SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.AudioSource)MyAgent).GetCustomCurve(type);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




