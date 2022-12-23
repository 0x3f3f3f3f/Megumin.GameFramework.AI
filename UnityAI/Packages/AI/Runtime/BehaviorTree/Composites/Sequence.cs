using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 为什么是Sequence不是Sequencer，因为Sequence字符串长度和Selector一样。
    /// </summary>
    public class Sequence : CompositeTaskNode
    {
        protected override Status OnTick()
        {
            for (int i = current; i < children.Count; i++)
            {
                current = i;
                var child = children[current];

                if (child.State != Status.Running)
                {
                    //已经运行的节点不在检查
                    var enterType = child.CanEnter();
                    if (enterType == EnterType.False)
                    {
                        return Status.Failed;
                    }

                    if (enterType == EnterType.Ignore)
                    {
                        continue;
                    }
                }

                switch (child.Tick())
                {
                    case Status.Failed:
                        return Status.Failed;
                    case Status.Running:
                        return Status.Running;
                }
            }

            return Status.Succeeded;
        }
    }
}
