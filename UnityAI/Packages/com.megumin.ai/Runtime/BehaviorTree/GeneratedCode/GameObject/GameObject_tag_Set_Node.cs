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
    [Icon("d_GameObject Icon")]
    [DisplayName("Set_GameObject_tag")]
    [Category("UnityEngine/GameObject")]
    [AddComponentMenu("Set_tag")]
    public sealed class GameObject_tag_Set_Node : BTActionNode<UnityEngine.GameObject>
    {
        [Space]
        public Megumin.Binding.RefVar_String Value;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.GameObject)MyAgent).tag = Value;
            return Status.Succeeded;
        }

    }
}




