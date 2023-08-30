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
    [DisplayName("Vector3Int_Scale")]
    [Category("UnityEngine/Vector3Int")]
    [AddComponentMenu("Scale(Vector3Int, Vector3Int)")]
    public sealed class Vector3Int_Scale_Vector3Int_Vector3Int_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3Int a;
        public Megumin.Binding.RefVar_Vector3Int b;

        [Space]
        public Megumin.Binding.RefVar_Vector3Int SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Vector3Int.Scale(a, b);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




