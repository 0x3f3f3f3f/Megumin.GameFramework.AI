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
        public void GenerateCode()
        {
            BehaviorTreeAsset_1_1 behaviorTree = CurrentAsset.AssetObject as BehaviorTreeAsset_1_1;
            CSCodeGenerator generator = new();


            generator.Push($"using System;");
            generator.PushBlankLines();

            generator.Push($"namespace Megumin.GameFramework.AI.BehaviorTree");
            using (generator.NewScope)
            {
                generator.Push($"public partial class $(ClassName) : BehaviorTreeCreator");
                using (generator.NewScope)
                {

                }
            }

            generator.Macro["$(ClassName)"] =
                BehaviorTreeCreator.GetCreatorTypeName(behaviorTree.TreeName, behaviorTree.GUID);


            string filePath = $"Assets/{behaviorTree.name}_Gene.cs";
            generator.Generate(filePath);

            //Open
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
            AssetDatabase.OpenAsset(script);
        }
    }
}


