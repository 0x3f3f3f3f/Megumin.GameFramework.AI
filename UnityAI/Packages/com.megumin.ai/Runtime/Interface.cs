using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.AI
{
    public interface ITitleable
    {
        string Title { get; }
    }

    public interface IDetailable
    {
        string GetDetail();
    }

    public interface IDetailAlignable
    {
        TextAnchor DetailTextAlign { get; }
    }

    /// <summary>
    /// 可验证节点数据的
    /// </summary>
    public interface IDataValidable
    {
        /// <summary>
        /// 用于验证节点数据的合法性，当参数不合法时，编辑器UI上增加提示。
        /// <para/> 通常0表示数据合法
        /// </summary>
        /// <returns></returns>
        (int Result, string ToolTip) Valid();
    }

    /// <summary>
    /// 含有输入端口信息的。Info-y
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInputPortInfoy<out T>
    {
        /// <summary>
        /// 输入端口信息
        /// </summary>
        T InputPortInfo { get; }
    }

    /// <summary>
    /// 含有输出端口信息的。Info-y
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOutputPortInfoy<out T>
    {
        /// <summary>
        /// 输出端口信息
        /// </summary>
        T OutputPortInfo { get; }
    }

    [Flags]
    public enum Status
    {
        Init = 0,
        Succeeded = 1 << 0,
        Failed = 1 << 1,
        Running = 1 << 2,
        //Aborted = 1 << 3, 中断是失败的一种，不应该放入枚举。在类中额外加一个字段表示失败原因。后期复杂情况失败原因可能不只一个。
        //IsCompleted => State == Status.Succeeded || State == Status.Failed;
    }

    public enum CompletedResult
    {
        Succeeded = 1,
        Failed = 2,
    }

    /// <summary>
    /// 执行失败的错误码
    /// </summary>
    public enum FailedCode
    {
        None = 0,
        Abort = -1,
        Error = -2,
        Test0 = -3,
    }

    /// <summary>
    /// 按位枚举使用负数，不能按位取消，取消时会取消掉符号位。
    /// </summary>
    [Flags]
    public enum TestFailedCode2 : uint
    {
        None,
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2,
        D = 1 << 3,
        Test0 = 1 << 4,
        Test1 = 0x80000000,
        Test2 = 0xFFFFFFFF,
    }

    public interface IBuildContextualMenuable
    {
        void BuildContextualMenu(ContextualMenuPopulateEvent evt);
    }


    public interface ITreeElement
    {
        /// <summary>
        /// 节点唯一ID
        /// </summary>
        string GUID { get; }
    }

    public interface IAgentable
    {
        object Agent { get; set; }
    }

    /// <summary>
    /// 泛型代理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMyAgentable<T>
    {
        T MyAgent { get; set; }
    }

    public interface IHasMyAgent
    {
        /// <summary>
        /// 泛型代理是否有效
        /// </summary>
        /// <returns></returns>
        bool HasMyAgent();
    }

    /// <summary>
    /// 可以绑定代理的
    /// </summary>
    public interface IBindAgentable
    {
        /// <summary>
        /// 由于泛型Agent，必须在主线程调用，所以和ParseBinding分开。
        /// </summary>
        /// <param name="agent"></param>
        void BindAgent(object agent);
    }

    public interface ISubtreeTreeElement : ITreeElement, IBindAgentable
    {
        object TreeAsset { get; }
    }

    public interface IAIMeta
    {

    }

    public interface IAwakeable
    {
        /// <summary>
        /// 绑定之后，解析之后，第一次Tick开始时调用，不能保证所有节点的调用顺序
        /// </summary>
        /// <param name="options"></param>
        void Awake(object options = null);
    }

    public interface IStartable
    {
        /// <summary>
        /// 绑定之后，解析之后，第一次Tick开始时，Awake之后调用，不能保证所有节点的调用顺序
        /// </summary>
        /// <param name="options"></param>
        void Start(object options = null);
    }

    public interface IStopable
    {
        void Stop(object options = null);
    }

    public interface IResetable
    {
        void Reset(object options = null);
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

    /// <summary>
    /// 树调试器
    /// </summary>
    public interface ITreeDebugger
    {
        void SetDebugDirty();
    }

    public interface IViewBodyExpandable 
    {
        VisualElement GetBodyExpend();
    }

}




