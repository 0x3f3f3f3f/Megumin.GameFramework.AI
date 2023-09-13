using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_meshrenderer icon")]
    [DisplayName("SetColor")]
    [Category("UnityEngine/MeshRenderer")]
    [AddComponentMenu("SetColor")]
    public sealed class MeshRenderer_SetColor : BTActionNode<MeshRenderer>
    {
        [Space]
        public bool ChangeInstanceMats = true;

        [Space]
        public RefVar_String ColorName = new RefVar_String() { value = "_BaseColor" };
        public RefVar_Color TargetColor = new RefVar_Color() { value = Color.white };

        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            Material[] materials = null;
            if (ChangeInstanceMats)
            {
                materials = MyAgent.materials;
            }
            else
            {
                materials = MyAgent.sharedMaterials;
            }

            foreach (var item in materials)
            {
                if (item.HasColor(ColorName))
                {
                    item.SetColor(ColorName, TargetColor);
                }
            }
        }
    }
}
