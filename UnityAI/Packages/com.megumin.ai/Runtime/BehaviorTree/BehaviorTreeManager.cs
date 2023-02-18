using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.GameFramework.AI.BehaviorTree
{
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
                //�����󴴽�
                Debug.LogError("BehaviorTreeManager �Ѿ����ڵ��������ʵ�����Զ����١�");
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

        // Update is called once per frame
        void Update()
        {

        }

        private void OnApplicationQuit()
        {
            IsApplicationQuiting = true;    
        }
    }
}
