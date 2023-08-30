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
    [AddComponentMenu("Raycast(Vector3, Vector3)")]
    public sealed class Physics_Raycast_Vector3_Vector3_Method_Decorator : ConditionDecorator
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 origin;
        public Megumin.Binding.RefVar_Vector3 direction;

        [Space]
        public Megumin.Binding.RefVar_Bool SaveValueTo;

        public override bool CheckCondition(object options = null)
        {
            var result = UnityEngine.Physics.Raycast(origin, direction);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return result;
        }

    }
}




