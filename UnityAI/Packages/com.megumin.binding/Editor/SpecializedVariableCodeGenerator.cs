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
            "Transform",
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

        const string template =
@"
[Serializable]
public class RefVar_$(Type) : RefVar<$(type)> { }

public class VariableCreator_$(Type) : VariableCreator
{
    public override string Name { get; set; } = ""$(type)"";

    public override IRefable Create()
    {
        return new RefVar_$(Type)() { RefName = ""$(Type)"" };
    }
}

[Serializable]
public class RefVar_$(Type)_List : RefVar<List<$(type)>> { }

public class VariableCreator_$(Type)_List : VariableCreator
{
    public override string Name { get; set; } = ""List/$(type)"";

    public override IRefable Create()
    {
        return new RefVar_$(Type)_List() { RefName = ""List<$(Type)>"", value = new() };
    }
}

[Serializable]
public class RefVar_$(Type)_Array : RefVar<$(type)[]> { }

public class VariableCreator_$(Type)_Array : VariableCreator
{
    public override string Name { get; set; } = ""Array/$(type)"";

    public override IRefable Create()
    {
        return new RefVar_$(Type)_Array() { RefName = ""Array<$(Type)>"" };
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

            generator.Push($"namespace Megumin.Binding");
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
                        var Type = generator.UpperStartChar(type);
                        if (type == "----")
                        {
                            generator.Push($"new Separator() {{ Name = \"Array/\" }},");
                        }
                        else
                        {
                            generator.Push($"new VariableCreator_{Type}_Array(),");
                        }
                    }
                    generator.Push($"new Separator(),");

                    foreach (var type in types)
                    {
                        var Type = generator.UpperStartChar(type);
                        if (type == "----")
                        {
                            generator.Push($"new Separator() {{ Name = \"List/\" }},");
                        }
                        else
                        {
                            generator.Push($"new VariableCreator_{Type}_List(),");
                        }
                    }
                    generator.Push($"new Separator(),");

                    foreach (var type in types)
                    {
                        var Type = generator.UpperStartChar(type);
                        if (type == "----")
                        {
                            generator.Push($"new Separator(),");
                        }
                        else
                        {
                            generator.Push($"new VariableCreator_{Type}(),");
                        }
                    }
                    generator.EndScopeWithSemicolon();
                }

                foreach (var type in types)
                {
                    var Type = generator.UpperStartChar(type);
                    if (type == "----")
                    {
                        continue;
                    }

                    var code = template.Replace("$(type)", type)
                                       .Replace("$(Type)", Type);

                    generator.PushTemplate(code);
                }
            }



            var fileName = "SpecializedVariable.cs";
            var dir = AssetDatabase.GetAssetPath(Folder);
            string filePath = Path.Combine(dir, fileName);
            var path = Path.GetFullPath(filePath);
            Debug.Log(path);
            generator.Generate(path);

            //Open
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
            AssetDatabase.OpenAsset(script);
        }
    }
}
