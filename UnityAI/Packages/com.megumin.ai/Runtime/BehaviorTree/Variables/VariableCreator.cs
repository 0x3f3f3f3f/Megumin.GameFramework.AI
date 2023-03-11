using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
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
            new VariableCreator<string>(),
            new VariableCreator<float>(),
            new VariableCreator<double>(),
            new Separator(),
            new VariableCreator<Vector2>(),
            new Separator(),
            new VariableCreator<Color>(),
        };

        public bool IsSeparator { get; set; }
        public virtual string Name { get; set; } = "VariableCreator";

        public virtual TestVariable Create()
        {
            return new ParamVariable<int>() { Name = "VariableCreator" };
        }

        public class Separator : VariableCreator
        {
            public override string Name { get; set; } = $"/";
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
