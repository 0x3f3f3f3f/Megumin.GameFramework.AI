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
    [DisplayName("Quaternion_ToString")]
    [Category("Unity/Quaternion")]
    [AddComponentMenu("ToString")]
    public sealed class Quaternion_ToString : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar<UnityEngine.Quaternion> MyAgent;


        public Megumin.Binding.RefVar_String Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Quaternion)MyAgent).ToString();

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




