using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 用于反序列化失败
    /// TODO 如果父是Sequence，返回成功。否则返回失败。类似Disable Mute
    /// </summary>
    internal class MissingNode : ActionTaskNode
    {
        protected override Status OnTick()
        {
            return base.OnTick();
        }
    }
}
