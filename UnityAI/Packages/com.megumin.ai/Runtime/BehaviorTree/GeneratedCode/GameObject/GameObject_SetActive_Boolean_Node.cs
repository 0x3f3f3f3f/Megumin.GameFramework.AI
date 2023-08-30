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
    [DisplayName("GameObject_SetActive")]
    [Category("UnityEngine/GameObject")]
    [AddComponentMenu("SetActive(Boolean)")]
    public sealed class GameObject_SetActive_Boolean_Node : BTActionNode<UnityEngine.GameObject>
    {
        [Space]
        public Megumin.Binding.RefVar_Bool value;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.GameObject)MyAgent).SetActive(value);
            return Status.Succeeded;
        }
    }
}




