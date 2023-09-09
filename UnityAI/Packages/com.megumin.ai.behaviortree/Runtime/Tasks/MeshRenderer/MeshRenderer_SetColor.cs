using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_meshrenderer icon")]
    [DisplayName("SetTargetActive")]
    [Category("UnityEngine/MeshRenderer")]
    [AddComponentMenu("SetColor")]
    public sealed class MeshRenderer_SetColor : BTActionNode<MeshRenderer>
    {
        public RefVar_String ColorName = new RefVar_String() { value = "_BaseColor" };
        public RefVar_Color TargetColor = new RefVar_Color() { value = Color.white };

        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            foreach (var item in MyAgent.materials)
            {
                if (item.HasColor(ColorName))
                {
                    item.SetColor(ColorName, TargetColor);
                }
            }
        }
    }
}
