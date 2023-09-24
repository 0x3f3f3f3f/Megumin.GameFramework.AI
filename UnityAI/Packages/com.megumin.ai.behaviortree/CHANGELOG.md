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

## [1.2.0] - 2023-09-24
### Added   
- TypeCache 增加API：分离命名空间。  
- 节点增加日志开关。  
- Inspector面板增加脚本资源显示。  
- RefVar增加索引器绑定。  
- RefVar增加泛型SetValue方法,用于空传播时赋值。  
- RefVar增加值类型到string类型适配。  
- TypeCache增加清除缓存方法。  
- TypeCache增加缓存别名菜单。  
- 动态修改行为树。  
- 增加[DebuggerStepThrough]，调试时跳过简单属性。  
- 绑定失败时节点增加提示。  
- 增加StateChild0节点。  
- 别名没有命名空间时，增加一个警告。  
- 增加IDataValidable接口，在节点数据不合法时，增加UI提示
* 增加Gameplay节点：MoveTo。  
* 增加Gameplay节点：FindDestination。  
* 增加Gameplay节点：Patrol。  
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

### Fixed  
- 修复命名空间改变时反序列化问题。  
- 修复类名改变时反序列化问题。  
- 修复Color类型不能正确序列化错误。  
- 优化编辑器打开速度。  
- OnTick前增加State判断，确保为Running状态。  
- 基元类型和string不要反射查找成员。  
- 修复节点顺序不更新bug。  


## [1.1.0] - 2023-08-11
### Changed  
- 标记过时API


## [1.0.1] - 2023-08-07
### Added 
- 增加文档。

## [0.0.1] - 2023-08-30
PackageWizard Fast Created.

