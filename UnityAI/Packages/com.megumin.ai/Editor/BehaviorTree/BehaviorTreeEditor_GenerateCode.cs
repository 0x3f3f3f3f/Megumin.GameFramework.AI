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
            CSCodeGenerator codeGenerator = new();











            string filePath = $"Assets/{behaviorTree.name}_Gene.cs";
            codeGenerator.Generate(filePath);

            //Open
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
            AssetDatabase.OpenAsset(script);
        }
    }
}


