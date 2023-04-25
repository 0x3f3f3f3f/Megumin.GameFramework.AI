using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreePortView : Port
    {
        public BehaviorTreePortView(Direction portDirection, Capacity portCapacity = Capacity.Multi)
           : this(portOrientation: Orientation.Vertical, portDirection, portCapacity, typeof(byte))
        {
            var connectorListener = new DefaultEdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<BehaviorTreeEdge>(connectorListener);
            this.AddManipulator(m_EdgeConnector);

            //m_ConnectorText.style.display = DisplayStyle.None;
            //Remove(m_ConnectorText);

            m_ConnectorBox.pickingMode = PickingMode.Position;
        }

        protected BehaviorTreePortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
        }
    }


    /// <summary>
    /// Cppy from Port.DefaultEdgeConnectorListener
    /// </summary>
    public class DefaultEdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphViewChange m_GraphViewChange;
        private List<Edge> m_EdgesToCreate;
        private List<GraphElement> m_EdgesToDelete;

        public DefaultEdgeConnectorListener()
        {
            m_EdgesToCreate = new List<Edge>();
            m_EdgesToDelete = new List<GraphElement>();

            m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
        }

        public async void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            var treeView = edge.GetFirstAncestorOfType<BehaviorTreeView>();
            if (treeView != null)
            {
                var screenPoint = position + treeView.EditorWindow.position.position;

                var parent = (edge?.output?.node as BehaviorTreeNodeView)?.Node as BTParentNode;
                var child = (edge?.input?.node as BehaviorTreeNodeView)?.Node;

                var (type, pos) = await treeView.SelectCreateNodeType(screenPoint, edge);

                var newNode = treeView.AddNewNode(type, pos);
                treeView.IncrementChangeVersion("OnDropOutsidePort");

                if (parent != null)
                {
                    if (parent is OneChildNode oneChildNode && oneChildNode.Child0 != null)
                    {
                        treeView.Tree.Disconnect(oneChildNode, oneChildNode.Child0);
                    }
                    treeView.Tree.Connect(parent, newNode);
                }

                if (child != null && newNode is BTParentNode newParent)
                {
                    if (child.TryGetFirstParent(out var oldParent))
                    {
                        treeView.Tree.Disconnect(oldParent, child);
                    }
                    treeView.Tree.Connect(newParent, child);
                }

                treeView.ReloadView();
            }
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            m_EdgesToCreate.Clear();
            m_EdgesToCreate.Add(edge);

            // We can't just add these edges to delete to the m_GraphViewChange
            // because we want the proper deletion code in GraphView to also
            // be called. Of course, that code (in DeleteElements) also
            // sends a GraphViewChange.
            m_EdgesToDelete.Clear();
            if (edge.input.capacity == Capacity.Single)
                foreach (Edge edgeToDelete in edge.input.connections)
                    if (edgeToDelete != edge)
                        m_EdgesToDelete.Add(edgeToDelete);
            if (edge.output.capacity == Capacity.Single)
                foreach (Edge edgeToDelete in edge.output.connections)
                    if (edgeToDelete != edge)
                        m_EdgesToDelete.Add(edgeToDelete);
            if (m_EdgesToDelete.Count > 0)
                graphView.DeleteElements(m_EdgesToDelete);

            var edgesToCreate = m_EdgesToCreate;
            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
            }

            foreach (Edge e in edgesToCreate)
            {
                graphView.AddElement(e);
                edge.input.Connect(e);
                edge.output.Connect(e);
            }
        }
    }
}
