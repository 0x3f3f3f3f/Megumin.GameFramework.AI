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
    [DisplayName("Mathf_Sin")]
    [Category("UnityEngine/Mathf")]
    [AddComponentMenu("Sin(Single)")]
    public sealed class Mathf_Sin_Single_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Float f;

        [Space]
        public Megumin.Binding.RefVar_Float SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Mathf.Sin(f);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




