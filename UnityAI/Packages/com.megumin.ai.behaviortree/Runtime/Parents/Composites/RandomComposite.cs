using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Megumin.AI.BehaviorTree
{
    public abstract class RandomComposite : CompositeNode, IDetailable, IDetailAlignable, IDataValidable
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
                //权重个数少于子节点个数，补充1
                for (int i = Priority.Count; i < Children.Count; i++)
                {
                    Priority[i] = 1;
                }
            }

            //权重个数多于子节点个数，不考虑
            GetRandomIndex(Priority, Children.Count, CurrentOrder);
        }

        protected List<int> randomList = new List<int>();

        /// <summary>
        /// 根据权重重新排序,每个元素仅可被随机到一次
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
                    //获得当前权重
                    var currentPriority = randomList[i];

                    value -= currentPriority;
                    if (value < 0)
                    {
                        order.Add(i);

                        //总权重减去当前权重，下一次随机时不包含这一项
                        total -= currentPriority;

                        //将随机到的元素权重置为0，防止再次被随机到
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

        public (int Result, string ToolTip) Valid()
        {
            if (Priority?.Count != Children.Count)
            {
                return (-1, "Priority.Count != Children.Count");
            }

            if (Priority?.Sum() == 0)
            {
                return (-2, "Total priority is 0");
            }

            return (0, null);
        }
    }


    public abstract class RandomComposite<T> : CompositeNode<T>, IDetailable, IDetailAlignable
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
                //权重个数少于子节点个数，补充1
                for (int i = Priority.Count; i < Children.Count; i++)
                {
                    Priority[i] = 1;
                }
            }

            //权重个数多于子节点个数，不考虑
            GetRandomIndex(Priority, Children.Count, CurrentOrder);
        }

        protected List<int> randomList = new List<int>();

        /// <summary>
        /// 根据权重重新排序,每个元素仅可被随机到一次
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
                    //获得当前权重
                    var currentPriority = randomList[i];

                    value -= currentPriority;
                    if (value < 0)
                    {
                        order.Add(i);

                        //总权重减去当前权重，下一次随机时不包含这一项
                        total -= currentPriority;

                        //将随机到的元素权重置为0，防止再次被随机到
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
