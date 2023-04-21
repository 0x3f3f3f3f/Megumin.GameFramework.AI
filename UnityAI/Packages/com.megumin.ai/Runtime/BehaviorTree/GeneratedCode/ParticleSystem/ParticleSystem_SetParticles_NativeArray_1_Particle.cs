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
    [DisplayName("ParticleSystem_SetParticles")]
    [Category("Unity/ParticleSystem")]
    [AddComponentMenu("SetParticles(NativeArray`1)")]
    public sealed class ParticleSystem_SetParticles_NativeArray_1_Particle : BTActionNode<UnityEngine.ParticleSystem>
    {
        [Space]
        public Megumin.Binding.RefVar<Unity.Collections.NativeArray<UnityEngine.ParticleSystem.Particle>> particles;

        protected override Status OnTick(BTNode from, object options = null)
        {
            ((UnityEngine.ParticleSystem)MyAgent).SetParticles(particles);
            return Status.Succeeded;
        }
    }
}



