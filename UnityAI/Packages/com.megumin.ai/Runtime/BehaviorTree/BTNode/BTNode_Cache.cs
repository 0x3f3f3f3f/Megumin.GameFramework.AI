using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    public partial class BTNode
    {
        /// <summary>
        /// 在节点中的序号
        /// </summary>
        /// <remarks>
        /// 实例化后计算，而不是保存在meta中。
        /// 优点：
        /// 运行时更改不会影响sharedmeta。
        /// 修改行为树结构是不会造成文件大幅度变化。
        /// 缺点：
        /// 每次实例化要重新计算序号，浪费了运行时的性能。
        /// </remarks>
        public int Index { get; set; }
        public int Depth { get; set; }





        [Obsolete("test", true)]
        public async ValueTask<bool> Extest()
        {
            var state = Status.Running;
            while (state != Status.Running)
            {
                //FrontDerators();
                Enter();
                var res = await onticktest();
                //var res2 = Exit(default);
                //res2 = BackDerators(res2);
            }

            return true;
        }

        [Obsolete("test", true)]
        ValueTask<bool> onticktest()
        {
            return new ValueTask<bool>(true);
        }
    }
}




