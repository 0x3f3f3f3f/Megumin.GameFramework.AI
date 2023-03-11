using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI
{
    public class VariableCreator
    {
        /// <summary>
        /// 用户可以在这里添加参数类型到菜单。
        /// </summary>
        public static List<VariableCreator> AllCreator = new()
        {
            new VariableCreator<bool>(),
            new VariableCreator<int>(),
            new VariableCreator<long>(),
            new VariableCreator<string>(),
            new VariableCreator<float>(),
            new VariableCreator<double>(),
            new Separator(),
            new VariableCreator<Vector2>(),
            new VariableCreator<Vector2Int>(),
            new VariableCreator<Vector3>(),
            new VariableCreator<Vector3Int>(),
            new VariableCreator<Vector4>(),
            new VariableCreator<Rect>(),
            new VariableCreator<RectInt>(),
            new VariableCreator<Bounds>(),
            new VariableCreator<BoundsInt>(),
            new Separator(),
            new VariableCreator<GameObject>(),
            new VariableCreator<ScriptableObject>(),
            new VariableCreator<Trigger>(),
            new VariableCreator<Color>(),
            new VariableCreator<Gradient>(),
            new VariableCreator<Texture2D>(),
            new VariableCreator<RenderTexture>(),
            new VariableCreator<AnimationCurve>(),
            new VariableCreator<Mesh>(),
            new VariableCreator<SkinnedMeshRenderer>(),
            new VariableCreator<Material>(),
        };

        public virtual bool IsSeparator { get; set; }
        public virtual string Name { get; set; } = "VariableCreator";

        public virtual TestVariable Create()
        {
            return new ParamVariable<int>() { Name = "VariableCreator" };
        }

        public class Separator : VariableCreator
        {
            public override string Name { get; set; } = $"";
            public override bool IsSeparator { get; set; } = true;
        }
    }

    public class VariableCreator<T> : VariableCreator
    {
        public override string Name { get; set; } = typeof(T).Name;

        public override TestVariable Create()
        {
            return new ParamVariable<T>() { Name = this.Name };
        }
    }
}
