using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Megumin.GameFramework.AI.BehaviorTree.Editor
{
    public class BlackboardVariableView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BlackboardVariableView, UxmlTraits> { }

        public BlackboardField BlackboardField { get; private set; }
        public TextField m_TextField { get; }
        public VisualElement Body { get; private set; }
        public BlackboardRow BlackboardRow { get; private set; }
        public BehaviorTreeBlackboardView Blackboard { get; internal set; }

        public BlackboardVariableView()
        {
            BlackboardField = new BlackboardField() { text = "Variable", typeText = "string" };

            {
                //Copy form Unity C# reference source
                m_TextField = BlackboardField.Q<TextField>("textField");
                Assert.IsTrue(m_TextField != null);

                m_TextField.style.display = DisplayStyle.None;

                var textinput = m_TextField.Q(TextField.textInputUssName);
                //TrickleDown.NoTrickleDown 保证我们的回调先执行。
                textinput.RegisterCallback<FocusOutEvent>(e => { OnEditTextFinished(); }, TrickleDown.NoTrickleDown);
            }
            





            Body = new VisualElement() { name = "body" };
            BlackboardRow = new BlackboardRow(BlackboardField, Body);




            this.Add(BlackboardRow);
        }

        private void OnEditTextFinished()
        {
            var table = Blackboard?.LookupTable;
            if (table != null)
            {
                string name = table.ValidName(m_TextField.text);
                if (name != m_TextField.text)
                {
                    m_TextField.SetValueWithoutNotify(name);
                }
            }

            var newName = m_TextField.text;
            if(Variable is TestVariable test)
            {
                test.Name = newName;
                BlackboardField.text = newName;
            }
        }

        public IVariable Variable { get; private set; }
        public void SetVariable(IVariable instance)
        {
            Variable = instance;
            BlackboardField.text = instance.Name;
            Body.Clear();
            var labelView = new Label() { text = "TestValue" };
            Body.Add(labelView);
        }
    }
}
