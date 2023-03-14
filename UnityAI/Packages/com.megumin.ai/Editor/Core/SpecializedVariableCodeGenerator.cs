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
        public List<string> types = new()
        {
            "bool",
            "int",
            "long",
            "string",
            "float",
            "double",
            "----",
            "Vector2",
            "Vector2Int",
            "Vector3",
            "Vector3Int",
            "Vector4",
            "Rect",
            "RectInt",
            "Bounds",
            "BoundsInt",
            "----",
            "GameObject",
            "ScriptableObject",
            "Trigger",
            "Color",
            "Gradient",
            "Texture2D",
            "RenderTexture",
            "AnimationCurve",
            "Mesh",
            "SkinnedMeshRenderer",
            "Material",
        };

#if MEGUMIN_EXPLOSION4UNITY

        const string template =
@"
public class ParamVariable_$(type) : MMData3<$(type)> { }

public class VariableCreator_$(type) : VariableCreator
{
    public override string Name { get; set; } = ""$(type)"";

    public override IRefSharedable Create()
    {
        return new ParamVariable_$(type)() { Name = ""$(Type)"" };
    }
}";
        [Editor]
        public void Generate()
        {
            CSCodeGenerator generator = new();
            generator.Push($"using System;");
            generator.Push($"using System.Collections.Generic;");
            generator.Push($"using UnityEngine;");
            generator.PushBlankLines();

            generator.Push($"namespace Megumin.GameFramework.AI");
            using (generator.NewScope)
            {
                generator.Push($"public partial class VariableCreator");
                using (generator.NewScope)
                {
                    generator.PushComment("用户可以在这里添加参数类型到菜单。");
                    generator.Push($"public static List<VariableCreator> AllCreator = new()");
                    generator.BeginScope();
                    foreach (var type in types)
                    {
                        if (type == "----")
                        {
                            generator.Push($"new Separator(),");
                        }
                        else
                        {
                            generator.Push($"new VariableCreator_{type}(),");
                        }
                    }
                    generator.EndScopeWithSemicolon();
                }

                foreach (var type in types)
                {
                    if (type == "----")
                    {
                        continue;
                    }

                    var code = template.Replace("$(type)", type)
                                       .Replace("$(Type)", generator.UpperFirstChar(type));

                    generator.PushTemplate(code);
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
