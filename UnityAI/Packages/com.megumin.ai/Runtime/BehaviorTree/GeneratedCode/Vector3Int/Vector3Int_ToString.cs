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
    [DisplayName("Vector3Int_ToString")]
    [Category("Unity/Vector3Int")]
    [AddComponentMenu("ToString")]
    public sealed class Vector3Int_ToString : BTActionNode
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3Int MyAgent;


        public Megumin.Binding.RefVar_String Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.Vector3Int)MyAgent).ToString();

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




