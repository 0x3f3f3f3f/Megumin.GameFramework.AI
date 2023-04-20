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
    [DisplayName("Vector3Int_Equals")]
    [Category("Unity/Vector3Int")]
    [AddComponentMenu("Equals(Vector3Int)")]
    public sealed class Vector3Int_Equals_Vector3Int : ConditionDecorator
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3Int MyAgent;

        [Space]
        public Megumin.Binding.RefVar_Vector3Int other;

        public Megumin.Binding.RefVar_Bool Result;

        public override bool CheckCondition(object options = null)
        {
            var result = ((UnityEngine.Vector3Int)MyAgent).Equals(other);

            if (Result != null)
            {
                Result.Value = result;
            }

            return result;
        }
    }
}




