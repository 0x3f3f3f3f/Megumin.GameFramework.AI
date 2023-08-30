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
    [Icon("d_Camera Icon")]
    [DisplayName("Camera_ScreenToViewportPoint")]
    [Category("UnityEngine/Camera")]
    [AddComponentMenu("ScreenToViewportPoint(Vector3)")]
    public sealed class Camera_ScreenToViewportPoint_Vector3_Node : BTActionNode<UnityEngine.Camera>
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 position;

        [Space]
        public Megumin.Binding.RefVar_Vector3 SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Camera)MyAgent).ScreenToViewportPoint(position);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




