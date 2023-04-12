using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using System.ComponentModel;

namespace Megumin.GameFramework.AI
{

    [Serializable]
    public class TreeElement<T> : ITreeElement
        where T : AITree
    {
        /// <summary>
        /// 节点唯一ID
        /// </summary>
        [field: SerializeField]
        public string GUID { get; set; }
        public string ShortGUID => GUID?[..13];

        [field: NonSerialized]
        public T Tree { get; set; }

        [HideInCallstack]
        public virtual void Log(object message)
        {
            Tree?.Log(message);
        }

        string tipString = null;
        public string TipString
        {
            get
            {
                if (tipString == null)
                {
                    tipString = $"[{ShortGUID}] {GetType().Name}";
                }
                return tipString;
            }
        }

        public override string ToString()
        {
            return TipString;
        }
    }

    public static class AICoreExtension_0B55B819EE6046679D61EBD313277135
    {
        public static bool TryGetToolTipString(this ITreeElement treeElement, out string tooltip)
        {
            if (treeElement != null)
            {
                var type = treeElement.GetType();
                StringBuilder stringBuilder = new StringBuilder();
                tooltip = null;
                foreach (var item in type.GetCustomAttributes(false))
                {
                    if (item is TooltipAttribute tooltipAttribute)
                    {
                        if (!string.IsNullOrEmpty(tooltipAttribute.tooltip))
                        {
                            stringBuilder.AppendLine(tooltipAttribute.tooltip);
                        }
                    }

                    if (item is DescriptionAttribute descriptionAttribute)
                    {
                        if (!string.IsNullOrEmpty(descriptionAttribute.Description))
                        {
                            stringBuilder.AppendLine(descriptionAttribute.Description);
                        }
                    }
                }

                if (stringBuilder.Length >= 2)
                {
                    //删除最后一行的换行符
                    tooltip = stringBuilder.ToString(0, stringBuilder.Length - 2);
                }

                return !string.IsNullOrEmpty(tooltip);
            }

            tooltip = null;
            return false;
        }
    }
}
