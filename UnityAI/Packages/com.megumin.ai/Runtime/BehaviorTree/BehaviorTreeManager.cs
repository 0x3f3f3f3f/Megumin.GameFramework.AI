using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [DefaultExecutionOrder(-99)]
    public partial class BehaviorTreeManager : MonoBehaviour
    {
        private static BehaviorTreeManager instance;
        const string ManagerName = "[BehaviorTreeManager]";
        public static BehaviorTreeManager Instance
        {
            get
            {
                if (!instance && !IsApplicationQuiting && Application.isPlaying)
                {
                    instance = GameObject.Find(ManagerName)?.GetComponent<BehaviorTreeManager>();
                    if (!instance)
                    {
                        instance = new GameObject(ManagerName).AddComponent<BehaviorTreeManager>();
                    }
                }
                return instance;
            }
        }

        public static bool IsApplicationQuiting = false;

        protected void Awake()
        {
            if (instance && instance != this)
            {
                //被错误创建
                Debug.LogError("BehaviorTreeManager 已经存在单例，这个实例被自动销毁。");
                if (name == ManagerName)
                {
                    DestroyImmediate(gameObject);
                }
                else
                {
                    DestroyImmediate(this);
                }

                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            IsApplicationQuiting = true;
            TreeDebugger?.StopDebug();
        }
    }

    public partial class BehaviorTreeManager
    {
        ///https://theburningmonk.com/2011/03/hashset-vs-list-vs-dictionary/
        ///遍历操作远多于添加删除操作。使用list而不是hashset。
        /// <summary>
        /// 不应该保存Runner，应该直接保存Tree，可能用户会实现自己的Runner，不要将Runner与Manager耦合
        /// </summary>
        public List<BehaviorTree> AllTree = new();
        List<BehaviorTree> UpdateTree = new();
        List<BehaviorTree> FixedUpdateTree = new();
        List<BehaviorTree> LateUpdateTree = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tree"></param>
        public void AddTree(BehaviorTree tree, TickMode tickMode)
        {
            if (tree == null)
            {
                return;
            }

            if (AllTree.Contains(tree) == false)
            {
                AllTree.Add(tree);
            }


            if ((tickMode & TickMode.Update) != 0)
            {
                if (UpdateTree.Contains(tree) == false)
                {
                    UpdateTree.Add(tree);
                }
            }
            else
            {
                UpdateTree.Remove(tree);
            }

            if ((tickMode & TickMode.FixedUpdate) != 0)
            {
                if (FixedUpdateTree.Contains(tree) == false)
                {
                    FixedUpdateTree.Add(tree);
                }
            }
            else
            {
                FixedUpdateTree.Remove(tree);
            }

            if ((tickMode & TickMode.LateUpdate) != 0)
            {
                if (LateUpdateTree.Contains(tree) == false)
                {
                    LateUpdateTree.Add(tree);
                }
            }
            else
            {
                LateUpdateTree.Remove(tree);
            }

            TreeDebugger?.AddDebugInstanceTree(tree);
        }

        List<BehaviorTree> needRemoveTree = new();
        public void RemoveTree(BehaviorTree tree)
        {
            needRemoveTree.Add(tree);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void TickTree(BehaviorTree tree)
        {
            Profiler.BeginSample(tree.InstanceName);
            Profiler.BeginSample("TickTree");
            tree.Tick();
            Profiler.EndSample();
            Profiler.EndSample();
        }

        private void Update()
        {
            Profiler.BeginSample("UpdateTick");

            foreach (var item in UpdateTree)
            {
                TickTree(item);
            }

            Profiler.EndSample();

            RemoveImmediate();
            TreeDebugger?.PostTick();
        }

        private void FixedUpdate()
        {
            Profiler.BeginSample("FixedUpdateTick");

            foreach (var item in FixedUpdateTree)
            {
                TickTree(item);
            }

            Profiler.EndSample();

            RemoveImmediate();
            TreeDebugger?.PostTick();
        }

        private void LateUpdate()
        {
            Profiler.BeginSample("LateUpdateTick");

            foreach (var item in LateUpdateTree)
            {
                TickTree(item);
            }

            Profiler.EndSample();

            RemoveImmediate();
            TreeDebugger?.PostTick();
        }

        void RemoveImmediate()
        {
            foreach (var item in needRemoveTree)
            {
                if (item != null)
                {
                    AllTree.Remove(item);
                    UpdateTree.Remove(item);
                    FixedUpdateTree.Remove(item);
                    LateUpdateTree.Remove(item);
                }
            }
            needRemoveTree.Clear();
        }
    }

    public partial class BehaviorTreeManager
    {
        public static ITreeDebugger TreeDebugger { get; set; }
    }

    public interface ITreeDebugger
    {
        void AddDebugInstanceTree(BehaviorTree tree);
        void PostTick();
        void StopDebug();
    }
}
