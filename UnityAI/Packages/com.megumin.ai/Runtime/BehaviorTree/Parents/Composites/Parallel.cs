using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public class Parallel : CompositeNode
    {
        List<BTNode> comp = new List<BTNode>();
        protected override void OnEnter()
        {
            comp.Clear();
        }

        protected override Status OnTick()
        {
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                if (comp.Contains(child))
                {
                    continue;
                }

                var state = child.Tick();
                if (state == Status.Failed)
                {
                    comp.Add(child);
                    AbortRunningChild();
                    return Status.Failed;
                }
                else if (state == Status.Succeeded)
                {
                    comp.Add(child);
                }
            }

            if (comp.Count == children.Count)
            {
                return Status.Succeeded;
            }

            return Status.Running;
        }

        public void AbortRunningChild()
        {
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                if (comp.Contains(child))
                {
                    continue;
                }

                child.Abort();
            }
        }

       
    }
}





