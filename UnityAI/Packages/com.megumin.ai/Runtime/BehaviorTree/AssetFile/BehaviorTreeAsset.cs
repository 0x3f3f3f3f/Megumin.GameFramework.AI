using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Reflection;
using Megumin.Serialization;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    /// <summary>
    /// 抽象资产接口，应对不同版本和资产类型
    /// </summary>
    public interface IBehaviorTreeAsset
    {
        string GUID { get; set; }
        string name { get; set; }
        /// <summary>
        /// Json文件可能需要Wrapper
        /// </summary>
        /// <returns></returns>
        UnityEngine.Object AssetObject { get; }
        string StartNodeGUID { get; set; }

        BehaviorTree Instantiate(InitOption initOption, IRefFinder overrideRef = null);
        bool SaveTree(BehaviorTree tree);
    }

    public static class BehaviorTreeAssetExtension
    {
        public static Task<BehaviorTree> InstantiateAsync(this IBehaviorTreeAsset treeAsset,
                                                          InitOption initOption,
                                                          IRefFinder refFinder = null)
        {
            if (treeAsset == null || initOption == null)
            {
                return Task.FromResult<BehaviorTree>(null);
            }

            if (initOption.AsyncInit)
            {
                return Task.Run(async () =>
                {
                    //先触发CacheAllTypes
                    await TypeCache.CacheAllTypesAsync().ConfigureAwait(false);
                    var tree = treeAsset.Instantiate(initOption, refFinder);
                    return tree;
                });
            }
            else
            {
                var tree = treeAsset.Instantiate(initOption, refFinder);
                return Task.FromResult(tree);
            }
        }

    }
}




