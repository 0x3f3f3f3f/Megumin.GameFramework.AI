using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Megumin.AI
{
    public class AITree
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [field: SerializeField]
        public string GUID { get; set; }

        [field: NonSerialized]
        public TraceListener TraceListener { get; set; }
        public RunOption RunOption { get; set; }

        /// <summary>
        /// 参数表中的一些值也在里面，没没有做过滤
        /// </summary>
        public HashSet<IBindingParseable> AllBindingParseable { get; } = new();
        public HashSet<IAwakeable> AllAwakeable { get; } = new();
        public HashSet<IStartable> AllStartable { get; } = new();
        public HashSet<IResetable> AllResetable { get; } = new();

        [Obsolete("use GetLogger instead", true)]
        [HideInCallstack]
        public virtual void Log(object message)
        {
            if (RunOption?.Log == true)
            {
                TraceListener?.WriteLine(message);
            }
        }

        public virtual TraceListener GetLogger(string key = null)
        {
            if (RunOption?.Log == true)
            {
                if (TraceListener == null)
                {
                    TraceListener = new UnityTraceListener();
                }
                return TraceListener;
            }

            return null;
        }

        protected readonly Unity.Profiling.ProfilerMarker awakeMarker = new("Awake");
        public virtual void Awake(object options = null)
        {
            using var profiler = awakeMarker.Auto();
            foreach (var item in AllAwakeable)
            {
                item?.Awake(options);
            }
        }

        protected readonly Unity.Profiling.ProfilerMarker startMarker = new("Start");
        public virtual void Start(object options = null)
        {
            using var profiler = startMarker.Auto();
            foreach (var item in AllStartable)
            {
                item?.Start(options);
            }
        }

        protected readonly Unity.Profiling.ProfilerMarker resetMarker = new("Reset");
        public virtual void Reset()
        {
            using var profiler = resetMarker.Auto();
            foreach (var item in AllResetable)
            {
                item?.Reset();
            }
        }
    }
}
