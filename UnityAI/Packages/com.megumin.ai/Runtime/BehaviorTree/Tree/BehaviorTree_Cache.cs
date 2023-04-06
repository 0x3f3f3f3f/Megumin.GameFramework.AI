﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public partial class BehaviorTree
    {
        protected int version = 0;

        protected int nodeIndexVersion = -1;
        public void UpdateNodeIndexDepth()
        {
            if (nodeIndexVersion == version)
            {
                return;
            }

            foreach (var item in AllNodes)
            {
                item.Index = -1;
                item.Depth = -1;
            }

            var index = 0;
            void SetNodeIndex(BTNode node, int depth = 0)
            {
                if (node == null)
                {
                    return;
                }

                node.Index = index;
                node.Depth = depth;

                index++;

                if (node is BTParentNode parentNode)
                {
                    var nextDepth = depth + 1;
                    foreach (var child in parentNode.children)
                    {
                        SetNodeIndex(child, nextDepth);
                    }
                }
            }

            SetNodeIndex(StartNode);
            nodeIndexVersion = version;
        }
    }
}


