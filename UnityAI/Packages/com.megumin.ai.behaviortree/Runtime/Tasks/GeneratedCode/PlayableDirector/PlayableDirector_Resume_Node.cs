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
    [DisplayName("PlayableDirector_Resume")]
    [Category("UnityEngine/PlayableDirector")]
    [AddComponentMenu("Resume")]
    public sealed class PlayableDirector_Resume_Node : BTActionNode<UnityEngine.Playables.PlayableDirector>
    {
        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Playables.PlayableDirector)MyAgent).Resume();
            return Status.Succeeded;
        }
    }
}



