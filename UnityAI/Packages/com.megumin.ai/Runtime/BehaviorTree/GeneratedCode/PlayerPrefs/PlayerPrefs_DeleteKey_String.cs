﻿///********************************************************************************************************************************
///本页代码由代码生成器生成，请勿手动修改。The code on this page is generated by the code generator, do not manually modify.
///生成器类型：Megumin.GameFramework.AI.BehaviorTree.Editor.NodeGeneraotr
///配置源文件：$(CodeGenericSourceFilePath)
///********************************************************************************************************************************

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [DisplayName("PlayerPrefs_DeleteKey")]
    [Category("Unity/PlayerPrefs")]
    [AddComponentMenu("DeleteKey(String)")]
    public sealed class PlayerPrefs_DeleteKey_String : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_String key;

        protected override Status OnTick(BTNode from, object options = null)
        {
            UnityEngine.PlayerPrefs.DeleteKey(key);
            return Status.Succeeded;
        }
    }
}



