using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    [Icon("d_meshrenderer icon")]
    [DisplayName("ChangeColorMoment")]
    [Category("UnityEngine/MeshRenderer")]
    [AddComponentMenu("ChangeColorMoment")]
    [HelpURL(URL.WikiTask + "MeshRenderer_ChangeColorMoment")]
    public class MeshRenderer_ChangeColorMoment : WaitAction<MeshRenderer>
    {
        [Space]
        public bool ChangeInstanceMats = true;

        [Space]
        public RefVar_String ColorName = new RefVar_String() { value = "_BaseColor" };
        public RefVar_Color TargetColor = new RefVar_Color() { value = Color.white };

        Material[] materials = null;
        Color[] orignalColors;
        int ColorID = 0;

        protected override void OnEnter(BTNode from, object options = null)
        {
            base.OnEnter(from, options);
            ColorID = Shader.PropertyToID(ColorName);

            if (ChangeInstanceMats)
            {
                materials = MyAgent.materials;
            }
            else
            {
                materials = MyAgent.sharedMaterials;
            }

            orignalColors = new Color[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                var item = materials[i];
                if (item.HasColor(ColorID))
                {
                    orignalColors[i] = item.GetColor(ColorID);
                    item.SetColor(ColorID, TargetColor);
                }
            }
        }

        protected override void OnExit(BTNode from, Status result, object options = null)
        {
            RestoreColor();
        }

        protected override void OnAbort(BTNode from, object options = null)
        {
            RestoreColor();
        }

        /// <summary>
        /// 还原材质颜色
        /// </summary>
        protected void RestoreColor()
        {
            for (int i = 0; i < materials.Length; i++)
            {
                var item = materials[i];
                if (item.HasColor(ColorID))
                {
                    item.SetColor(ColorID, orignalColors[i]);
                }
            }
        }
    }
}
