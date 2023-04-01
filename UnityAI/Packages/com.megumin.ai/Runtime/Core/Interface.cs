﻿using System;
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
    /// TODO,使用负数
    /// </summary>
    [Flags]
    public enum FailedCode
    {
        None,
        Abort = 1 << 0,
    }

    public interface IBuildContextualMenuable
    {
        void BuildContextualMenu(ContextualMenuPopulateEvent evt);
    }
}




