using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 保证同名锁同一时间只能有一个Task执行
    /// </summary>
    internal class Lock : ConditionDecorator, IPostDecorator, IPreDecorator, IConditionDecorator
    {
        BehaviorTree tree;
        public string lockName;
        public Status AfterNodeExit(Status result, BTNode bTNode)
        {
            tree.lockDic.Remove(lockName);
            return result;
        }

        public void BeforeNodeEnter(BTNode bTNode)
        {
            if (string.IsNullOrEmpty(lockName))
            {
                lockName = bTNode.InstanceID;
            }

            tree.lockDic.Add(lockName, this);
        }

        protected override bool OnCheckCondition(BTNode container)
        {
            tree.lockDic.TryGetValue(lockName, out var result);
            return result == this;
        }
    }
}
