using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace Megumin.AI.BehaviorTree.Editor
{


    public class DecoratorWrapper : TreeElementWrapper
    {
        [SerializeReference]
        public ITreeElement Decorator;

        public BehaviorTreeDecoratorView View { get; internal set; }

        [Editor]
        public void Test()
        {

        }

        public override BehaviorTreeView TreeView => View?.NodeView?.TreeView;
    }

    [CustomEditor(typeof(DecoratorWrapper), true, isFallback = false)]
    public class DecoratorWrapperEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            //this.DrawButtonBeforeDefaultInspector();


            var wrapper = (DecoratorWrapper)target;

            if (wrapper?.View != null)
            {
                //if (GUILayout.Button($"Open Script"))
                //{
                //    wrapper.View.OpenScript();
                //}

                var hasLogger = (wrapper.View.Decorator as ITraceable)?.TraceListener != null;

                GUILayout.BeginHorizontal();

                using (new EditorGUI.DisabledScope(hasLogger || !Application.isPlaying))
                {
                    if (GUILayout.Button($"Enable Logger"))
                    {
                        if (wrapper.View.Decorator is ITraceable traceable)
                        {
                            traceable.TraceListener = new UnityTraceListener();
                        }
                    }
                }

                using (new EditorGUI.DisabledScope(!hasLogger))
                {
                    if (GUILayout.Button($"Disable Logger"))
                    {
                        if (wrapper.View.Decorator is ITraceable traceable)
                        {
                            traceable.TraceListener = new UnityTraceListener();
                        }
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }

            //内部使用了EditorGUI.BeginChangeCheck();
            //用这种方法检测是否面板更改，触发UndoRecord
            if (DrawDefaultInspector())
            {
                //这里值已经改变了，再Record已经来不及了
                //Todo BUG, Undo时没办法回退ChangeVersion，造成编辑器未保存状态无法消除
                wrapper?.View?.NodeView?.TreeView?.IncrementChangeVersion($"Inspector Changed");
                wrapper?.View?.ReloadView();
            }

            //this.DrawButtonAfterDefaultInspector();
        }
    }
}
