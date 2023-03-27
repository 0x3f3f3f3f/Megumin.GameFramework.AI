using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Serialization;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 抽象资产接口，应对不同版本和资产类型
    /// </summary>
    public interface IBehaviorTreeAsset
    {
        string name { get; set; }

        /// <summary>
        /// Json文件可能需要Wrapper
        /// </summary>
        /// <returns></returns>
        UnityEngine.Object AssetObject { get; }
        string StartNodeGUID { get; set; }

        BehaviorTree Instantiate(bool instanceMeta = true, IRefFinder overrideRef = null);
        bool SaveTree(BehaviorTree tree);
    }
}




