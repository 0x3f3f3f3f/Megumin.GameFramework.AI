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
    [Icon("GameObject Icon")]
    [DisplayName("GameObject_BroadcastMessage")]
    [Category("Unity/GameObject")]
    [AddComponentMenu("BroadcastMessage(String)")]
    public sealed class GameObject_BroadcastMessage_String : BTActionNode<UnityEngine.GameObject>
    {
        [Space]
        public Megumin.Binding.RefVar_String methodName;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.GameObject)MyAgent).BroadcastMessage(methodName);
            return Status.Succeeded;
        }
    }
}



