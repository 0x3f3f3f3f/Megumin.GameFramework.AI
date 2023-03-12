﻿///********************************************************************************************************************************
///本页代码由代码生成器生成，请勿手动修改。The code on this page is generated by the code generator, do not manually modify.
///生成器类型：$(CodeGenericType)
///配置源文件：$(CodeGenericSourceFilePath)
///********************************************************************************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.GameFramework.AI
{
    public partial class VariableCreator
    {
        /// <summary>
        /// 用户可以在这里添加参数类型到菜单。
        /// </summary>
        public static List<VariableCreator> AllCreator = new()
        {
            new VariableCreator_bool(),
            new VariableCreator_int(),
            new VariableCreator_long(),
            new VariableCreator_string(),
            new VariableCreator_float(),
            new VariableCreator_double(),
            new Separator(),
            new VariableCreator_Vector2(),
            new VariableCreator_Vector2Int(),
            new VariableCreator_Vector3(),
            new VariableCreator_Vector3Int(),
            new VariableCreator_Vector4(),
            new VariableCreator_Rect(),
            new VariableCreator_RectInt(),
            new VariableCreator_Bounds(),
            new VariableCreator_BoundsInt(),
            new Separator(),
            new VariableCreator_GameObject(),
            new VariableCreator_ScriptableObject(),
            new VariableCreator_Trigger(),
            new VariableCreator_Color(),
            new VariableCreator_Gradient(),
            new VariableCreator_Texture2D(),
            new VariableCreator_RenderTexture(),
            new VariableCreator_AnimationCurve(),
            new VariableCreator_Mesh(),
            new VariableCreator_SkinnedMeshRenderer(),
            new VariableCreator_Material(),
        };
    }

    public class ParamVariable_bool : ParamVariable<bool> { }

    public class VariableCreator_bool : VariableCreator
    {
        public override string Name { get; set; } = "bool";

        public override TestVariable Create()
        {
            return new ParamVariable_bool() { Name = "Bool" };
        }
    }

    public class ParamVariable_int : ParamVariable<int> { }

    public class VariableCreator_int : VariableCreator
    {
        public override string Name { get; set; } = "int";

        public override TestVariable Create()
        {
            return new ParamVariable_int() { Name = "Int" };
        }
    }

    public class ParamVariable_long : ParamVariable<long> { }

    public class VariableCreator_long : VariableCreator
    {
        public override string Name { get; set; } = "long";

        public override TestVariable Create()
        {
            return new ParamVariable_long() { Name = "Long" };
        }
    }

    public class ParamVariable_string : ParamVariable<string> { }

    public class VariableCreator_string : VariableCreator
    {
        public override string Name { get; set; } = "string";

        public override TestVariable Create()
        {
            return new ParamVariable_string() { Name = "String" };
        }
    }

    public class ParamVariable_float : ParamVariable<float> { }

    public class VariableCreator_float : VariableCreator
    {
        public override string Name { get; set; } = "float";

        public override TestVariable Create()
        {
            return new ParamVariable_float() { Name = "Float" };
        }
    }

    public class ParamVariable_double : ParamVariable<double> { }

    public class VariableCreator_double : VariableCreator
    {
        public override string Name { get; set; } = "double";

        public override TestVariable Create()
        {
            return new ParamVariable_double() { Name = "Double" };
        }
    }

    public class ParamVariable_Vector2 : ParamVariable<Vector2> { }

    public class VariableCreator_Vector2 : VariableCreator
    {
        public override string Name { get; set; } = "Vector2";

        public override TestVariable Create()
        {
            return new ParamVariable_Vector2() { Name = "Vector2" };
        }
    }

    public class ParamVariable_Vector2Int : ParamVariable<Vector2Int> { }

    public class VariableCreator_Vector2Int : VariableCreator
    {
        public override string Name { get; set; } = "Vector2Int";

        public override TestVariable Create()
        {
            return new ParamVariable_Vector2Int() { Name = "Vector2Int" };
        }
    }

    public class ParamVariable_Vector3 : ParamVariable<Vector3> { }

    public class VariableCreator_Vector3 : VariableCreator
    {
        public override string Name { get; set; } = "Vector3";

        public override TestVariable Create()
        {
            return new ParamVariable_Vector3() { Name = "Vector3" };
        }
    }

    public class ParamVariable_Vector3Int : ParamVariable<Vector3Int> { }

    public class VariableCreator_Vector3Int : VariableCreator
    {
        public override string Name { get; set; } = "Vector3Int";

        public override TestVariable Create()
        {
            return new ParamVariable_Vector3Int() { Name = "Vector3Int" };
        }
    }

    public class ParamVariable_Vector4 : ParamVariable<Vector4> { }

    public class VariableCreator_Vector4 : VariableCreator
    {
        public override string Name { get; set; } = "Vector4";

        public override TestVariable Create()
        {
            return new ParamVariable_Vector4() { Name = "Vector4" };
        }
    }

    public class ParamVariable_Rect : ParamVariable<Rect> { }

    public class VariableCreator_Rect : VariableCreator
    {
        public override string Name { get; set; } = "Rect";

        public override TestVariable Create()
        {
            return new ParamVariable_Rect() { Name = "Rect" };
        }
    }

    public class ParamVariable_RectInt : ParamVariable<RectInt> { }

    public class VariableCreator_RectInt : VariableCreator
    {
        public override string Name { get; set; } = "RectInt";

        public override TestVariable Create()
        {
            return new ParamVariable_RectInt() { Name = "RectInt" };
        }
    }

    public class ParamVariable_Bounds : ParamVariable<Bounds> { }

    public class VariableCreator_Bounds : VariableCreator
    {
        public override string Name { get; set; } = "Bounds";

        public override TestVariable Create()
        {
            return new ParamVariable_Bounds() { Name = "Bounds" };
        }
    }

    public class ParamVariable_BoundsInt : ParamVariable<BoundsInt> { }

    public class VariableCreator_BoundsInt : VariableCreator
    {
        public override string Name { get; set; } = "BoundsInt";

        public override TestVariable Create()
        {
            return new ParamVariable_BoundsInt() { Name = "BoundsInt" };
        }
    }

    public class ParamVariable_GameObject : ParamVariable<GameObject> { }

    public class VariableCreator_GameObject : VariableCreator
    {
        public override string Name { get; set; } = "GameObject";

        public override TestVariable Create()
        {
            return new ParamVariable_GameObject() { Name = "GameObject" };
        }
    }

    public class ParamVariable_ScriptableObject : ParamVariable<ScriptableObject> { }

    public class VariableCreator_ScriptableObject : VariableCreator
    {
        public override string Name { get; set; } = "ScriptableObject";

        public override TestVariable Create()
        {
            return new ParamVariable_ScriptableObject() { Name = "ScriptableObject" };
        }
    }

    public class ParamVariable_Trigger : ParamVariable<Trigger> { }

    public class VariableCreator_Trigger : VariableCreator
    {
        public override string Name { get; set; } = "Trigger";

        public override TestVariable Create()
        {
            return new ParamVariable_Trigger() { Name = "Trigger" };
        }
    }

    public class ParamVariable_Color : ParamVariable<Color> { }

    public class VariableCreator_Color : VariableCreator
    {
        public override string Name { get; set; } = "Color";

        public override TestVariable Create()
        {
            return new ParamVariable_Color() { Name = "Color" };
        }
    }

    public class ParamVariable_Gradient : ParamVariable<Gradient> { }

    public class VariableCreator_Gradient : VariableCreator
    {
        public override string Name { get; set; } = "Gradient";

        public override TestVariable Create()
        {
            return new ParamVariable_Gradient() { Name = "Gradient" };
        }
    }

    public class ParamVariable_Texture2D : ParamVariable<Texture2D> { }

    public class VariableCreator_Texture2D : VariableCreator
    {
        public override string Name { get; set; } = "Texture2D";

        public override TestVariable Create()
        {
            return new ParamVariable_Texture2D() { Name = "Texture2D" };
        }
    }

    public class ParamVariable_RenderTexture : ParamVariable<RenderTexture> { }

    public class VariableCreator_RenderTexture : VariableCreator
    {
        public override string Name { get; set; } = "RenderTexture";

        public override TestVariable Create()
        {
            return new ParamVariable_RenderTexture() { Name = "RenderTexture" };
        }
    }

    public class ParamVariable_AnimationCurve : ParamVariable<AnimationCurve> { }

    public class VariableCreator_AnimationCurve : VariableCreator
    {
        public override string Name { get; set; } = "AnimationCurve";

        public override TestVariable Create()
        {
            return new ParamVariable_AnimationCurve() { Name = "AnimationCurve" };
        }
    }

    public class ParamVariable_Mesh : ParamVariable<Mesh> { }

    public class VariableCreator_Mesh : VariableCreator
    {
        public override string Name { get; set; } = "Mesh";

        public override TestVariable Create()
        {
            return new ParamVariable_Mesh() { Name = "Mesh" };
        }
    }

    public class ParamVariable_SkinnedMeshRenderer : ParamVariable<SkinnedMeshRenderer> { }

    public class VariableCreator_SkinnedMeshRenderer : VariableCreator
    {
        public override string Name { get; set; } = "SkinnedMeshRenderer";

        public override TestVariable Create()
        {
            return new ParamVariable_SkinnedMeshRenderer() { Name = "SkinnedMeshRenderer" };
        }
    }

    public class ParamVariable_Material : ParamVariable<Material> { }

    public class VariableCreator_Material : VariableCreator
    {
        public override string Name { get; set; } = "Material";

        public override TestVariable Create()
        {
            return new ParamVariable_Material() { Name = "Material" };
        }
    }
}