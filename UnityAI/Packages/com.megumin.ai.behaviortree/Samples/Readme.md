# 示例行为树说明

## WaitAndLog
开始节点是Sequence序列节点，使用Loop装饰器，次数-1表示无限循环。  
依次等待1，2，3，三个等待节点，然后执行Log节点。

## Selector
开始节点是Selector选择节点，使用Loop装饰器，次数-1表示无限循环。  
执行第一个Wait节点，因为Inverter装饰器反转了结果值，返回失败。
然后执行第二个Wait节点，返回成功。
Log节点不会被执行。

## MissNode
Missing节点演示了，作为Selector子节点返回失败，
作为Sequece子节点返回成功，就像这个节点不存在一样。

## AbortType
演示了Selector下子节点条件终止的自身终止和低优先级终止两种情况。  
运行时更改CheckBool装饰的值，观察执行效果。  

## AbortType2
演示了Sequence下子节点条件终止的自身终止和低优先级终止两种情况。  
运行时更改CheckBool装饰的值，观察执行效果。  

注意：非常不建议在Sequence下子节点使用低优先级终止，它违法直觉。

## Parallel
并列执行所有子节点。

## KeyCode
演示了行为树使用按键事件作为触发条件。  
当行为树执行到右侧节点时，按下Space键，触发条件终止。

## Event
演示了发送事件和接收事件。  
事件有一个自己的名字。  
右侧触发了一个自定义事件"MyEvent"，当事件触发时，左侧触发条件终止。  

## Trigger
演示了触发器的使用。  
触发器有一个自己的名字。  
右侧设置一个触发器，左侧两个节点检查触发器。  
与事件不同，触发器仅可以被使用一次。可以看到左侧两个检查触发器不会同时被触发。

## RootTree SubTree
演示了子树可以单独执行。
也可以作为其他树的一个子节点执行。  
在RootTree编辑器中，运行模式，右键Subtree节点选择EditorTree，即可打开Subtree的Debug窗口。  

## Random
演示随机条件节点。
在0-10之间随机一个整数，如果大于等于7，执行Wait。
每次随机得到的值，可以在CompareRandomInt SaveTo属性处查看。  

## RandomSelector
演示了随机选择器，根据设定的节点权重，随机执行。

## Goto
演示了Goto节点，可以将Goto节点的目标节点设置为5678节点。
当执行到Goto节点时，会跳转到目标节点。

