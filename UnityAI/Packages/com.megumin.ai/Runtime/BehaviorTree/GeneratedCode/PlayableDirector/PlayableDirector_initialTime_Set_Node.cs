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
    [Icon("d_PlayableDirector Icon")]
    [DisplayName("Set_PlayableDirector_initialTime")]
    [Category("UnityEngine/PlayableDirector")]
    [AddComponentMenu("Set_initialTime")]
    public sealed class PlayableDirector_initialTime_Set_Node : BTActionNode<UnityEngine.Playables.PlayableDirector>
    {
        [Space]
        public Megumin.Binding.RefVar_Double Value;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Playables.PlayableDirector)MyAgent).initialTime = Value;
            return Status.Succeeded;
        }

    }
}




