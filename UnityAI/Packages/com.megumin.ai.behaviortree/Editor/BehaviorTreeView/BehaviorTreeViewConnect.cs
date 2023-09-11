using System;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Megumin.AI.BehaviorTree.Editor
{
    public partial class BehaviorTreeView
    {
        /// <summary>
        /// 更新节点序号
        /// </summary>
        public void UpdateNodeIndex()
        {
            Tree?.UpdateNodeIndexDepth();
            foreach (var item in graphElements)
            {
                if (item is BehaviorTreeNodeView nodeView)
                {
                    nodeView.RefreshNodeIndex();
                }
            }
        }

        public void SortChild(BTParentNode parentNode)
        {
            var hasChanged = false;
            parentNode.Children.Sort((lhs, rhs) =>
            {
                var lhsView = GetElementByGuid(lhs.GUID);
                var rhsView = GetElementByGuid(rhs.GUID);
                var result = lhsView.layout.position.x.CompareTo(rhsView.layout.position.x);
                if (result > 0)
                {
                    hasChanged = true;
                }
                return result;
            });

            if (hasChanged)
            {
                //Debug.LogError($"hasChanged {hasChanged}");
                Tree?.OnChildIndexChanged(parentNode);
            }
        }

        public void ConnectChild(BehaviorTreeNodeView parentNodeView, BehaviorTreeNodeView childNodeView)
        {
            if (parentNodeView.SONode.Node is BTParentNode parentNode)
            {
                ConnectChild(parentNode, childNodeView.SONode.Node);
                ReloadAllNodeView();
            }
        }

        public void ConnectChild(BTParentNode parentNode, BTNode childNode)
        {
            this.LogMethodName();
            UndoRecord($"ConnectChild [{parentNode.GetType().Name}] -> [{childNode.GetType()}]");

            Tree?.Connect(parentNode, childNode);
            //重新排序
            SortChild(parentNode);
            UpdateNodeIndex();
        }

        public void DisconnectChild(BehaviorTreeNodeView parentNodeView, BehaviorTreeNodeView childNodeView)
        {
            if (parentNodeView.SONode.Node is BTParentNode parentNode)
            {
                DisconnectChild(parentNode, childNodeView.SONode.Node);
                ReloadAllNodeView();
            }
        }

        public void DisconnectChild(BTParentNode parentNode, BTNode childNode)
        {
            this.LogMethodName();
            UndoRecord($"DisconnectChild [{parentNode.GetType().Name}] -> [{childNode.GetType()}]");

            Tree?.Disconnect(parentNode, childNode);
            //重新排序
            SortChild(parentNode);
            UpdateNodeIndex();
        }

        public Task<(Type Type, Vector2 GraphPosition)> SelectCreateNodeType(Vector2 position, Edge edge = null)
        {
            TaskCompletionSource<(Type Type, Vector2 GraphPosition)> taskCompletion = new();
            createNodeMenu.NextTaskSource = taskCompletion;
            createNodeMenu.NextEdge = edge;
            SearchWindow.Open(new SearchWindowContext(position), createNodeMenu);
            return taskCompletion.Task;
        }
    }
}
