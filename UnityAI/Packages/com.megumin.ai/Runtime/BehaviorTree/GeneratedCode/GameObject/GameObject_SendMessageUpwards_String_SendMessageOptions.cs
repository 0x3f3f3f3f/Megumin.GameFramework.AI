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
    [Icon("GameObject Icon")]
    [DisplayName("GameObject_SendMessageUpwards")]
    [Category("Unity/GameObject")]
    [AddComponentMenu("SendMessageUpwards(String, SendMessageOptions)")]
    public sealed class GameObject_SendMessageUpwards_String_SendMessageOptions : BTActionNode<UnityEngine.GameObject>
    {
        [Space]
        public Megumin.Binding.RefVar_String methodName;
        public Megumin.Binding.RefVar<UnityEngine.SendMessageOptions> options;

        protected override Status OnTick(BTNode from, object options1 = null)
        {
            ((UnityEngine.GameObject)MyAgent).SendMessageUpwards(methodName, options);
            return Status.Succeeded;
        }
    }
}




