﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
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
