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
        public RefVar_String Text = new() { value = "Hello world!" };
        public RefVar_Transform Ref_Transform;
        public RefVar_GameObject Ref_GameObject;

        readonly StringBuilder StringBuilder = new StringBuilder();
        public StringBuilder Rebuid()
        {
            StringBuilder.Clear();
            StringBuilder.Append(Text);
            if (Ref_Transform?.Value)
            {
                StringBuilder.Append("----");
                StringBuilder.Append(Ref_Transform.Value.name);
            }

            if (Ref_GameObject?.Value)
            {
                StringBuilder.Append("----");
                StringBuilder.Append(Ref_GameObject.Value.name);
            }

            return StringBuilder;
        }
    }
}
