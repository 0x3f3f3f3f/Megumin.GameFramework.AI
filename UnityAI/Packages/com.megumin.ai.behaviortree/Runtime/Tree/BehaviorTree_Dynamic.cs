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

            InitAddObjTreeElement(member);
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

        /// <summary>
        /// 动态替换一个节点,保持父子节点，装饰器不变。
        /// </summary>
        /// <param name="oldNode"></param>
        /// <param name="newNode"></param>
        public void DynamicReplace(BTNode oldNode, BTNode newNode)
        {
            RemoveNode(oldNode);

            //继承meta guid
            newNode.Meta = oldNode.Meta;
            newNode.Index = oldNode.Index;
            newNode.Depth = oldNode.Depth;
            newNode.GUID = oldNode.GUID;

            //继承装饰器
            foreach (var decorator in oldNode.Decorators)
            {
                newNode.AddDecorator(decorator);
            }

            DynamicAdd(newNode);

            //继承子节点
            if (oldNode is BTParentNode parentNode && newNode is BTParentNode newParent)
            {
                foreach (var item in parentNode.Children)
                {
                    Connect(newParent, item);
                }
            }

            //继承父节点
            if (TryGetFirstParent(oldNode, out var parent))
            {
                var index = IndexOfParentNode(parent, oldNode);
                Disconnect(parent, oldNode);
                Connect(parent, newNode, index);
            }

            //通过ReCacheDic 更新开始节点
            ReCacheDic();


            version++;
            UpdateNodeIndexDepth(true);
            DynamicReBind();
        }
    }
}


