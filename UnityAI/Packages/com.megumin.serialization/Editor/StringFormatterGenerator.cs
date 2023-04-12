using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Megumin.GameFramework.AI
{
    internal class StringFormatterGenerator : ScriptableObject
    {
        public UnityEngine.Object Folder;
        public List<string> types = new()
        {
            "bool",
            "byte",
            "sbyte",
            "char",
            "short",
            "ushort",
            "int",
            "uint",
            "long",
            "ulong",
            "float",
            "double",
            //"string",
        };

        const string template =
@"
public sealed class $(Type)Formatter : IFormatter<string, $(type)>
{
    public string Serialize(object value)
    {
        return value.ToString();
    }

    public bool TrySerialize(object value, out string destination)
    {
        destination = value.ToString();
        return true;
    }

    public bool TryDeserialize(string source, out object value)
    {
        if ($(type).TryParse(source, out var result))
        {
            value = result;
            return true;
        }
        value = default;
        return false;
    }

    public bool TrySerialize($(type) value, out string destination)
    {
        destination = value.ToString();
        return true;
    }

    public bool TryDeserialize(string source, out $(type) value)
    {
        if ($(type).TryParse(source, out value))
        {
            return true;
        }

        value = default;
        return false;
    }
}";
        [Editor]
        public void Generate()
        {
            CSCodeGenerator generator = new();
            generator.Push($"using System;");
            generator.Push($"using System.Collections.Generic;");
            generator.PushBlankLines();

            generator.Push($"namespace Megumin.Serialization");
            using (generator.NewScope)
            {
                generator.Push($"public partial class StringFormatter");
                using (generator.NewScope)
                {
                    foreach (var type in types)
                    {
                        var length = 1;
                        if (type.StartsWith("u"))
                        {
                            length = 2;
                        }

                        if (type == "sbyte")
                        {
                            length = 2;
                        }

                        var code = template.Replace("$(type)", type)
                                           .Replace("$(Type)", generator.UpperStartChar(type, length));

                        generator.PushTemplate(code);
                    }
                }
            }



            var fileName = "StringFormatterPrimitive.cs";
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
