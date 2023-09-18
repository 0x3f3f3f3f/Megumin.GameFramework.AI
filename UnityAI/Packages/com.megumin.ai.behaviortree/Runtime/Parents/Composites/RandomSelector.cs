using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Megumin.AI.BehaviorTree
{
    /// <summary>
    /// 加权随机新的执行顺序，不支持低优先级终止
    /// </summary>
    public class RandomSelector : CompositeNode, IDetailable, IDetailAlignable
    {
        public List<int> priority;
        List<BTNode> thisOrder = new List<BTNode>();
        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);
            if (priority == null)
            {

            }
            else
            {

            }

            thisOrder = Children;
        }

        public string GetDetail()
        {
            if (priority != null)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < priority.Count; i++)
                {
                    int p = priority[i];
                    sb.Append(p.ToString());
                    if (i < priority.Count - 1)
                    {
                        sb.Append(" | ");
                    }
                }
                return sb.ToString();
            }
            return null;
        }

        public TextAnchor DetailTextAlign => TextAnchor.MiddleCenter;
    }
}
