using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.AI.BehaviorTree
{
    public partial class BehaviorTree
    {
        //Realtime Data
        public bool IsRunning { get; internal set; }

        private Status treestate = Status.Init;

        public string InstanceName { get; set; } = "anonymity";

        public int CompletedCount { get; protected set; } = 0;
        public int SucceededCount { get; protected set; } = 0;
        public int FailedCount { get; protected set; } = 0;

        /// <summary>
        /// 可以用于节点区分是不是当前tick。
        /// </summary>
        public int TotalTickCount { get; protected set; } = 0;
        /// <summary>
        /// 是不是第一次Tick
        /// </summary>
        public bool IsFirstTick { get; protected set; } = false;

        /// <summary>
        /// 当前正在Tick的节点序号
        /// </summary>
        public int LastTickNodeIndex { get; set; } = -1;
        public BTNode LastTick { get; set; } = null;
    }
}

