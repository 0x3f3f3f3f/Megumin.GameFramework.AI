# BehaviorTree
https://robohub.org/introduction-to-behavior-trees/

## 分层有限状态机（HFSM）
- EntryNode
- ExitNode
- AnyStateNode
- UpStateMachine
  
Animator 指定子层后，如果不指定子状态，会走默认状态。
Enter 对每个状态设置过渡，每个状态对Exit设置过渡，完全可以模拟 Selector。
通过EntryNode和ExitNode, 完全可以模拟 Selector，Sequence。
HFSM 允许从一个子状态机的子状态，过渡到另一个子状态机的子状态。  
行为树则不行，不能从一个分支直接过渡到不同层的分支节点。
HFSM的表达能力比行为树更强。

行为树可以并行2个节点，状态机则不行。状态机通过layer来解决这个问题。

anystatenode提供了更强大的过渡能力。

## 驱动方式
- 事件驱动(event-driven)
- 轮询驱动(tick-driven)
    - root-leaf
    - last leaf 

Q：为什么不采用事件驱动(event-driven)行为树？  
A：实现起来太过于复杂，涉及到参数绑定值变化时没有办法处理。    
事件驱动实现基础时存在Blackboard，并且Blackboard SetValue触发更新。  
本库不存在Blackboard，绑定后没有SetValue，类似与Laze模式，每次取值时才计算值，无法触发事件，优点是不用手动SetValue。  
事件驱动优点是性能更高，缺点是因为没有tick所以需要service节点。  
轮询驱动优点是实现简单。  

- 从Start节点一路Tick到末端，目前才用的方式  
  优点是实现简单，容易理解
- 记录最后执行的末端节点，每次Tick最后的节点  
  优点是性能更好一点


## 支持列表
- [ ] 使用异步API，比如父节点调用子节点等待结果，使用异步代替事件。Running可以使用异步代替。增加一个开关，异步模式还是Running模式。  
  实际实现时发现，使用异步实现起来更加繁琐。开销更大。所以暂时不采用这种方式。



