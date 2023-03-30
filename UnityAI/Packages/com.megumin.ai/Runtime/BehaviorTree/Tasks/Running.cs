﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Category("Debug")]
    public class Running : ActionTaskNode
    {
        protected override Status OnTick(BTNode from)
        {
            return Status.Running;
        }
    }
}


