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
    [DisplayName("Vector3Int_y")]
    [Category("UnityEngine/Vector3Int")]
    [AddComponentMenu("y")]
    public sealed class Vector3Int_y_Decorator : CompareDecorator<int>
    {
        [Space]
        public Megumin.Binding.RefVar_Vector3Int MyAgent;

        [Space]
        public Megumin.Binding.RefVar_Int CompareTo;

        [Space]
        public Megumin.Binding.RefVar_Int SaveValueTo;

        public override int GetResult()
        {
            var result = ((UnityEngine.Vector3Int)MyAgent).y;

            if (SaveValueTo != null)
            {
                SaveValueTo.Value = result;
            }

            return result;
        }

        public override int GetCompareTo()
        {
            return CompareTo;
        }

    }
}



