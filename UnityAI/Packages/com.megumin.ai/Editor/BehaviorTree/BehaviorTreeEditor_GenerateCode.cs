using System.Collections;
using System.Collections.Generic;
using Megumin.Binding;
using Megumin.Reflection;
using UnityEditor;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    partial class BehaviorTreeEditor
    {
        //TODO, 重构代码，不然后面还要改
        public void GenerateCode()
        {
            //BehaviorTreeAsset_1_1 behaviorTree = CurrentAsset.AssetObject as BehaviorTreeAsset_1_1;
            BehaviorTree tree = TreeView.Tree;

            if (tree == null)
            {
                return;
            }

            CSCodeGenerator generator = new();


            generator.Push($"using System;");
            generator.Push($"using Megumin.Reflection;");
            generator.Push($"using Megumin.Serialization;");
            generator.PushBlankLines();

            generator.Push($"namespace Megumin.GameFramework.AI.BehaviorTree");
            using (generator.NewScope)
            {
                generator.Push($"public sealed partial class $(ClassName) : BehaviorTreeCreator");
                using (generator.NewScope)
                {
                    GenerateInitMethod(generator, tree);
                }
            }

            string className = BehaviorTreeCreator.GetCreatorTypeName(tree.Asset.name, tree.GUID);
            generator.Macro["$(ClassName)"] = className;
            generator.Macro["$(TreeName)"] = tree.Asset.name;

            string filePath = $"Assets/{className}.cs";
            generator.Generate(filePath);

            //Open
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
            AssetDatabase.OpenAsset(script);
        }

        class DeclaredObject
        {
            public object Instance { get; set; }
            public string VarName { get; set; }
            public string RefName { get; set; }
        }

        public void GenerateInitMethod(CSCodeGenerator generator, BehaviorTree tree)
        {
            generator.Push($"static readonly Unity.Profiling.ProfilerMarker instantiateMarker = new(\"$(TreeName)_Init\");");
            generator.Push($"public override BehaviorTree Instantiate(InitOption initOption, IRefFinder refFinder = null)");
            using (generator.NewScope)
            {
                generator.Push($"using var profiler = instantiateMarker.Auto();");
                generator.PushBlankLines();

                generator.Push($"if (initOption == null)");
                using (generator.NewScope)
                {
                    generator.Push($"return null;");
                }
                generator.PushBlankLines();

                generator.Push($"//创建 引用查找器");
                generator.Push($"RefFinder finder = new();");
                generator.Push($"finder.Parent = refFinder;");
                generator.PushBlankLines();

                DeclaredObject treeObj = new();
                treeObj.Instance = tree;
                treeObj.VarName = SafeVarName(tree.GUID, tree);
                treeObj.RefName = tree.GUID;

                generator.Push($"//创建 树实例");
                generator.Push($"BehaviorTree tree = new();");
                generator.Push($"tree.GUID = \"{tree.GUID}\";");
                generator.Push($"tree.RootTree = tree;");
                generator.Push($"tree.InitOption = initOption;");
                generator.Push($"tree.RefFinder = finder;");
                generator.PushBlankLines();

                generator.Push($"var {treeObj.VarName} = tree;");
                generator.PushBlankLines();

                //Dictionary<object, string> declaredObj = new();
                Dictionary<object, DeclaredObject> declaredObjs = new();

                Dictionary<object, DeclaredObject> varis = new();
                Dictionary<object, DeclaredObject> nodes = new();
                Dictionary<object, DeclaredObject> decos = new();

                Queue<DeclaredObject> needSetMember = new();

                void DeclareObj(string refName, object obj)
                {
                    if (obj is UnityEngine.Object unityObj)
                    {
                        generator.Push($"//生成代码跳过unity对象 {refName}，请将对象添加到预制体重写");
                        return;
                    }

                    string varName = SafeVarName(refName, obj);
                    if (declaredObjs.TryGetValue(obj, out var variableName))
                    {
                        //generator.Push($"var {varName} = {variableName};");
                    }
                    else
                    {
                        DeclaredObject dclaredObject = new();
                        dclaredObject.Instance = obj;
                        dclaredObject.VarName = varName;
                        dclaredObject.RefName = refName;

                        generator.Push($"var {varName} = new {obj.GetType().ToCodeString()}();");

                        declaredObjs.Add(obj, dclaredObject);
                        needSetMember.Enqueue(dclaredObject);

                        DeclareObjMember(refName, obj);
                    }
                }

                void DeclareObjMember(string refName, object obj)
                {
                    foreach (var (memberName, memberValue, memberType) in obj.GetSerializeMembers())
                    {
                        if (memberType.IsPrimitive || memberValue is string || memberValue == null)
                        {
                            //generator.Push($"{varName}.{memberName} = {memberValue.ToCodeString()};");
                        }
                        else
                        {

                            if (memberValue is BTNode || memberValue is IDecorator)
                            {
                                //节点和装饰器统一声明。不在成员处声明
                                return;
                            }

                            //引用对象声明
                            var refMemberName = $"{refName}.{memberName}";
                            DeclareObj(refMemberName, memberValue);
                        }
                    }
                }

                generator.Push($"//创建 参数，节点，装饰器，普通对象");
                foreach (var variable in tree.Variable.Table)
                {
                    DeclareObj(variable.RefName, variable);
                    varis.Add(variable, declaredObjs[variable]);
                }
                generator.PushBlankLines();

                foreach (var node in tree.AllNodes)
                {
                    DeclareObj(node.GUID, node);
                    nodes.Add(node, declaredObjs[node]);

                    foreach (var decorator in node.Decorators)
                    {
                        DeclareObj(decorator.GUID, decorator);
                        decos.Add(decorator, declaredObjs[decorator]);
                    }

                    generator.PushBlankLines();
                }
                generator.Push($"//以上创建 {varis.Count} 参数");
                generator.Push($"//以上创建 {nodes.Count} 节点");
                generator.Push($"//以上创建 {decos.Count} 装饰器");
                generator.Push($"//以上创建 {declaredObjs.Count - nodes.Count - decos.Count - varis.Count} 普通对象");
                generator.PushBlankLines();

                generator.Push($"//以上创建 {declaredObjs.Count} 所有对象");
                generator.PushBlankLines();

                generator.Push($"//添加实例到引用查找器 {declaredObjs.Count}");
                foreach (var item in declaredObjs)
                {
                    generator.Push($"finder.RefDic.Add({item.Value.RefName.ToCodeString()}, {item.Value.VarName});");
                }
                generator.PushBlankLines();




                declaredObjs.Add(tree, treeObj);
                generator.Push($"//添加树实例到引用查找器");
                generator.Push($"finder.RefDic.Add({tree.GUID.ToCodeString()}, tree);");
                generator.PushBlankLines();



                HashSet<object> alrendySetMember = new();
                using (generator.GetRegionScope($"初始化成员值"))
                {
                    generator.Push($"//初始化成员值");
                    while (needSetMember.Count > 0)
                    {
                        var v = needSetMember.Dequeue();
                        var item = v.Instance;
                        var varName = v.VarName;

                        if (alrendySetMember.Contains(item))
                        {
                            continue;
                        }

                        foreach (var (memberName, memberValue, memberType, isGetPublic, isSetPublic)
                            in item.GetSerializeMembers())
                        {
                            if (memberType.IsPrimitive || memberValue is string || memberValue == null)
                            {
                                string memberValueCode = memberValue.ToCodeString();
                                if (item is IList)
                                {
                                    generator.Push($"{varName}.Insert({memberName}, {memberValueCode});");
                                }
                                else if (isSetPublic)
                                {
                                    generator.Push($"{varName}.{memberName} = {memberValueCode};");
                                }
                                else
                                {
                                    if (typeof(IRefable).IsAssignableFrom(item.GetType()) && memberName == "refName")
                                    {
                                        generator.Push($"{varName}.{nameof(IRefable.RefName)} = {memberValueCode};");
                                    }
                                    else if (typeof(IBindable).IsAssignableFrom(item.GetType()) && memberName == "bindingPath")
                                    {
                                        generator.Push($"{varName}.{nameof(IBindable.BindingPath)} = {memberValueCode};");
                                    }
                                    else
                                    {
                                        generator.Push($"{varName}.TrySetMemberValue({memberName.ToCodeString()}, {memberValueCode});");
                                    }
                                }
                            }
                            else
                            {
                                string memberRefName = $"{declaredObjs[item].RefName}.{memberName}";
                                string memberValueCode = SafeVarName($"ref_{memberRefName}");

                                generator.Push($"if (finder.TryGetRefValue<{memberType.ToCodeString()}>(");
                                generator.Push($"{memberRefName.ToCodeString()},", 1);
                                generator.Push($"out var {memberValueCode}))", 1);

                                generator.BeginScope();

                                if (item is IList)
                                {
                                    generator.Push($"{varName}.Insert({memberName}, {memberValueCode});");
                                }
                                else if (isSetPublic)
                                {
                                    generator.Push($"{varName}.{memberName} = {memberValueCode};");
                                }
                                else
                                {
                                    generator.Push($"{varName}.TrySetMemberValue({memberName.ToCodeString()}, {memberValueCode});");
                                }

                                generator.EndScope();
                                generator.PushBlankLines();
                            }
                        }

                        alrendySetMember.Add(item);

                        generator.PushBlankLines();
                    }
                }

                using (generator.GetRegionScope("添加实例到树"))
                {
                    //generator.Push($"//添加到集合");
                    //generator.PushBlankLines();

                    generator.Push($"//添加参数");
                    foreach (var item in varis)
                    {
                        generator.Push($"tree.InitAddVariable({item.Value.VarName});");
                    }
                    generator.Push($"//以上添加到树 {varis.Count} 参数实例");
                    generator.PushBlankLines();

                    generator.Push($"//添加普通对象");
                    //先处理非节点装饰器对象
                    int objCount = 0;
                    foreach (var item in alrendySetMember)
                    {
                        if (varis.ContainsKey(item))
                        {
                            continue;
                        }

                        if (nodes.ContainsKey(item))
                        {
                            continue;
                        }

                        if (decos.ContainsKey(item))
                        {
                            continue;
                        }

                        generator.Push($"tree.InitAddTreeRefObj({declaredObjs[item].VarName});");
                        objCount++;
                    }
                    generator.Push($"//以上添加到树 {objCount} 普通对象");
                    generator.PushBlankLines();

                    generator.Push($"//添加装饰器");
                    foreach (var item in decos)
                    {
                        generator.Push($"tree.InitAddTreeRefObj({item.Value.VarName});");
                    }
                    generator.Push($"//以上添加到树 {decos.Count} 装饰器");
                    generator.PushBlankLines();

                    generator.Push($"//添加节点");
                    foreach (var item in nodes)
                    {
                        generator.Push($"tree.InitAddTreeRefObj({item.Value.VarName});");
                    }
                    generator.Push($"//以上添加到树 {nodes.Count} 节点");
                    generator.PushBlankLines();

                }

                using (generator.GetRegionScope($"设置开始节点 和 装饰器Owner"))
                {
                    generator.Push($"tree.StartNode = {declaredObjs[tree.StartNode].VarName};");

                    foreach (var item in decos)
                    {
                        generator.Push($"{item.Value.VarName}.Owner = {declaredObjs[(item.Key as IDecorator).Owner].VarName};");
                    }
                }

                generator.Push($"tree.UpdateNodeIndexDepth();");

                generator.PushWrapBlankLines($"PostInit(initOption, tree);");

                generator.Push($"return tree;");
            }
        }

        public string SafeVarName(string refName, object obj = null)
        {
            var name = refName;
            if (obj is BehaviorTree tree)
            {
                name = $"tree_{refName}";
            }
            else if (obj is BTNode node)
            {
                name = $"node_{refName}";
            }
            else if (obj is IDecorator deco)
            {
                name = $"deco_{refName}";
            }
            else if (obj is ITreeElement elem)
            {
                name = $"elem_{refName}";
            }
            else if (name.StartsWith("temp_") == false)
            {
                name = $"temp_{refName}";
            }

            name = name.Replace('-', '_');
            name = name.Replace('.', '_');
            return name;
        }
    }
}


