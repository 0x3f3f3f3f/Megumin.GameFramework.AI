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
        /// <summary>
        /// 使用多线程异步实例化，防止阻塞主线程。
        /// 缺点是不会在当前帧立刻完成并执行行为树。
        /// </summary>
        public bool AsyncTaskInit = true;
        /// <summary>
        /// 使用多线程绑定，解析binding对象，防止阻塞主线程。
        /// 缺点是不会在当前帧立刻完成并执行行为树。
        /// </summary>
        public bool AsyncBindAgent = true;
        /// <summary>
        /// 运行时通常不会修改meta信息，也不会修改树结构。可以考虑共享meta。
        /// </summary>
        public bool SharedMeta = true;
        /// <summary>
        /// 延迟实例化子树，推迟到子树节点运行时实例化。
        /// </summary>
        public bool LazyInitSubtree = false;
    }
}
