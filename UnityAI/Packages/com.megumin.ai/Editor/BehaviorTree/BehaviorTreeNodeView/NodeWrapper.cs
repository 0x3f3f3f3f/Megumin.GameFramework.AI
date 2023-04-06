using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using Megumin;
using System.Collections.Generic;
using Megumin.Binding;
using System;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public abstract class TreeElementWrapper : ScriptableObject, IRefVariableFinder, ITreeElementWrapper
    {
        public BehaviorTree Tree => TreeView?.Tree;
        public abstract BehaviorTreeView TreeView { get; }

        public void GetAllElementsDerivedFrom(Type baseType, List<ITreeElement> refables)
        {
            if (Tree != null)
            {
                foreach (var node in Tree.AllNodes)
                {
                    if (baseType.IsAssignableFrom(node.GetType()))
                    {
                        refables.Add(node);
                    }

                    foreach (var d in node.Decorators)
                    {
                        if (baseType.IsAssignableFrom(d.GetType()))
                        {
                            refables.Add(d);
                        }
                    }
                }
            }
        }

        IEnumerable<IRefable> IRefVariableFinder.GetVariableTable()
        {
            return Tree?.Variable.Table;
        }

        public bool TryGetParam(string name, out IRefable variable)
        {
            variable = null;
            if (Tree?.Variable?.TryGetParam(name, out variable) ?? false)
            {
                return true;
            }
            return false;
        }

        public void Export(IRefable currentValue)
        {
            TreeView.Blackboard.AddNewVariable(currentValue);
        }
    }

    public class NodeWrapper : TreeElementWrapper
    {
        [SerializeReference]
        public BTNode Node;

        public override BehaviorTreeView TreeView => View?.TreeView;

        public BehaviorTreeNodeView View { get; internal set; }

        [Editor]
        public void Test()
        {
            if (View.outputContainer.ClassListContains("unDisplay"))
            {
                View.outputContainer.RemoveFromClassList("unDisplay");
            }
            else
            {
                View.outputContainer.AddToClassList("unDisplay");
            }
        }

        public VariableTable GetVariableTable()
        {
            return View?.TreeView?.Tree?.Variable;
        }


        //TODO
        //private void OnValidate()
        //{
        //    this.LogMethodName();
        //    //Undo 也会触发这个函数
        //    View.TreeView.UndoRecord($"Inspector Changed");
        //}
    }

    [CustomEditor(typeof(NodeWrapper), true, isFallback = false)]
    public class NodeWrapperEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            //this.DrawButtonBeforeDefaultInspector();


            var wrapper = (NodeWrapper)target;
            //内部使用了EditorGUI.BeginChangeCheck();
            //用这种方法检测是否面板更改，触发UndoRecord
            if (DrawDefaultInspector())
            {
                //这里值已经改变了，再Record已经来不及了
                //Todo BUG, Undo时没办法回退ChangeVersion，造成编辑器未保存状态无法消除
                //TODO, 打开关闭foldout也会触发，需要过滤掉。
                wrapper.View.TreeView.IncrementChangeVersion($"Inspector Changed");
                wrapper.View.TreeView.RefreshAllNodeEnabled();
                wrapper?.View?.ReloadView();
            }

            //this.DrawButtonAfterDefaultInspector();
        }
    }
}
