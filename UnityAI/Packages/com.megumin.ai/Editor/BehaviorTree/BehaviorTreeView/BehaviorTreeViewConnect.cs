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
        public void SortChild(BTParentNode parentNode)
        {
            parentNode.children.Sort((lhs, rhs) =>
            {
                var lhsView = GetElementByGuid(lhs.GUID);
                var rhsView = GetElementByGuid(rhs.GUID);
                return lhsView.layout.position.x.CompareTo(rhsView.layout.position.x);
            });
        }

        public void ConnectChild(BehaviorTreeNodeView parentNodeView, BehaviorTreeNodeView childNodeView)
        {
            this.LogFuncName();
            UndoRecord($"ConnectChild [{parentNodeView.SONode.name}] -> [{childNodeView.SONode.name}]");
            if (parentNodeView.SONode.Node is BTParentNode parentNode)
            {
                parentNode.children.Add(childNodeView.SONode.Node);

                //重新排序
                SortChild(parentNode);
            }
        }

        public void DisconnectChild(BehaviorTreeNodeView parentNodeView, BehaviorTreeNodeView childNodeView)
        {
            this.LogFuncName();
            UndoRecord($"DisconnectChild [{parentNodeView.SONode.name}] -> [{childNodeView.SONode.name}]");
            if (parentNodeView.SONode.Node is BTParentNode parentNode)
            {
                parentNode.children.RemoveAll(elem => elem.GUID == childNodeView.SONode.Node.GUID);

                //重新排序
                SortChild(parentNode);
            }
        }
    }
}
