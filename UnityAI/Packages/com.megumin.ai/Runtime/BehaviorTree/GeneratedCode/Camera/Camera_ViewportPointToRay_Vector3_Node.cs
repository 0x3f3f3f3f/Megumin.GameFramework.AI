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
    [DisplayName("Camera_ViewportPointToRay")]
    [Category("UnityEngine/Camera")]
    [AddComponentMenu("ViewportPointToRay(Vector3)")]
    public sealed class Camera_ViewportPointToRay_Vector3_Node : BTActionNode<UnityEngine.Camera>
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 pos;

        [Space]
        public Megumin.Binding.RefVar<UnityEngine.Ray> SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Camera)MyAgent).ViewportPointToRay(pos);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




