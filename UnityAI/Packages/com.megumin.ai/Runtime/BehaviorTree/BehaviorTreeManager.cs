using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
    [Flags]
    public enum TickMode
    {
        None = 0,
        Update = 1 << 0,
        LateUpdate = 1 << 1,
        FixedUpdate = 1 << 2,
        Manual = 1 << 3,
    }

    [DefaultExecutionOrder(-99)]
    public partial class BehaviorTreeManager : MonoBehaviour
    {
        private static BehaviorTreeManager instance;

        public static BehaviorTreeManager Instance
        {
            get
            {
                if (!instance && !IsApplicationQuiting && Application.isPlaying)
                {
                    instance = new GameObject("BehaviorTreeManager").AddComponent<BehaviorTreeManager>();
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
                if (name == nameof(BehaviorTreeManager))
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

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnApplicationQuit()
        {
            IsApplicationQuiting = true;
            TreeDebugger?.StopDebug();
        }


    }

    public partial class BehaviorTreeManager
    {
        public List<BehaviorTreeRunner> AllTree = new();
        List<BehaviorTreeRunner> UpdateTree = new();
        List<BehaviorTreeRunner> FixedUpdateTree = new();
        List<BehaviorTreeRunner> LateUpdateTree = new();

        public void AddTree(BehaviorTreeRunner behaviorTreeRunner)
        {
            if (AllTree.Contains(behaviorTreeRunner))
            {
                AllTree.Remove(behaviorTreeRunner);
                UpdateTree.Remove(behaviorTreeRunner);
                FixedUpdateTree.Remove(behaviorTreeRunner);
                LateUpdateTree.Remove(behaviorTreeRunner);
            }

            AllTree.Add(behaviorTreeRunner);
            if (behaviorTreeRunner.TickMode.HasFlag(TickMode.Update))
            {
                UpdateTree.Add(behaviorTreeRunner);
                UpdateTree.Sort();
            }

            if (behaviorTreeRunner.TickMode.HasFlag(TickMode.FixedUpdate))
            {
                FixedUpdateTree.Add(behaviorTreeRunner);
                FixedUpdateTree.Sort();
            }

            if (behaviorTreeRunner.TickMode.HasFlag(TickMode.LateUpdate))
            {
                LateUpdateTree.Add(behaviorTreeRunner);
                LateUpdateTree.Sort();
            }

            TreeDebugger?.AddTreeRunner(behaviorTreeRunner);
        }

        void Update()
        {
            foreach (var item in UpdateTree)
            {
                if (item && item.enabled)
                {
                    item.TickTree();
                }
            }

            TreeDebugger?.PostTick();
        }
    }

    public partial class BehaviorTreeManager
    {
        public static ITreeDebugger TreeDebugger { get; set; }
    }

    public interface ITreeDebugger
    {
        void AddTreeRunner(BehaviorTreeRunner behaviorTreeRunner);
        void PostTick();
        void StopDebug();
    }
}
