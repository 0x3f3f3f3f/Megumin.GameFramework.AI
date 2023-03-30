using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.GameFramework.AI.Editor;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    internal static class RefreshExtension
    {
        public static void RefreshAbortTypeBadge(this VisualElement badge, object abortable)
        {
            if (badge == null)
            {
                return;
            }

            var hasSelf = (abortable as IAbortable)?.AbortType.HasFlag(AbortType.Self) ?? false;
            badge.SetToClassList(UssClassConst.abortTypeSelf, hasSelf);
            var hasLow = (abortable as IAbortable)?.AbortType.HasFlag(AbortType.LowerPriority) ?? false;
            badge.SetToClassList(UssClassConst.abortTypeLowerPriority, hasLow);
            badge.tooltip = null;
            if (hasSelf && hasLow)
            {
                badge.tooltip = "AbortType.Both";
            }
            else if (hasSelf)
            {
                badge.tooltip = "AbortType.Self";
            }
            else if (hasLow)
            {
                badge.tooltip = "AbortType.LowerPriority";
            }
        }
    }
}
