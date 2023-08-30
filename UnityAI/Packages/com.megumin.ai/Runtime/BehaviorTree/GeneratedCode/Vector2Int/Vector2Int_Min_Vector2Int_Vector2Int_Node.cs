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
    [DisplayName("Vector2Int_Min")]
    [Category("UnityEngine/Vector2Int")]
    [AddComponentMenu("Min(Vector2Int, Vector2Int)")]
    public sealed class Vector2Int_Min_Vector2Int_Vector2Int_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector2Int lhs;
        public Megumin.Binding.RefVar_Vector2Int rhs;

        [Space]
        public Megumin.Binding.RefVar_Vector2Int SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Vector2Int.Min(lhs, rhs);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




