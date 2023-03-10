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
            title = "参数表";
            subTitle = "测试subTitle";

            addItemRequested += b =>
            {
                Debug.Log(b);
            };
            scrollable = true;
            SetPosition(BehaviorTreeEditor.BlackboardLayout);

            // 不能使用ListView,子元素能折叠打开，动态大小，bug很多。
            editTextRequested += OnEditTextRequested;
        }

        public override void UpdatePresenterPosition()
        {
            base.UpdatePresenterPosition();
            BehaviorTreeEditor.BlackboardLayout.value = layout;
        }

        public VariableTable LookupTable {  get; set; }   
        public void ReloadView(bool force = false)
        {
            this.Clear();
            var tree = (graphView as BehaviorTreeView)?.Tree;
            LookupTable = tree?.Variable;
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




