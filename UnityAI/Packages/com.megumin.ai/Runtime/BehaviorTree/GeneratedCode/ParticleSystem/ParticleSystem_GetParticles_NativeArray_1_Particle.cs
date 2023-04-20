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
    [Icon("ParticleSystem Icon")]
    [DisplayName("ParticleSystem_GetParticles")]
    [Category("Unity/ParticleSystem")]
    [AddComponentMenu("GetParticles(NativeArray`1)")]
    public sealed class ParticleSystem_GetParticles_NativeArray_1_Particle : BTActionNode<UnityEngine.ParticleSystem>
    {
        [Space]
        public Megumin.Binding.RefVar<Unity.Collections.NativeArray<UnityEngine.ParticleSystem.Particle>> particles;

        public Megumin.Binding.RefVar_Int Result;

        protected override Status OnTick(BTNode from, object options = null)
        {
            var result = ((UnityEngine.ParticleSystem)MyAgent).GetParticles(particles);

            if (Result != null)
            {
                Result.Value = result;
            }

            return Status.Succeeded;
        }
    }
}




