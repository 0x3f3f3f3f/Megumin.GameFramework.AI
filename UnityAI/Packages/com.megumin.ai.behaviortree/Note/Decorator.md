# ConditionDecorator
条件装饰

## KeyCodeEvent_Decorator
检查按键  

## MouseEvent_Decorator
检查鼠标事件

## Lock_Decorator
锁装饰器
在节点执行时上锁，相同名字的锁同时只能有一个节点执行。  
只与锁的名字有关，与节点类型无关。  

## CheckBool_Decorator
检查设置的bool参数

## CheckGameObject_Decorator
检查设置的GameObject是否满足条件。  

## CheckLayer_Decorator
检查设置的GameObject的Layer。  

## CheckEvent_Decorator
检查自定义事件  
与[SendEvent](./Task.md#SendEvent)配合使用。  

## CheckTrigger_Decorator
检查自定义触发器
与[SendEvent](./Task.md#SendEvent)配合使用。  

## CheckTimeout_Decorator
检查超时  
从执行节点开始计时，超时后终止节点，返回失败。

>通过AbortType.Self实现，所以不要更改AbortType设置。
源码位置 BTNode_Tick.cs 262行。  

---

## CompareBool_Decorator
比较设置的两个bool参数。  

## CompareFloat_Decorator
比较设置的两个float参数。  

## CompareInt_Decorator
比较设置的两个int参数。  

## CompareString_Decorator
比较设置的两个string参数。  

## CompareRandomFloat_Decorator
随机一个float值，保存到SaveTo，并与设置的值比较。  

## CompareRandomInt_Decorator
随机一个int值，保存到SaveTo，并与设置的值比较。  

## EqualsString_Decorator
比较设置的两个string参数是否相等，可以设置忽略大小写等条件。  





---
---

## RandomFloat_Decorator
在设置的DecoratorPosition触发时，随机一个float值，保存到SaveTo。  

## RandomInt_Decorator
在设置的DecoratorPosition触发时，随机一个int值，保存到SaveTo。  

## Cooldown_Decorator
冷却装饰器  
进入或完成节点时进入冷却，冷却完成前条件装饰器返回false。  

## Inverter_Decorator
反转结果装饰器  

## Log_Decorator
日志装饰器  

## Loop_Decorator
循环装饰器  
循环执行节点，-1表示无限循环。  

## LoopUntil_Decorator
无限循环节点，直到满足设置的结果。  

## Missing_Decorator
用于代替反序列化失败的装饰器，不含有任何功能。  

## Remap_Decorator
改变节点的结果。强制成功，强制失败，结果取反。  
比Inverter_Decorator更灵活。  





---
---

# Gameplay

## CanSeeTarget_Transform_Decorator
感知组件能否看见目标Transform

**需要GameObject含有`TransformPerception`组件。**  

## CanSeeTarget_GameObject_Decorator
感知组件能否看见目标GameObject

**需要GameObject含有`GameObjectPerception`组件。**  

## PerceptionHasTarget_Transform_Decorator
判断感知组件是否有目标，并保存到SaveTo。  

**需要GameObject含有`TransformPerception`组件。**  

## PerceptionHasTarget_GameObject_Decorator
判断感知组件是否有目标，并保存到SaveTo。  

**需要GameObject含有`GameObjectPerception`组件。**  


## TryFindDestination_Decorator
尝试从DestinationList找到一个检查点，并存入Destination。  

## IsArrive_Decorator
检查Transform是否到达指定地点。  
