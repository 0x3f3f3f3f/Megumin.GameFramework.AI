using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using System;
using UnityEditor;
using Megumin.GameFramework.AI.Editor;
using System.Linq;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeView
    {
        /// <summary>
        /// 正常是将复制的元素序列化为字符串。粘贴时将字符串反序列为新对象
        /// 使用一个取巧的方法做复制粘贴功能。
        /// 使用一个静态成员将要复制的元素保存起来。静态是因为可能再多个编辑器实例中复制粘贴。
        /// 缺点：无法在不同的项目之间实现复制粘贴。
        /// </summary>
        static HashSet<GraphElement> copyedElement = new ();

        private string OnSerializeGraphElements(IEnumerable<GraphElement> elements)
        {
            this.LogFuncName();
            var hashset = elements.ToHashSet();
            if (hashset.Count > 0)
            {
                copyedElement = hashset;
                return "trickCopy";
            }
            return default;
        }

        private bool OnCanPasteSerializedData(string data)
        {
            this.LogFuncName();
            return data== "trickCopy";
        }

        private void OnUnserializeAndPaste(string operationName, string data)
        {
            //可能一次性创建多个节点，这里只注册一次Undo
            UndoRecord("Paste");
            using var mute = UndoMute.Enter("Copy/Paste");

            this.LogFuncName(operationName);
            if (data == "trickCopy")
            {
                foreach (var item in copyedElement)
                {
                    if (item is BehaviorTreeNodeView nodeView)
                    {
                        PasteNodeAndView(nodeView);
                    }

                }
            }
        }
    }
}
