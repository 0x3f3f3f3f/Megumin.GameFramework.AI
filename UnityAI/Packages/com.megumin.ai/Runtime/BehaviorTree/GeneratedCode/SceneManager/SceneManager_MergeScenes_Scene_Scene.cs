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
    [DisplayName("SceneManager_MergeScenes")]
    [Category("Unity/SceneManager")]
    [AddComponentMenu("MergeScenes(Scene, Scene)")]
    public sealed class SceneManager_MergeScenes_Scene_Scene : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.SceneManagement.Scene> sourceScene;
        public Megumin.Binding.RefVar<UnityEngine.SceneManagement.Scene> destinationScene;

        protected override Status OnTick(BTNode from, object options = null)
        {
            UnityEngine.SceneManagement.SceneManager.MergeScenes(sourceScene, destinationScene);
            return Status.Succeeded;
        }
    }
}




