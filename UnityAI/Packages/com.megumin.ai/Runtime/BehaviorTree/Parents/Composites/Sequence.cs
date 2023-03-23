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
    public class Sequence : CompositeNode
    {
        protected override Status OnTick()
        {
            for (int i = current; i < children.Count; i++)
            {
                current = i;
                var child = children[current];

                switch (child.Tick(this))
                {
                    case Status.Failed:
                        return Status.Failed;
                    case Status.Running:
                        return Status.Running;
                }
            }

            return Status.Succeeded;
        }




        //async ValueTask<bool> Extest()
        //{
        //    foreach (var item in children)
        //    {
        //        var res = await item.Extest();
        //        if (res == false)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }
}
