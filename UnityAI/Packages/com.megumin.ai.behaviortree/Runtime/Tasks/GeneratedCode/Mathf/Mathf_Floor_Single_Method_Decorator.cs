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
    [Icon("d_collabcreate icon")]
    [DisplayName("Mathf_Floor")]
    [Category("UnityEngine/Mathf")]
    [AddComponentMenu("Floor(Single)")]
    public sealed class Mathf_Floor_Single_Method_Decorator : CompareDecorator<float>
    {
        [Space]
        public Megumin.Binding.RefVar_Float f;

        [Space]
        public Megumin.Binding.RefVar_Float CompareTo;

        [Space]
        public Megumin.Binding.RefVar_Float SaveValueTo;

        public override float GetResult()
        {
            var result = UnityEngine.Mathf.Floor(f);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return result;
        }

        public override float GetCompareTo()
        {
            return CompareTo;
        }

    }
}



