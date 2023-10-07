using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Megumin.Binding;
using Megumin.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace Megumin.AI.BehaviorTree
{
    [HelpURL("https://github.com/KumoKyaku/Megumin.AI.Samples/wiki/BehaviorTree_Document_en#component")]
    public class BehaviorTreeRunner : MonoBehaviour
    {
        //[field: SerializeField]
        public BehaviorTree BehaviourTree { [DebuggerStepThrough] get; [DebuggerStepThrough] protected set; }
        public BehaviorTreeAsset_1_1 BehaviorTreeAsset;
        public TickMode TickMode = TickMode.Update;

        public bool AutoEnable = true;
        [field: SerializeField]
        public OperationTree OnEnabled { get; set; } = OperationTree.Enable;

        [field: SerializeField]
        public OperationTree OnDisabled { get; set; } = OperationTree.Disable;
        public InitOption InitOption;
        public RunOption RunOption;

        [Space]
        public InitEvent<BehaviorTree, BehaviorTreeRunner> InitEvents;

        private void OnEnable()
        {
            if (BehaviourTree != null)
            {
                if ((OnEnabled & OperationTree.Enable) != 0
                    || (OnEnabled & OperationTree.Resume) != 0)
                {
                    DisableTree();
                }
            }

            if (AutoEnable)
            {
                EnableTree();
            }
        }

        private void OnDisable()
        {
            if (BehaviourTree != null)
            {
                if ((OnDisabled & OperationTree.Disable) != 0
                    || (OnDisabled & OperationTree.Pause) != 0)
                {
                    DisableTree();
                }
            }
        }

        /// <summary>
        /// 由于可能是异步，防止多次实例化
        /// </summary>
        bool isIniting = false;
        public async void EnableTree()
        {
            if (isIniting)
            {
                return;
            }
            isIniting = true;

            if (BehaviourTree == null && BehaviorTreeAsset)
            {
                RefFinder refFinder = null;

                if (OverrideVariables != null)
                {
                    refFinder = new RefFinder();
                    if (OverrideUnityObjectRef != null)
                    {
                        foreach (var item in OverrideUnityObjectRef)
                        {
                            if (string.IsNullOrEmpty(item?.Name))
                            {
                                continue;
                            }
                            refFinder.RefDic[item.Name] = item;
                        }
                    }

                    foreach (var item in OverrideVariables.Table)
                    {
                        if (string.IsNullOrEmpty(item?.RefName))
                        {
                            continue;
                        }
                        refFinder.RefDic[item.RefName] = item;
                    }

                    if (refFinder.RefDic.Count == 0)
                    {
                        refFinder = null;
                    }
                }

                //声明一个临时变量，方式闭包捕获gameObject，造成在非主线程访问gameObject。
                //防止 UnityException: get_gameObject can only be called from the main thread.
                var agent = gameObject;

                //实例行为树
                InitEvents?.BeforeInstantiate?.Invoke(null, this);
                BehaviourTree = await BehaviorTreeAsset.InstantiateAsync(InitOption, refFinder);
                InitEvents?.AfterInstantiate?.Invoke(BehaviourTree, this);

                BehaviourTree.RunOption = RunOption;
                BehaviourTree.InstanceName = gameObject.name;

                //绑定代理对象
                if (InitOption.BeforeBindAgentDelayFrame.Enabled)
                {
                    int dCount = InitOption.BeforeBindAgentDelayFrame.Value;
                    if (dCount > 0)
                    {
                        await WaitFrames(dCount);
                    }
                }

                InitEvents?.BeforeBindAgent?.Invoke(BehaviourTree, this);
                BehaviourTree.BindAgent(agent);
                InitEvents?.AfterBindAgent?.Invoke(BehaviourTree, this);



                //解析绑定变量
                if (InitOption.BeforeParseBindingDelayFrame.Enabled)
                {
                    int dCount = InitOption.BeforeParseBindingDelayFrame.Value;
                    if (dCount > 0)
                    {
                        await WaitFrames(dCount);
                    }
                }

                InitEvents?.BeforeParseBinding?.Invoke(BehaviourTree, this);
                OverrideVariables?.ParseBinding(agent, true);
                BehaviourTree.ParseAllBindable(agent);
                InitEvents?.AfterParseBinding?.Invoke(BehaviourTree, this);

                var delay = 0;
                if (InitOption.DelayMinFrame.Enabled)
                {
                    delay = InitOption.DelayMinFrame.Value;
                }

                if (InitOption.DelayRandomFrame.Enabled)
                {
                    //下限和上限都包括在内。
                    delay = UnityEngine.Random.Range(delay, InitOption.DelayRandomFrame.Value);
                }

                if (delay > 0)
                {
                    await WaitFrames(delay);
                }
            }

            if (BehaviourTree != null)
            {
                //添加到Manager
                InitEvents?.BeforeAddToManager?.Invoke(BehaviourTree, this);
                BehaviorTreeManager.Instance.AddTree(BehaviourTree, TickMode);
                BehaviourTree.IsRunning = true;
                InitEvents?.AfterAddToManager?.Invoke(BehaviourTree, this);
            }

            isIniting = false;
        }

        public void DisableTree()
        {
            if (BehaviourTree != null)
            {
                //从Manager中移除
                InitEvents?.BeforeRemoveFromManager?.Invoke(BehaviourTree, this);
                BehaviorTreeManager.Instance.RemoveTree(BehaviourTree);
                BehaviourTree.IsRunning = false;
                InitEvents?.AfterRemoveFromManager?.Invoke(BehaviourTree, this);
            }
        }

        public void ReStart()
        {
            BehaviourTree.ReStart();
        }

        public void ReBindAgent()
        {
            BehaviourTree.BindAgent(gameObject);
        }

        public void ReParseBinding()
        {
            OverrideVariables?.ParseBinding(gameObject, true);
            BehaviourTree?.ParseAllBindable(gameObject, true);
        }

        public void LogVariables()
        {
            if (OverrideVariables != null)
            {
                foreach (var item in OverrideVariables.Table)
                {
                    if (item is IBindingParseable parseable)
                    {
                        parseable.DebugParseResult();
                    }
                    else
                    {
                        Debug.Log(item);
                    }
                }
            }

            if (BehaviourTree != null)
            {
                foreach (var item in BehaviourTree.Variable.Table)
                {
                    if (item is IBindingParseable parseable)
                    {
                        parseable.DebugParseResult();
                    }
                    else
                    {
                        Debug.Log(item);
                    }
                }
            }
        }

        public List<UnityObjectData> OverrideUnityObjectRef = new();
        [FormerlySerializedAs("Override")]
        public VariableTable OverrideVariables = new();

        private void OnValidate()
        {
            if (BehaviourTree?.IsRunning == true)
            {
                //调试时tickmode改变
                EnableTree();
            }
        }

        public Task WaitFrames(int framesToWait)
        {
            TaskCompletionSource<int> source = new();
            StartCoroutine(WaitFrames(framesToWait, source));
            return source.Task;
        }

        public IEnumerator WaitFrames(int framesToWait, TaskCompletionSource<int> source)
        {
            int frameCount = 0;
            while (frameCount < framesToWait)
            {
                yield return new WaitForEndOfFrame();
                frameCount++;
            }

            source?.TrySetResult(frameCount);
        }
    }
}


