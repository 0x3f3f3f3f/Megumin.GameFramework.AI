﻿///********************************************************************************************************************************
///The code on this page is generated by the code generator, do not manually modify.
///CodeGenerator: Megumin.CSCodeGenerator.  Version: 1.0.1
///CodeGenericBy: Megumin.AI.BehaviorTree.Editor.NodeGenerator
///CodeGenericSourceFilePath: Packages/com.megumin.ai/Editor/BehaviorTree/Generator/InputSystem.asset
///********************************************************************************************************************************

#if ENABLE_INPUT_SYSTEM

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_DefaultAsset Icon")]
    [DisplayName("InputActionAsset_FindControlScheme")]
    [Category("UnityEngine/InputActionAsset")]
    [AddComponentMenu("FindControlScheme(String)")]
    public sealed class InputActionAsset_FindControlScheme_String_Node : BTActionNode
    {
        [Space]
        public UnityEngine.InputSystem.InputActionAsset MyAgent;

        [Space]
        public Megumin.Binding.RefVar_String name;

        [Space]
        public Megumin.Binding.RefVar<System.Nullable<UnityEngine.InputSystem.InputControlScheme>> SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.InputSystem.InputActionAsset)MyAgent).FindControlScheme(name);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}

#endif



