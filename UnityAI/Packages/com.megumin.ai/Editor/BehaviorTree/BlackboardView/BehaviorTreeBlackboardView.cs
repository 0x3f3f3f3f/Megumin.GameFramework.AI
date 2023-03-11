using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BehaviorTreeBlackboardView : Blackboard
    {
        public BehaviorTreeBlackboardView(GraphView associatedGraphView = null)
            : base(associatedGraphView)
        {
            TreeView = associatedGraphView as BehaviorTreeView;
            LookupTable = TreeView?.Tree?.Variable;

            title = "参数表";
            subTitle = "测试subTitle";

            addItemRequested += OnAddClicked;

            scrollable = true;
            SetPosition(BehaviorTreeEditor.BlackboardLayout);

            // 不能使用ListView,子元素能折叠打开，动态大小，bug很多。
            editTextRequested += OnEditTextRequested;
        }

        public BehaviorTreeView TreeView { get; set; }

        private void OnAddClicked(Blackboard blackboard)
        {
            var parameterType = new GenericMenu();

            parameterType.AddItem(new GUIContent("Category"), false, () => { Debug.Log("Todo!"); });
            parameterType.AddSeparator($"");

            var list = VariableCreator.AllCreator;
            foreach (var item in list)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.IsSeparator)
                {
                    parameterType.AddSeparator(item.Name);
                }
                else
                {
                    parameterType.AddItem(new GUIContent(item.Name), false, () =>
                    {
                        AddNewVariable(item);
                    });
                }
            }

            parameterType.ShowAsContext();
        }

        public void AddNewVariable(VariableCreator creator)
        {
            if (LookupTable == null)
            {
                TreeView?.CreateTreeSOTreeIfNull();
                LookupTable = TreeView?.Tree?.Variable;
                if (LookupTable == null)
                {
                    Debug.LogError("LookupTable == null");
                    return;
                }
            }

            TreeView.UndoRecord("AddNewVariable");
            var vara = creator.Create();
            vara.Name = LookupTable.ValidName(vara.Name);
            LookupTable.Table.Add(vara);
            ReloadView();
        }

        public override void UpdatePresenterPosition()
        {
            base.UpdatePresenterPosition();
            BehaviorTreeEditor.BlackboardLayout.value = layout;
        }

        public VariableTable LookupTable { get; set; }

        public void ReloadView(bool force = false)
        {
            this.Clear();

            LookupTable = TreeView?.Tree?.Variable;
            if (LookupTable == null)
            {
            }
            else
            {
                foreach (var variable in LookupTable.Table)
                {
                    var view = new BlackboardVariableView();
                    view.Blackboard = this;
                    view.SetVariable(variable);
                    Add(view);
                }
            }
        }

        private void OnEditTextRequested(Blackboard arg1, VisualElement arg2, string arg3)
        {

        }
    }
}




