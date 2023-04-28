# Binding
绑定功能。  
- BindingPath 是一个可解析的字符串路径。  
- BindingPath 绑定到一个类成员(属性，字段，方法)。  
- BindingPath 语法:  (类型：组件类|静态类|接口)/成员/....../成员/成员。  
- BindingPath 区分大小写。  

+ 常用类型是 `RefVar<T>`
+ 解析方法是 `ParseBindingResult ParseBinding(object bindInstance, bool force = false, object options = null);`
+ 解析器类是 `BindingParser` , `UnityBindingParser`

```cs
[Flags]
public enum ParseBindingResult
{
    /// <summary>
    /// Get Set 均解析失败
    /// </summary>
    None = 0,
    Get = 1 << 0,
    Set = 1 << 1,
    Both = Get | Set,
}
```

## 支持列表
- [x] 绑定属性
- [x] 绑定字段
- [x] 绑定方法(仅无参数方法)
- [ ] 绑定泛型方法(仅无参数方法)
- [ ] 绑定扩展方法(仅无参数方法)
- [ ] 绑定方法特殊参数支持(例如 gameobject , time 等)
- [x] Set绑定带有返回值的方法时，尝试忽略返回值
- [ ] 区分方法重载
- [x] 类型自动适配
- [ ] 类型自动适配时自动查找基类，协变和逆变(仅From侧)
- [x] 静态类型和成员支持
- [x] 接口支持
- [x] 非可序列化类型支持(目前为有限的支持)
- [x] 多层级绑定
- [ ] 模糊匹配
- [ ] 纯C#运行时支持，使用表达式树优化解析。
+ [x] Mono打包验证
+ [x] IL2CPP打包验证
+ [x] 手动填写BindingPath
+ [x] 快速绑定菜单

可绑定成员比快速绑定菜单中的选项更多，用户可以尝试手动编写绑定字符串，并测试解析。  

## 性能
- 最好只绑定一个级别成员，深度越大，性能越低。
- 绑定过程使用的特性越多，性能越低。
- 开销由低到高：属性 > 方法 > 字段 > 泛型方法 > 类型适配
- 性能开销分为3部分
  - 初始化时缓存所有类型部分。
  - 绑定时创建委托部分。
  - 获取值或设置值时调用委托部分。

在使用第一个绑定值时，极有可能会有巨大卡顿。  
建议在第一次调用前，使用异步初始化类型缓存。在获取值之前，使用异步解析。

## 注意
- 成员很可能被IL2CPP剪裁掉导致无法绑定，尤其是静态成员和泛型。
- BindingPath的第一个string(类型：组件类|静态类|接口)，在unity中用于识别组件，不一定包含后面的成员。

## 自动适配
- 支持绑定与目标类型不同的成员，支持规则是向上转型。  
- 可以向Megumin.Reflection.TypeAdpter中添加适配器，增加自定义类型转换。  

## 示例
参考 TestBinding.unity





