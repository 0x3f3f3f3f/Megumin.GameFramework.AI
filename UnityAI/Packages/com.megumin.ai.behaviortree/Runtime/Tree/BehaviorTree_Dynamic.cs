using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Reflection;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public partial class BehaviorTree
    {
        /// <summary>
        /// 运行时动态添加元素，通常debug时使用
        /// </summary>
        /// <param name="element"></param>
        public void DynamicAdd(object element)
        {
            RecursiveAdd(element);

            UpdateNodeIndexDepth(true);
            DynamicReBind();
        }

        /// <summary>
        /// 递归添加所有引用对象
        /// </summary>
        /// <param name="member"></param>
        protected void RecursiveAdd(object member)
        {
            var memberInfos = member.GetSerializeMembers();//.ToArray();
            foreach (var item in memberInfos)
            {
                //过滤防止无限递归
                if (item.Value == null)
                {
                    continue;
                }

                if (item.Value == member)
                {
                    continue;
                }

                if (item.Value == this)
                {
                    continue;
                }

                RecursiveAdd(item.Value);
            }

            InitAddTreeRefObj(member);
        }

        /// <summary>
        /// 运行时重新绑定，通常debug时使用
        /// </summary>
        public void DynamicReBind()
        {
            if (Application.isPlaying && Agent != null)
            {
                BindAgent(Agent);
                ParseAllBindable(Agent, true);
            }
        }
    }
}


