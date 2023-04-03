using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI
{
    public interface IRunOption
    {
        bool Log { get; }
        int Order { get; }
        OperationTree OnSucceeded { get; }
        OperationTree OnFailed { get; }
    }

    [Serializable]
    public class RunOption : IRunOption
    {
        [field: SerializeField]
        public bool Log { get; set; }

        [field: SerializeField]
        public int Order { get; set; }

        [field: SerializeField]
        public OperationTree OnSucceeded { get; set; } = OperationTree.None;

        [field: SerializeField]
        public OperationTree OnFailed { get; set; } = OperationTree.None;
    }

    [Flags]
    public enum OperationTree
    {
        None = 0,
        ReSet = 1 << 0,
        ReStart = 1 << 1,
        Init = 1 << 2,
        Destory = 1 << 3,
        Pause = 1 << 4,
        Resume = 1 << 5,
        Enable = 1 << 6,
        Disable = 1 << 7,
        Start = 1 << 8,
        Stop = 1 << 9,
    }

    [Serializable]
    public class InitOption
    {
        public bool AsyncInit = true;
        public bool TaskInit = true;
        /// <summary>
        /// 运行时通常不会修改meta信息，也不会修改树结构。可以考虑共享meta。
        /// </summary>
        public bool SharedMeta = true;
        public bool LazyInitSubtree = false;
    }
}
