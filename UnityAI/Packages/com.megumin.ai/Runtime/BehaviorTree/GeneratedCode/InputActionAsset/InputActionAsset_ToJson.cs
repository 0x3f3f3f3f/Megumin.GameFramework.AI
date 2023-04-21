﻿///********************************************************************************************************************************
///本页代码由代码生成器生成，请勿手动修改。The code on this page is generated by the code generator, do not manually modify.
///生成器类型：Megumin.GameFramework.AI.BehaviorTree.Editor.NodeGeneraotr
///配置源文件：$(CodeGenericSourceFilePath)
///********************************************************************************************************************************

#if ENABLE_INPUT_SYSTEM

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Icon("DefaultAsset Icon")]
    [DisplayName("InputActionAsset_ToJson")]
    [Category("Unity/InputActionAsset")]
    [AddComponentMenu("ToJson")]
    public sealed class InputActionAsset_ToJson : BTActionNode
    {
        [Space]
        public UnityEngine.InputSystem.InputActionAsset MyAgent;


        [Space]
        public Megumin.Binding.RefVar_String SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.InputSystem.InputActionAsset)MyAgent).ToJson();

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}

#endif



