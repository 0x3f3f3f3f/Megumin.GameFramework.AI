using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //BlackboardSection child = new BlackboardSection() { title = "测试2222" };
            //Add(child);
            //BlackboardSection child1 = new BlackboardSection() { title = "测试33333" };
            //Add(child1);

            var field = new BlackboardField() { text = "参数1", typeText = "string" };

            var labelView = new Label() { text = "TestValue" };
            var labelView2 = new Label() { text = "TestValue2" };
            var row = new BlackboardRow(labelView2, labelView);
            Add(row);


            addItemRequested += b =>
            {
                Debug.Log(b);
            };
        }
    }
}
