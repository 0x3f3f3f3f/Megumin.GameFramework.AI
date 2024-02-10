# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!--
## [Unreleased] - YYYY-MM-NN

### Added  
### Changed  
### Deprecated  
### Removed  
### Fixed  
### Security  
-->

---

## [Unreleased] - YYYY-MM-NN

## [1.3.0] - 2024-02-10
### Added  
- 节点增加异步执行模式
- 增加Goto节点
- 允许间接引用其他节点
  
### Changed  
- BTNode.OnTick虚方法默认返回值由Succeeded改为Running。
- 重构Event Trigger，现在可以携带更多参数。支持泛型事件。
- 破坏性API更改： OnEnter OnExit增加BTNode from参数
- 重构  GetIgnoreResult(BTNode from)。去除对Selector节点的引用
- 重构移动等节点，现在允许智能获取对象碰撞盒半径。

### Fixed  
- 修改TypeCache测试
- 修复类型适配器引起的空引用

## [1.2.3] - 2024-01-08

### Added  
- 增加TypeCache优化测试
- 统计打印各类型节点数量
- 创建代码模板
- 增加状态机标准接口
- 增加OnEnter2虚方法，方便修改执行状态
- 增加body扩展
- 支持MoveFromAttribute

## [1.2.2] - 2023-10-29
### Added   
- 增加一键从项目中删除插件菜单
- 增加一键重新保存所有行为树文件菜单
- 增加安卓测试
- 抽象初始化方法
- 增加预热测试
- 增加关闭所有编辑器窗口菜单
- 增加关闭此外编辑器窗口菜单
- 增加Properties菜单。整理上下文菜单

### Changed  
- 重构性能采样标记

### Fixed  
- 修复流程图map关键字 导致不能渲染bug
- 修改优化文档
- 优化预热代码
- 优化Debug性能。debug调用放入EditorLoop中
- 优化Tick中的Linq Any 

## [1.2.1] - 2023-10-08
### Added   
- 增加代码生成器创建菜单  
- 增加MyRefVar示例  
- 增加初始化事件回调
- 增加延迟加入Manager增加最小值设置
- 在BindAgent和ParseBinding前增加延迟设置
- 增加预热API
- 优化Linq FirstOrDefault

### Fixed  
- 修复WebGL异步初始化bug

## [1.2.0] - 2023-09-29
### Added   
- TypeCache 增加API：分离命名空间。  
- 节点增加日志开关。  
- Inspector面板增加脚本资源显示。  
- RefVar增加索引器绑定。  
- RefVar增加泛型SetValue方法,用于空传播时赋值。  
- RefVar增加值类型到string类型适配。  
- TypeAdpter增加unity常见类型适配器。  
- TypeCache增加清除缓存方法。  
- TypeCache增加缓存别名菜单。  
- 动态修改行为树。  
- 增加Awake Start调用点。  
- 增加[DebuggerStepThrough]，调试时跳过简单属性。  
- 绑定失败时节点增加提示。  
- 增加StateChild0节点。  
- 别名没有命名空间时，增加一个警告。  
- 增加IDataValidable接口，在节点数据不合法时，增加UI提示。  
- 增加输入输出端口信息接口。节点可以自定义显示端口名字。  
* 增加Gameplay节点：MoveTo。  
* 增加Gameplay节点：FindDestination。  
* 增加Gameplay节点：Patrol。  
* 增加Gameplay节点：Follow。  
+ 增加Gameplay装饰器：IsArrive。  
+ 增加Gameplay装饰器：CanSeeTarget。  


### Changed  
- 拆分为AI基础包和行为树两个包。  
- 重构节点代码生成器。重新生成节点。  
- 重构VerifyMyAgent字段。  
- 重新设计Log机制，避免生成无用字符串。  
- 循环节点可以在子树中关闭。   
- ParseBindingResult 重命名为 CreateDelegateResult，并移动到Megumin.Reflection。  
- 获取序列化成员增加ignoreDefaultValue参数。  
- Enableable 重命名为  Enable。  
- 拆分接口，条件装饰器接口不在继承IAbortable接口，不是所有的条件装饰都支持abort，例如随机条件装饰器。  
- 重构Log装饰器，加入宏机制。  
- 装饰器类型使用 _Decorator 后缀。
- 节点终止前增加状态检测。  

### Fixed  
- 修复命名空间改变时反序列化问题。  
- 修复类名改变时反序列化问题。  
- 修复Color类型不能正确序列化错误。  
- 优化编辑器打开速度。  
- OnTick前增加State判断，确保为Running状态。  
- 基元类型和string不要反射查找成员。  
- 修复节点顺序不更新bug。  
- 修复unity2023中AbortType图标被遮挡bug。  


## [1.1.0] - 2023-08-11
### Changed  
- 标记过时API


## [1.0.1] - 2023-08-07
### Added 
- 增加文档。

## [0.0.1] - 2023-08-30
PackageWizard Fast Created.

