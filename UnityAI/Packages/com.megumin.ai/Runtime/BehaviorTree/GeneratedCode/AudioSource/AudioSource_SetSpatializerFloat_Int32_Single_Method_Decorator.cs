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
    [DisplayName("AudioSource_SetSpatializerFloat")]
    [Category("UnityEngine/AudioSource")]
    [AddComponentMenu("SetSpatializerFloat(Int32, Single)")]
    public sealed class AudioSource_SetSpatializerFloat_Int32_Single_Method_Decorator : ConditionDecorator<UnityEngine.AudioSource>
    {
        [Space]
        public Megumin.Binding.RefVar_Int index;
        public Megumin.Binding.RefVar_Float value;

        [Space]
        public Megumin.Binding.RefVar_Bool SaveValueTo;

        public override bool CheckCondition(object options = null)
        {
            var result = ((UnityEngine.AudioSource)MyAgent).SetSpatializerFloat(index, value);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return result;
        }

    }
}




