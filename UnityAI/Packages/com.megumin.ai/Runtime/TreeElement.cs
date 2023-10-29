using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;

namespace Megumin.AI
{

    [Serializable]
    public class TreeElement<T> : ITreeElement, ITraceable
        where T : AITree
    {
        /// <summary>
        /// 节点唯一ID
        /// </summary>
        [field: SerializeField]
        public string GUID { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
        public string ShortGUID
        {
            [DebuggerStepThrough]
            get
            {
                return GUID?[..13];
            }
        }

        [field: NonSerialized]
        public T Tree { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        [field: NonSerialized]
        public TraceListener TraceListener { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = null;

        [Obsolete("use GetLogger instead", true)]
        [HideInCallstack]
        public virtual void Log(object message)
        {
            if (TraceListener == null)
            {
                Tree?.Log(message);
            }
            else
            {
                TraceListener.WriteLine(message);
            }
        }

        /// <summary>
        /// 使用空传播 <![CDATA[GetLogger()?.WriteLine($"Count: {completeCount} / {loopCount}");]]> 
        /// <para/> 当Logger返回null时，不会调用WriteLine，可以在关闭日志时，不生成log字符串拼接。
        /// <para/> 这样设计等价于先判断 CanLog，然后再生成日志。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TraceListener GetLogger(string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                return TraceListener ?? Tree?.GetLogger(key);
            }
            else
            {
                return Tree?.GetLogger(key);
            }
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
