using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using Megumin;
using Megumin.Serialization;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    partial class BehaviorTreeEditor
    {
        //TODO, 重构代码，不然后面还要改
        public void GenerateCode()
        {
            BehaviorTreeAsset_1_1 behaviorTree = CurrentAsset.AssetObject as BehaviorTreeAsset_1_1;
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
                    generator.Push($"static readonly Unity.Profiling.ProfilerMarker instantiateMarker = new(\"$(TreeName)_Init\");");
                    GenerateInitMethod(generator, behaviorTree);
                }
            }

            generator.Macro["$(ClassName)"] =
                BehaviorTreeCreator.GetCreatorTypeName(behaviorTree.TreeName, behaviorTree.GUID);
            generator.Macro["$(TreeName)"] = behaviorTree.TreeName;

            string filePath = $"Assets/{behaviorTree.name}_Gene.cs";
            generator.Generate(filePath);

            //Open
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
            AssetDatabase.OpenAsset(script);
        }

        public void GenerateInitMethod(CSCodeGenerator generator, BehaviorTreeAsset_1_1 behaviorTree)
        {
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
                    var varName = SafeVarName(item.Name);
                    if (item.Member != null)
                    {
                        foreach (var memberData in item.Member)
                        {
                            using (generator.NewScope)
                            {
                                generator.Push($"//SetMember {memberData.Name}");
                                if (memberData.Type == ObjectData.NullType)
                                {
                                    generator.Push($"{varName}.{memberData.Name} = null;");
                                }
                                else if (memberData.Type == ObjectData.RefType)
                                {
                                    var memberVarName = SafeVarName(memberData.Value);
                                    generator.Push($"{varName}.{memberData.Name} = {memberVarName};");
                                }
                                else
                                {
                                    
                                }
                            }
                        }
                    }
                }
                generator.PushBlankLines();

                generator.Push($"return tree;");
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
            var name = $"temp_{refName}";
            name = name.Replace('-', '_');
            name = name.Replace('.', '_');
            return name;
        }
    }
}


