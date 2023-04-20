﻿///********************************************************************************************************************************
///本页代码由代码生成器生成，请勿手动修改。The code on this page is generated by the code generator, do not manually modify.
///生成器类型：$(CodeGenericType)
///配置源文件：$(CodeGenericSourceFilePath)
///********************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Icon("")]
    [DisplayName("SceneManager_CreateScene")]
    [Category("Unity/SceneManager")]
    [AddComponentMenu("CreateScene(String)")]
    public sealed class SceneManager_CreateScene_String : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_String sceneName;

        public Megumin.Binding.RefVar<UnityEngine.SceneManagement.Scene> Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.SceneManagement.SceneManager.CreateScene(sceneName);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




