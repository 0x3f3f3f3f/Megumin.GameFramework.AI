using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

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
                    var varName = SafeVarName(item.Name);
                    generator.Push($"var {varName} = new {item.Type}();");
                }

                generator.Push($"return tree;");
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


