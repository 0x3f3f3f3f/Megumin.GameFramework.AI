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
    [Icon("Transform Icon")]
    [DisplayName("Transform_Rotate")]
    [Category("Unity/Transform")]
    [AddComponentMenu("Rotate(Vector3)")]
    public sealed class Transform_Rotate_Vector3 : BTActionNode<UnityEngine.Transform>
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 eulers;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.Transform)MyAgent).Rotate(eulers);
            return Status.Succeeded;
        }
    }
}



