using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using Megumin;
using System.Collections.Generic;
using Megumin.Binding;
using System;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class NodeWrapper : ScriptableObject, IRefVariableFinder, ITreeElementWrapper
    {
        [SerializeReference]
        public BTNode Node;

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

        IEnumerable<IRefable> IRefVariableFinder.GetVariableTable()
        {
            return View?.TreeView?.Tree?.Variable.Table;
        }

        public bool TryGetParam(string name, out IRefable variable)
        {
            variable = null;
            if (View?.TreeView?.Tree?.Variable?.TryGetParam(name, out variable) ?? false)
            {
                return true;
            }
            return false;
        }

        public void GetAllRefDerivedFrom(Type baseType, List<ITreeElement> refables)
        {
            var tree = View?.TreeView?.Tree;
            if (tree != null)
            {
                foreach (var node in tree.AllNodes)
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
            }

            //this.DrawButtonAfterDefaultInspector();
        }
    }
}
