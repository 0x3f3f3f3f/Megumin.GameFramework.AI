namespace Megumin.Binding
{
    public partial class VariableCreator
    {
        ///// <summary>
        ///// 用户可以在这里添加参数类型到菜单。
        ///// </summary>
        //public static List<VariableCreator> AllCreator = new()
        //{
        //    new VariableCreator<bool>(),
        //    new VariableCreator<int>(),
        //    new VariableCreator<long>(),
        //    new VariableCreator<string>(),
        //    new VariableCreator<float>(),
        //    new VariableCreator<double>(),
        //    new Separator(),
        //    new VariableCreator<Vector2>(),
        //    new VariableCreator<Vector2Int>(),
        //    new VariableCreator<Vector3>(),
        //    new VariableCreator<Vector3Int>(),
        //    new VariableCreator<Vector4>(),
        //    new VariableCreator<Rect>(),
        //    new VariableCreator<RectInt>(),
        //    new VariableCreator<Bounds>(),
        //    new VariableCreator<BoundsInt>(),
        //    new Separator(),
        //    new VariableCreator<GameObject>(),
        //    new VariableCreator<ScriptableObject>(),
        //    new VariableCreator<Trigger>(),
        //    new VariableCreator<Color>(),
        //    new VariableCreator<Gradient>(),
        //    new VariableCreator<Texture2D>(),
        //    new VariableCreator<RenderTexture>(),
        //    new VariableCreator<AnimationCurve>(),
        //    new VariableCreator<Mesh>(),
        //    new VariableCreator<SkinnedMeshRenderer>(),
        //    new VariableCreator<Material>(),
        //};

        public virtual bool IsSeparator { get; set; }
        public virtual string Name { get; set; } = "VariableCreator";

        /// <summary>
        /// Unity 2023之前的版本不支持 多态 + 泛型序列化，所以用户自定义时不要返回泛型类型，一定要声明一个非泛型类型参数。
        /// </summary>
        /// <returns></returns>
        public virtual IRefable Create()
        {
            return new RefVar<int>() { RefName = "VariableCreator" };
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

        public override IRefable Create()
        {
            return new RefVar<T>() { RefName = this.Name };
        }
    }
}
