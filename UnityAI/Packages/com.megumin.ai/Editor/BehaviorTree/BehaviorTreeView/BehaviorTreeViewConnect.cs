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
            if (parentNodeView.SONode.Node is BTParentNode parentNode)
            {
                ConnectChild(parentNode, childNodeView.SONode.Node);
                parentNodeView.ReloadView();
                childNodeView.ReloadView();
            }
        }

        public void ConnectChild(BTParentNode parentNode, BTNode childNode)
        {
            this.LogMethodName();
            UndoRecord($"ConnectChild [{parentNode.GetType().Name}] -> [{childNode.GetType()}]");

            parentNode.children.Add(childNode);
            //重新排序
            SortChild(parentNode);
        }

        public void DisconnectChild(BehaviorTreeNodeView parentNodeView, BehaviorTreeNodeView childNodeView)
        {
            if (parentNodeView.SONode.Node is BTParentNode parentNode)
            {
                DisconnectChild(parentNode, childNodeView.SONode.Node);
                parentNodeView.ReloadView();
                childNodeView.ReloadView();
            }
        }

        public void DisconnectChild(BTParentNode parentNode, BTNode childNode)
        {
            this.LogMethodName();
            UndoRecord($"DisconnectChild [{parentNode.GetType().Name}] -> [{childNode.GetType()}]");

            parentNode.children.RemoveAll(elem => elem.GUID == childNode.GUID);

            //重新排序
            SortChild(parentNode);
        }
    }
}
