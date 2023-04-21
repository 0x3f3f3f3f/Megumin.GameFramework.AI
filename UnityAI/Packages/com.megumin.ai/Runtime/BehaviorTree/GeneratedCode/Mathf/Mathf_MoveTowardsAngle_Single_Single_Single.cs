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
    [Icon("d_collabcreate icon")]
    [DisplayName("Mathf_MoveTowardsAngle")]
    [Category("Unity/Mathf")]
    [AddComponentMenu("MoveTowardsAngle(Single, Single, Single)")]
    public sealed class Mathf_MoveTowardsAngle_Single_Single_Single : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Float current;
        public Megumin.Binding.RefVar_Float target;
        public Megumin.Binding.RefVar_Float maxDelta;

        [Space]
        public Megumin.Binding.RefVar_Float SaveValueTo;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = UnityEngine.Mathf.MoveTowardsAngle(current, target, maxDelta);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return Status.Succeeded;
        }
    }
}



