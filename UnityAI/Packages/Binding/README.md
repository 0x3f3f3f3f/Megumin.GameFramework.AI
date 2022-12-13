# Binding
绑定功能。  
通过BindingPath绑定到一个目标。  
BindingString格式:  (类型：组件类|静态类|接口)/成员/....../成员/成员。  

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
- [ ] 类型自动适配
- [x] 静态类型和成员支持
- [x] 接口支持
- [x] 非可序列化类型支持(目前为有限的支持)
- [x] 多层级绑定
- [ ] 模糊匹配
- [ ] 纯C#运行时支持，使用表达式树优化解析。
+ [x] Mono打包验证
+ [x] IL2CPP打包验证
+ [x] 手动填写BindingPath
+ [ ] 快速绑定工具Unity编辑器


## 性能
最好只绑定一个级别成员，深度越大，性能越低。

## 注意
- 成员很可能被IL2CPP剪裁掉导致无法绑定。

## 示例
参考 BindTestBehaviour.cs





