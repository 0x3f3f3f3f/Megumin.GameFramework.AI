|          | FSM  | HFSM | Animator | State Tree | Behavior Tree |
| -------- | ---- | ---- | -------- | ---------- | ------------- |
| 并行     | ❌    | ❌    | ✅        |            | ✅             |
| 跨层过渡 | ⛔🚫❎  | ✅    | ✅        | ✅          | ❌             |
|          |      |      |          |            |               |

Animator 没有丝毫折扣的实现了HFSM的所有功能。

**注意Animator的Create Sub-State Machine才是HFSM的主要特征，和Layers功能无关，不要被“层”这个名字误导。Animator的Layers功能更主要的好使要实现并行状态需求。**

Animator 编辑器进入SubMachine时上层就不见了，无法看到整个HFSM的全貌。

Animator 是状态机，是HFSM的一个实现。但是 不是所有的状态机，HFSM都是Animator 。

Animator 是一个状态机的超集。

不能将状态机概念和Animator混淆。Animator是一个庞然大物，Animator包含实现的feature太多了，不是状态机一个词可能概况的。





Animator 的AnyStateNode实现了从任意状态切换到任意状态，实现了Abort。



虽然Animator很厉害，当并不等于好用。作为一个Component来说，杂糅了太多功能，并且耦合严重，不符合单一职责原则。

如果能将Animator拆分为状态机和动画播放2个部分会更好（似乎会回到Animation时代😂）。



Animator不好用不是在状态机设计上有缺陷，而是在参数绑定和对Owner调用上有缺陷。

Owner要驱动状态机，必须用Animator.SetValue。

状态机要调用Owner方法，要使用StateMachineBehaviour，内部还要animator.GetComponent才能拿到Agent。

为什么状态机不能直接读取Owner成员值呢？为什么Animator不能直接调用Owner方法呢？

我是知道原因的，但是就结论来说，现在的Animator不好用，一万个原因也站不住脚。





跨层调度弊端：    
行为树增加删除一个节点时，副作用很小，能过渡到这个节点的只有它的父节点。  
分层状态机增加删除一个节点，不知道有多少未知过渡指向这个节点。任何节点都可以个过渡到任何节点，对维护来说是很可怕的。



# HFSM

一个明显特征是状态机嵌套另一个状态机，其他的功能点并没有明确定义。



- 状态可以过渡到子状态机，但并不指定子状态机内的哪一个状态，由子状态机自己决定。这是一种Selector机制。

> [Unity - Manual: Sub-State Machines (unity3d.com)](https://docs.unity3d.com/2023.1/Documentation/Manual/NestedStateMachines.html)As noted above, a sub-state machine is just a way of visually collapsing a group of states in the editor, so when you make a transition to a sub-state machine, you have to choose which of its states you want to connect to.  
>
>  ![MecanimSelectSubState.png (504×120) (unity3d.com)](https://docs.unity3d.com/2023.1/Documentation/uploads/Main/MecanimSelectSubState.png)

实测现在已经没有这个限制了，只是文档没改。其实本质上也不应该存在这个限制。



子状态机在上一次可以并必须看成一个状态。既然是一个状态，那么必须允许这个状态到其他状态的过渡。无论子状态内部如何，子状态机本身到其他状态的过渡条件都是要检测的，很多HFSM都没有实现这一点。



# 状态机的特征





状态机是没有Selector机制的。在状态机运行的任意时刻，状态机必须处于某种状态。   
当我们需要从某个状态退出时，必须要有明确过渡指向另一个state。  
Q：当我们必须退出，又不知道退出后去哪个状态时怎么办？

A：这就是Selector机制。状态机可以根据条件，从所有状态节点中选择出下一个节点。

Animator 的ExitNode实现无责任退出。

Animator 的EnterNode实现了Selector。

# 行为树的特征

一个Seq下又两个Task，Sleep，Say。 逻辑是，睡醒了说句话。可以由Sleep过渡到Say，但是在Say后无法在切换换回Sleep，除非修改上层或者上上层节点。

因为行为树在同一个层之间执行时，总是按顺序的，是不能回头的。状态机不然，同一个层次执行过渡是任意的。







---

---



从结构上来说：[数据结构树和图之间有什么区别？ (qastack.cn)](https://qastack.cn/programming/7423401/whats-the-difference-between-the-data-structure-tree-and-graph) 

StateTree将图结构改成树结构。

StateTree 是一种HFSM的受限形式。





从状态切换过渡性来说：

行为树是一种StateTree的受限形式。









# 参考

- [有限状态机 - 维基百科，自由的百科全书 (wikipedia.org)](https://zh.wikipedia.org/wiki/有限状态机)
- [Inspiaaa/UnityHFSM: A simple yet powerful class based hierarchical finite state machine for Unity3D (github.com)](https://github.com/Inspiaaa/UnityHFSM)
- [学习笔记2.5-----分层有限状态机 - 知乎 (zhihu.com)](https://zhuanlan.zhihu.com/p/558422986)
- [Unity - Manual: State Machine Basics (unity3d.com)](https://docs.unity3d.com/2023.1/Documentation/Manual/StateMachineBasics.html)

































