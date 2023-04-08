using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI
{
    public interface ITitleable
    {
        string Title { get; }
    }

    public interface IDetailable
    {
        string GetDetail();
    }


    [Flags]
    public enum Status
    {
        Init = 0,
        Succeeded = 1 << 0,
        Failed = 1 << 1,
        Running = 1 << 2,
        //Aborted = 1 << 3, 中断是失败的一种，不应该放入枚举。在类中额外加一个字段表示失败原因。后期复杂情况失败原因可能不只一个。
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
}




