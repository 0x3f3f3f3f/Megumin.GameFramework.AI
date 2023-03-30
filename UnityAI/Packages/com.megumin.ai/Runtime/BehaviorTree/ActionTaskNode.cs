using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 装饰器
    /// </summary>
    public interface IDecorator
    {

    }

    /// <summary>
    /// 可终止的
    /// </summary>
    public interface IAbortable
    {
        AbortType AbortType { get; }
    }

    public interface IConditionDecorator : IDecorator, IAbortable
    {
        bool CheckCondition(BTNode container);
        bool LastCheckResult { get; }
    }

    internal interface IPreDecorator : IDecorator
    {
        /// <summary>
        /// 在Node Enter 之前被调用。
        /// </summary>
        /// <param name="container"></param>
        void BeforeNodeEnter(BTNode container);
    }

    /// <summary>
    /// 在装饰器中使用OnNodeTick,性能损失太高，没有必要，可以使用OneChild代替，功能上是一致的。
    /// </summary>
    [Obsolete("Use OneChild instead.", true)]
    public interface IPreTickDecorator : IDecorator
    {
        void OnPreNodeTick(BTNode container);
    }

    internal interface IPostDecorator : IDecorator
    {
        /// <summary>
        /// 在 Node Exit 之后被调用。只有这样才能实现Loop装饰器。
        /// </summary>
        /// <param name="result"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        Status AfterNodeExit(Status result, BTNode container);
    }

    internal interface IAbortDecorator : IDecorator
    {
        void OnNodeAbort(BTNode container);
    }


    public interface IStartable
    {
        bool IsStarted { get; }
        void Start();
        void Stop();
    }

    public interface IEnableable
    {
        bool Enabled { get; set; }

        //void Enable();
        //void Disable();
    }

    /// <summary>
    /// 想要轮询必须支持开启和关闭。这样才能正确处理Start。
    /// </summary>
    public interface ITickable : IEnableable
    {

    }

    [Flags]
    public enum AbortType
    {
        None = 0,
        Self = 1 << 0,
        LowerPriority = 1 << 1,
        Both = Self | LowerPriority
    }

    public enum EnterType
    {
        True,
        False,
        /// <summary>
        /// 有些节点可能调试时临时关闭，需要忽略这些节点。
        /// Enabled可以代替，但是感觉以后会用到，暂时保留。
        /// </summary>
        Ignore,
    }

    /// <summary>
    /// 可层层桥套的计时器，更改父计时器，子计时器也受到影响。
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// 重设Scale时，同时重设原点
        /// </summary>
        float Scale { get; }
        float Now { get; }
        float NoScaleNow { get; }
        DateTimeOffset realTimeNow { get; }
        DateTimeOffset Origin { get; }
        /// <summary>
        /// 由父类调用
        /// </summary>
        /// <param name="delta"></param>
        void MoveNext(float delta);
    }

    public class Timer
    {
        public Timer parent;
        double Origin = 0;
        double realtime = Time.realtimeSinceStartupAsDouble;
        double Now => (realtime - Origin) * GlobalScale;
        double localScale;
        double GlobalScale => localScale * (parent?.GlobalScale ?? 1);
    }


}
