# Parent

## Sequence
序列节点  
节点按从左到右的顺序执行其子节点。当其中一个子节点失败时，序列节点也将停止执行。如果有子节点失败，那么序列就会失败。如果该序列的所有子节点运行都成功执行，则序列节点成功。  

非常不建议在Sequence的子节点使用LowerPriority低优先级终止，它是违反直觉的。  
在UE中Sequence的子节点禁用了低优先级终止，本插件还是保留可用性，防止真的有人用。

## Selector
选择节点  
节点按从左到右的顺序执行其子节点。当其中一个子节点执行成功时，选择器节点将停止执行。如果选择器的一个子节点成功运行，则选择器运行成功。如果选择器的所有子节点运行失败，则选择器运行失败。  

## Parallel
并行节点  
同时执行其所有子项（不是多线程）。根据FinishMode有不同的行为。  

| FinishMode          |                                                      |
| ------------------- | ---------------------------------------------------- |
| AnyFailed           | 任意一个子节点失败，返回失败。                       |
| AnySucceeded        | 任意一个子节点成功，返回成功。                       |
| AnyCompleted        | 任意一个子节点完成，返回完成节点的结果。             |
| AnySucceededWaitAll | 等待所有子节点都完成，任意一个子节点成功，返回成功。 |
| AnyFailedWaitAll    | 等待所有子节点都完成，任意一个子节点失败，返回失败。 |

## RandomOne
随机一个节点  
加权随机选出一个子节点执行，返回子节点的结果。  

## RandomSelector
随机选择节点
加权随机新的执行顺序，不支持低优先级终止。  

## RandomSequence
随机序列节点
加权随机新的执行顺序，不支持低优先级终止。  

## Empty
空节点  
没有任何效果，就像不存在一样，根据父节点返回不影响父节点的返回值。  

## Repeater
重复节点  
重复执行一定次数的子节点。-1表示无限重复。  

## StateChild0
子状态节点（抽象节点）  
根据特定状态执行Child0。看作一个小状态机。  

## Timeout
超时节点  
执行一定时间，如果子节点还没有完成，终止子节点，返回Failed。  

## Until
直到节点  
无限循环执行子节点，直到子节点满足设定的结果。  

## WaitDo
等待节点  
等待指定时间，然后执行子节点。  




---
---
# Action

## RandomFloat
随机浮点  
随机一个浮点数，并保存到SaveTo中。  

## RandomInt
随机整数  
随机一个整数，并保存到SaveTo中。  

## SendEvent
发出事件  
与 [CheckEvent_Decorator](./Decorator.md#Checkevent_decorator) 组合使用。  
发出一个事件，根据名字，触发对应的事件检查节点。  
事件的生命周期为一个tick。  
事件可以同时触发多个事件检查节点。  

## SetTrigger
设置触发器
与 [CheckTrigger_Decorator](./Decorator.md#CheckTrigger_Decorator) 组合使用。  
触发器生命周期为永久，但仅能被使用一次。  

## SubTree
子树节点
引用一个外部行为树，当作子节点执行。  
运行时右键EditorTree，即可进入子树debug功编辑器。  

## Wait
等待节点  
等待一定时间后，返回成功。  

>//不要在Wait节点增加返回值设置，默认都是返回成功。
//没有使用public CompletedResult Result = CompletedResult.Succeeded;
//也没有使用GetIgnoreResult(from);
//如果有的Wait返回成功，有的Wait返回失败，相同的节点返回不同返回值，
//在UI上无法表现出Wait节点的差异性，会给用户造成困惑，非常不容理解。

## Log
日志节点，打印日志到控制台
日志节点不可能满足所有的需求场景，所以建议用户实现自己的日志节点。  

---
---

## GameObject_SetTargetActive
游戏对象开关节点

## MeshRenderer_SetColor
设置MeshRenderer组件的材质球颜色。  

## MeshRenderer_ChangeColorMoment
临时改变MeshRenderer组件的材质球颜色，等待一定时间后改回原来的颜色。  

## NavMeshAgent_MoveTo
导航代理移动到目的地。  


---
---
# Gameplay

## FindDestination
找到一个目的地,并保存在Destination参数中。  

## MoveToVector3
移动到Vector3目的地。  

**需要GameObject含有实现`IMoveToable<Vector3>`接口的组件。**  

## Patrol_1
巡逻节点  
在设置的检查点列表`RefVar_Transform_List`中按顺序，循环巡逻。  
每到达一个检查点，执行一次子节点。  

**需要GameObject含有实现`IMoveToable<Vector3>`接口的组件。**  

## Patrol_2
巡逻节点  
以节点开始的位置为中心，一定半径内随机检查点巡逻。  
每到达一个检查点，执行一次子节点。  

**需要GameObject含有实现`IMoveToable<Vector3>`接口的组件。**  











