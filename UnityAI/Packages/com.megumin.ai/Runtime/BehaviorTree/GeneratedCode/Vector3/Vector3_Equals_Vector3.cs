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
    [DisplayName("Vector3_Equals")]
    [Category("Unity/Vector3")]
    [AddComponentMenu("Equals(Vector3)")]
    public sealed class Vector3_Equals_Vector3 : ConditionDecorator
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3 MyAgent;

        [Space]
        public Megumin.Binding.RefVar_Vector3 other;

        [Space]
        public Megumin.Binding.RefVar_Bool SaveValueTo;

        public override bool CheckCondition(object options = null)
        {
            var result = ((UnityEngine.Vector3)MyAgent).Equals(other);

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return result;
        }
    }
}



