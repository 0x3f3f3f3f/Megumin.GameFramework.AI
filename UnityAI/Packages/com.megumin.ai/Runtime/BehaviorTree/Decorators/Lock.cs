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
    internal class Lock : IPostDecirator, IPreDecirator, IConditionable
    {
        BehaviorTree tree;
        public string lockName;
        public Status AfterNodeExit(Status result, BTNode bTNode)
        {
            tree.locDic.Remove(lockName);
            return result;
        }

        public void BeforeNodeEnter(BTNode bTNode)
        {
            tree.locDic.Add(lockName, this);
        }

        public bool Cal()
        {
            tree.locDic.TryGetValue(lockName, out var result);
            Result = result == this;
            return Result;
        }

        public bool Result { get; private set; }
    }
}
