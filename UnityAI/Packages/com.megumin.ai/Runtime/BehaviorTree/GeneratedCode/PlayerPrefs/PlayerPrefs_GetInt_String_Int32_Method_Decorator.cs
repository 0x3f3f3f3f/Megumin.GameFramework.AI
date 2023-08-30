﻿///********************************************************************************************************************************
///The code on this page is generated by the code generator, do not manually modify.
///CodeGenerator: Megumin.CSCodeGenerator.  Version: 1.0.1
///CodeGenericBy: Megumin.AI.BehaviorTree.Editor.NodeGenerator
///CodeGenericSourceFilePath: Packages/com.megumin.ai/Editor/BehaviorTree/Generator/NodeGeneraotr.asset
///********************************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [DisplayName("PlayerPrefs_GetInt")]
    [Category("UnityEngine/PlayerPrefs")]
    [AddComponentMenu("GetInt(String, Int32)")]
    public sealed class PlayerPrefs_GetInt_String_Int32_Method_Decorator : CompareDecorator<int>
    {
        [Space]
        public Megumin.Binding.RefVar_String key;
        public Megumin.Binding.RefVar_Int defaultValue;

        [Space]
        public Megumin.Binding.RefVar_Int CompareTo;

        [Space]
        public Megumin.Binding.RefVar_Int SaveValueTo;

        public override int GetResult()
        {
            var result = UnityEngine.PlayerPrefs.GetInt(key, defaultValue);

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




