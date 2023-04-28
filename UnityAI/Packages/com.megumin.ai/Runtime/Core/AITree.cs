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
        public RunOption RunOption { get; set; }

        /// <summary>
        /// 参数表中的一些值也在里面，没没有做过滤
        /// </summary>
        public HashSet<IBindingParseable> AllBindingParseable { get; } = new();

        [HideInCallstack]
        public virtual void Log(object message)
        {
            if (RunOption?.Log == true)
            {
                TraceListener?.WriteLine(message);
            }
        }
    }
}
