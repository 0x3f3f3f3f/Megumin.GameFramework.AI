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
    [DisplayName("SceneManager_GetSceneByBuildIndex")]
    [Category("UnityEngine/SceneManager")]
    [AddComponentMenu("GetSceneByBuildIndex(Int32)")]
    public sealed class SceneManager_GetSceneByBuildIndex_Int32_Node : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Int buildIndex;

        [Space]
        public Megumin.Binding.RefVar<UnityEngine.SceneManagement.Scene> SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(buildIndex);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




