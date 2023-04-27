using System;
using System.Collections;
using System.Collections.Generic;
using Megumin.Reflection;
using Megumin.Serialization;
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

            generator.Macro["$(ClassName)"] =
                BehaviorTreeCreator.GetCreatorTypeName(tree.Asset.name, tree.GUID);
            generator.Macro["$(TreeName)"] = tree.Asset.name;

            string filePath = $"Assets/{tree.Asset.name}_Gene.cs";
            generator.Generate(filePath);

            //Open
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
            AssetDatabase.OpenAsset(script);
        }

        public void GenerateInitMethod(CSCodeGenerator generator, BehaviorTreeAsset_1_1 behaviorTree)
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

                generator.Push($"BehaviorTree tree = new();");
                generator.Push($"tree.GUID = \"{behaviorTree.GUID}\";");
                generator.Push($"tree.RootTree = tree;");
                generator.Push($"tree.InitOption = initOption;");
                generator.PushBlankLines();


                generator.Push("//生成节点");
                foreach (var item in behaviorTree.nodes)
                {
                    DeclareObj(generator, item);
                }

                generator.PushBlankLines();

                generator.Push("//生成装饰器");
                foreach (var item in behaviorTree.decorators)
                {
                    DeclareObj(generator, item);
                }
                generator.PushBlankLines();

                generator.Push("//生成ref obj");
                foreach (var item in behaviorTree.refObjs)
                {
                    DeclareObj(generator, item);
                }
                generator.PushBlankLines();

                generator.Push("//反序列化 nodes");
                foreach (var item in behaviorTree.nodes)
                {
                    Deserialize(generator, item);
                }
                generator.PushBlankLines();

                generator.Push("//反序列化 decorators");
                foreach (var item in behaviorTree.decorators)
                {
                    Deserialize(generator, item);
                }
                generator.PushBlankLines();

                //generator.Push("//反序列化 ref obj");
                //foreach (var item in behaviorTree.refObjs)
                //{
                //    Deserialize(generator, item);
                //}
                //generator.PushBlankLines();

                generator.Push($"return tree;");
            }
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

                generator.Push($"BehaviorTree tree = new();");
                generator.Push($"tree.GUID = \"{tree.GUID}\";");
                generator.Push($"tree.RootTree = tree;");
                generator.Push($"tree.InitOption = initOption;");
                generator.PushBlankLines();

                Dictionary<object, string> declaredObj = new();
                Queue<(object, string)> needSetMember = new();

                //缓存所有已知引用对象
                declaredObj.Add(tree, SafeVarName(tree.GUID));



                void DeclareObj(string varName, object obj)
                {
                    if (declaredObj.TryGetValue(obj, out var variableName))
                    {
                        if (varName != variableName)
                        {
                            generator.Push($"var {varName} = {variableName};");
                            declaredObj.Add(obj, varName);
                        }
                    }
                    else
                    {
                        generator.Push($"var {varName} = new {obj.GetType().ToCodeString()}();");
                        declaredObj.Add(obj, varName);

                        needSetMember.Enqueue((obj, varName));
                    }

                    DeclareObjMember(varName, obj);
                }

                void DeclareObjMember(string varName, object obj)
                {
                    foreach (var (memberName, memberValue, memberType) in obj.GetSerializeMembers())
                    {
                        if (memberType.IsPrimitive || memberValue is string || memberValue == null)
                        {
                            //generator.Push($"{varName}.{memberName} = {memberValue.ToCodeString()};");
                        }
                        else
                        {
                            //引用对象声明
                            var varMemberName = SafeVarName($"{varName}.{memberName}");
                            DeclareObj(varMemberName, memberValue);
                        }
                    }
                }

                generator.Push("//声明复杂对象");
                foreach (var variable in tree.Variable.Table)
                {
                    string varName = SafeVarName(variable.RefName);
                    DeclareObj(varName, variable);
                    generator.PushBlankLines();
                }

                foreach (var node in tree.AllNodes)
                {
                    string varName = SafeVarName(node.GUID);
                    DeclareObj(varName, node);

                    foreach (var decorator in node.Decorators)
                    {
                        string varName1 = SafeVarName(decorator.GUID);
                        DeclareObj(varName1, decorator);
                    }

                    generator.PushBlankLines();
                }

                HashSet<object> alrendySetMember = new();
                generator.Push("//生成member代码");

                while (needSetMember.Count > 0)
                {
                    var v = needSetMember.Dequeue();
                    var item = v.Item1;
                    var varName = v.Item2;

                    if (alrendySetMember.Contains(item))
                    {
                        continue;
                    }

                    if (item is IList list)
                    {
                        if (item is Array)
                        {

                        }
                        else
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                object elem = list[i];
                                generator.Push($"//TODO :{varName} list {i}");
                            }
                        }
                    }
                    else
                    {
                        foreach (var (memberName, memberValue, memberType) in item.GetSerializeMembers())
                        {
                            if (memberType.IsPrimitive || memberValue is string || memberValue == null)
                            {
                                generator.Push($"{varName}.{memberName} = {memberValue.ToCodeString()};");
                            }
                            else
                            {
                                //引用对象声明
                                if (declaredObj.TryGetValue(memberValue, out var dName))
                                {
                                    generator.Push($"{varName}.{memberName} = {dName};");
                                }
                                else
                                {
                                    generator.Push($"//TODO : {memberName}");
                                }
                            }
                        }
                    }

                    alrendySetMember.Add(item);

                    generator.PushBlankLines();
                }

                generator.Push($"return tree;");
            }
        }

        public void Deserialize(CSCodeGenerator generator, ObjectData item)
        {
            using (generator.GetRegionScope(item.Name))
            {
                var varName = SafeVarName(item.Name);
                if (item.Member != null)
                {
                    foreach (var memberData in item.Member)
                    {
                        using (generator.NewScope)
                        {
                            generator.Push($"//SetMember {memberData.Name}");

                            var resultString = $"//{memberData.Type} can not parse!";

                            if (memberData.Type == ObjectData.NullType)
                            {
                                resultString = "null";
                            }
                            else if (memberData.Type == ObjectData.RefType)
                            {
                                resultString = SafeVarName(memberData.Value);
                            }
                            else
                            {
                                if (StringFormatter.TryDeserialize(memberData.Type, memberData.Value, out var value))
                                {
                                    resultString = value.ToCodeString();
                                }
                            }

                            generator.Push($"{varName}.{memberData.Name} = {resultString};");
                        }

                        generator.PushBlankLines();
                    }
                }
            }
        }

        /// <summary>
        /// 声明对象
        /// </summary>
        /// <param name="generator"></param>
        /// <param name="item"></param>
        public void DeclareObj(CSCodeGenerator generator, ObjectData item)
        {
            var varName = SafeVarName(item.Name);
            if (Megumin.Reflection.TypeCache.TryGetType(item.Type, out var type))
            {
                generator.Push($"var {varName} = new {type.ToCodeString()}();");
            }
            else
            {
                generator.Push($"//{item.Type} can not parse!");
            }
        }

        public string SafeVarName(string refName)
        {
            var name = refName;
            if (name.StartsWith("temp_") == false)
            {
                name = $"temp_{refName}";
            }

            name = name.Replace('-', '_');
            name = name.Replace('.', '_');
            return name;
        }
    }
}


