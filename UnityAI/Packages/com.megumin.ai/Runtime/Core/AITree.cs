using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Megumin.GameFramework.AI
{
    public class AITree
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [field: SerializeField]
        public string GUID { get; set; }

        [field: NonSerialized]
        public TraceListener TraceListener { get; set; } = new UnityTraceListener();
        public IRunOption RunOption { get; set; }

        /// <summary>
        /// 参数表中的一些值也在里面，没没有做过滤
        /// </summary>
        public HashSet<IBindingParseable> AllBindingParseable { get; } = new();

        public virtual void Log(object message)
        {
            if (RunOption?.Log == true)
            {
                TraceListener?.WriteLine(message);
            }
        }
    }

    public class UnityTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            Debug.Log(message);
        }

        public override void Write(object o, string category)
        {
            if (category.Contains("Warning", StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning(o);
            }
            else
            {
                base.Write(o, category);
            }
        }

        public override void Write(string message, string category)
        {
            if (category.Contains("Warning", StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning(message);
            }
            else
            {
                base.Write(message, category);
            }
        }

        public override void WriteLine(string message)
        {
            Debug.Log(message);
        }

        public override void WriteLine(object o, string category)
        {
            if (category.Contains("Warning", StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning(o);
            }
            else
            {
                base.WriteLine(o, category);
            }
        }

        public override void WriteLine(string message, string category)
        {
            if (category.Contains("Warning", StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning(message);
            }
            else
            {
                base.WriteLine(message, category);
            }
        }

        public override void Fail(string message, string detailMessage)
        {
            Debug.LogError($"{message}\n{detailMessage}");
        }

        public override void Fail(string message)
        {
            Debug.LogError(message);
        }
    }
}
