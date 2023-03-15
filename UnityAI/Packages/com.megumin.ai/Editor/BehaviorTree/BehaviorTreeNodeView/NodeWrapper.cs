using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using Megumin;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class NodeWrapper : ScriptableObject, ITreeElementWrapper
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


            var wrapper= (NodeWrapper)target;
            //内部使用了EditorGUI.BeginChangeCheck();
            //用这种方法检测是否面板更改，触发UndoRecord
            if (DrawDefaultInspector())
            {
                //这里值已经改变了，再Record已经来不及了
                //Todo BUG, Undo时没办法回退ChangeVersion，造成编辑器未保存状态无法消除
                wrapper.View.TreeView.IncrementChangeVersion($"Inspector Changed");
            }

            //this.DrawButtonAfterDefaultInspector();
        }
    }
}
