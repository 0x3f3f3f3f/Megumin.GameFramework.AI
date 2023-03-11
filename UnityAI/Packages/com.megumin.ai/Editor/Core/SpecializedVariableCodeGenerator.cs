using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Megumin;
using System.ComponentModel;
using System.IO;
using UnityEditor;

namespace Megumin.GameFramework.AI
{
    internal class SpecializedVariableCodeGenerator : ScriptableObject
    {
        public UnityEngine.Object Folder;

#if MEGUMIN_EXPLOSION4UNITY

        [Editor]
        public void Generate()
        {
            CSCodeGenerator generator = new();
            generator.Push($"using System;");
            generator.PushBlankLines();

            generator.Push($"namespace Megumin.GameFramework.AI");
            using (generator.NewScope)
            {
                foreach (var type in VariableCreator.AllCreator)
                {
                    
                }
            }



            var fileName = "SpecializedVariable.cs";
            var dir = AssetDatabase.GetAssetPath(Folder);
            var path = Path.GetFullPath(Path.Combine(dir, fileName));
            Debug.Log(path);
            generator.Generate(path);

            //Open
            string assetPath = path.MakeUnityProjectRelativePath();
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            AssetDatabase.OpenAsset(script);
        }

#endif
    }
}
