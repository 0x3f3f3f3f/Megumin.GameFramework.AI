using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Selector : CompositeNode
    {
        protected override Status OnTick()
        {
            for (int i = current; i < children.Count; i++)
            {
                current = i;
                var child = children[current];

                switch (child.Tick())
                {
                    case Status.Succeeded:
                        return Status.Succeeded;
                    case Status.Running:
                        return Status.Running;
                }
            }

            return Status.Succeeded;
        }
    }
}
