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
    [Icon("d_Animator Icon")]
    [DisplayName("Animator_CrossFade")]
    [Category("UnityEngine/Animator")]
    [AddComponentMenu("CrossFade(Int32, Single)")]
    public sealed class Animator_CrossFade_Int32_Single_Node : BTActionNode<UnityEngine.Animator>
    {
        [Space]
        public Megumin.Binding.RefVar_Int stateHashName;
        public Megumin.Binding.RefVar_Float normalizedTransitionDuration;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Animator)MyAgent).CrossFade(stateHashName, normalizedTransitionDuration);
            return Status.Succeeded;
        }
    }
}




