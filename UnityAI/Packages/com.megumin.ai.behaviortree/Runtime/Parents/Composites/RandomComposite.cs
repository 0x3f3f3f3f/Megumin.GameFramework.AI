using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public abstract class RandomComposite : CompositeNode, IDetailable, IDetailAlignable
    {
        [Space]
        public List<int> Priority;

        [ReadOnlyInInspector]
        public List<int> CurrentOrder = new List<int>();

        protected override void OnEnter(object options = null)
        {
            base.OnEnter(options);

            if (Priority == null)
            {
                Priority = new List<int>();
            }

            if (Priority.Count < Children.Count)
            {
                //Ȩ�ظ��������ӽڵ����������1
                for (int i = Priority.Count; i < Children.Count; i++)
                {
                    Priority[i] = 1;
                }
            }

            //Ȩ�ظ��������ӽڵ������������
            GetRandomIndex(Priority, Children.Count, CurrentOrder);
        }

        protected List<int> randomList = new List<int>();

        /// <summary>
        /// ����Ȩ����������,ÿ��Ԫ�ؽ��ɱ������һ��
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void GetRandomIndex(List<int> list, int count, List<int> order)
        {
            randomList.Clear();
            order.Clear();

            for (int i = 0; i < count; i++)
            {
                randomList.Add(list[i]);
            }

            var total = randomList.Sum();

            while (true)
            {
                if (total <= 0)
                {
                    break;
                }

                int value = UnityEngine.Random.Range(0, total);
                for (int i = 0; i < count; i++)
                {
                    //��õ�ǰȨ��
                    var currentPriority = randomList[i];

                    value -= currentPriority;
                    if (value < 0)
                    {
                        order.Add(i);

                        //��Ȩ�ؼ�ȥ��ǰȨ�أ���һ�����ʱ��������һ��
                        total -= currentPriority;

                        //���������Ԫ��Ȩ����Ϊ0����ֹ�ٴα������
                        randomList[i] = 0;
                        break;
                    }
                }
            }
        }

        public string GetDetail()
        {
            if (Priority != null)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Priority.Count; i++)
                {
                    int p = Priority[i];
                    sb.Append(p.ToString());
                    if (i < Priority.Count - 1)
                    {
                        sb.Append(" | ");
                    }
                }
                return sb.ToString();
            }
            return null;
        }

        public TextAnchor DetailTextAlign => TextAnchor.MiddleCenter;
    }
}
