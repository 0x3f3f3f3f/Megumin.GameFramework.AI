﻿///********************************************************************************************************************************
///本页代码由代码生成器生成，请勿手动修改。The code on this page is generated by the code generator, do not manually modify.
///生成器类型：Megumin.GameFramework.AI.BehaviorTree.Editor.NodeGenerator
///配置源文件：$(CodeGenericSourceFilePath)
///********************************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Icon("d_CircleCollider2D Icon")]
    [DisplayName("Set_CircleCollider2D_radius")]
    [Category("UnityEngine/CircleCollider2D")]
    [AddComponentMenu("Set_radius")]
    public sealed class CircleCollider2D_radius_Set_Node : BTActionNode<UnityEngine.CircleCollider2D>
    {
        [Space]
        public Megumin.Binding.RefVar_Float Value;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.CircleCollider2D)MyAgent).radius = Value;
            return Status.Succeeded;
        }

    }
}



