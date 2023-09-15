using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Megumin.Binding;
using UnityEngine;

namespace Megumin.AI
{
    /// <summary>
    /// TODO, 宏支持
    /// </summary>
    [Serializable]
    public class LogInfo
    {
        public RefVar_String Text = new() { value = "Hello world!    Hello: $(name)! [$(position)]" };

        [Space]
        [Tooltip("Use Ref_GameObject or Ref_Transform macro replace Text.")]
        public bool MacroRefObj = false;
        public RefVar_Transform Ref_Transform;
        public RefVar_GameObject Ref_GameObject;


        readonly StringBuilder StringBuilder = new StringBuilder();
        public StringBuilder Rebuid()
        {
            StringBuilder.Clear();
            StringBuilder.Append(Text);

            var macro = MacroRefObj;

            if (MacroRefObj)
            {
                UnityEngine.Object uo = null;
                if (Ref_GameObject?.Value)
                {
                    uo = Ref_GameObject.Value;
                }
                else if (Ref_Transform?.Value)
                {
                    uo = Ref_Transform.Value;
                }

                if (uo)
                {
                    StringBuilder.MacroUnityObject(uo);
                }
            }

            return StringBuilder;
        }
    }
}
