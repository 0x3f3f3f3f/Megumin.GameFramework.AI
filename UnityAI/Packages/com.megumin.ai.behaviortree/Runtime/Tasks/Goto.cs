using System;
using System.ComponentModel;
using Megumin;
using Megumin.Binding;
using Megumin.Perception;
using Megumin.Reflection;
using Megumin.Serialization;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace Megumin.AI.BehaviorTree
{
    //需要绘制引用节点UI Ref需要扩展

    /// <summary>
    /// Goto 节点。
    /// 就像目标节点是自己的子节点一样。
    /// 大杀器，极大程度弥补行为树的不足。
    /// 部分等价与菱形环形结构，突破了树形结构的限制。
    /// 但是也是混乱的开端。潘多拉魔盒。极容易导致运行时bug。
    /// 尽可能小心使用这个节点。
    /// </summary>
    [Serializable]
    [DisplayName("Goto")]
    [Category("Utility")]
    [AddComponentMenu("Goto")]
    //[Color(1f, 47f / 51f, 0.0156862754f, 1f)]
    //[Icon("")]
    //[SerializationAlias("Megumin.AI.BehaviorTree.Goto")]
    //[MovedFrom(true, sourceNamespace: "Megumin.AI.BehaviorTree", sourceAssembly: null, sourceClassName: "Goto")]
    [HelpURL(URL.WikiTask + "Goto")]
    public sealed class Goto : BTActionNode, IDataValidable, IDetailable
    {
        [RefSetter(Filter = "Node", CanExpand = true, ReadOnly = true)]
        public BTNode TargetNode;

        protected override Status OnTick(BTNode from, object options = null)
        {
            if (TargetNode != null)
            {
                return TargetNode.Tick(this, options);
            }

            return GetIgnoreResult(from);
        }

        public (int Result, string ToolTip) Valid()
        {
            if (TargetNode == null)
            {
                return (-1, "no target, ignore this");
            }

            return (0, "");
        }

        public string GetDetail()
        {
            if (TargetNode != null)
            {
                return $"idx:{TargetNode.Index}  {TargetNode.ShortGUID}";
            }
            return null;
        }
    }
}



