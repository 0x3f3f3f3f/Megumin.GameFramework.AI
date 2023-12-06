using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.AI.StateMachine
{
    public class StateMachine
    {

    }

    //状态机 历史执行状态队列代替 from
    //虚拟状态 Enter Exit Any
    //过渡是过渡
    //条件是条件
    //一个条件检测 可以分配给多个过渡上用，仅检测计算一次
    //一个过渡可以附加多个条件，并允许逻辑运算
    public interface IState
    {
        int Enter(IState from);
        int Tick(object option = null);
        int Exit(IState to);
    }

    public interface IVisualState
    {

    }

    public interface ITransition
    {

    }

    public interface IConditon
    {

    }

    public interface IStateMachine
    { 
        IVisualState Any { get; }
        IVisualState Enter {  get; }
        IVisualState Exit { get; }

        
    }

}
