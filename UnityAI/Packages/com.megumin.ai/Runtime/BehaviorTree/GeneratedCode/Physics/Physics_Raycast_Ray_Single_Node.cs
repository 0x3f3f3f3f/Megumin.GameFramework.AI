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
    [DisplayName("Physics_Raycast")]
    [Category("UnityEngine/Physics")]
    [AddComponentMenu("Raycast(Ray, Single)")]
    public sealed class Physics_Raycast_Ray_Single_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.Ray> ray;
        public Megumin.Binding.RefVar_Float maxDistance;

        [Space]
        public Megumin.Binding.RefVar_Bool SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Physics.Raycast(ray, maxDistance);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




