using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megumin.AI.BehaviorTree
{
    public partial class BehaviorTree
    {
        /// <summary>
        /// 事件生命周期为一个tick，事件可以被多次响应
        /// </summary>
        Dictionary<string, EventData> eventCache = new();

        public class EventData
        {
            public BTNode SendNode { get; set; }
            public int SendTick { get; set; }
        }

        List<string> removeKey = new();
        protected void RemoveLifeEndEventData()
        {
            if (eventCache.Count > 0)
            {
                removeKey.Clear();
                foreach (var item in eventCache)
                {
                    if (item.Value.SendTick + 1 < TotalTickCount)
                    {
                        //大于1个tick的事件数据被删除
                        removeKey.Add(item.Key);
                    }
                }

                foreach (var item in removeKey)
                {
                    eventCache.Remove(item);
                }
                removeKey.Clear();
            }
        }

        public void SendEvent(string eventName, BTNode sendNode = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            EventData eventData = new();
            eventData.SendTick = TotalTickCount;
            eventData.SendNode = sendNode;
            eventCache[eventName] = eventData;
        }

        public bool TryGetEvent(string eventName, BTNode checkNode, out object eventData)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                eventData = null;
                return false;
            }

            if (eventCache.TryGetValue(eventName, out var evtData))
            {
                eventData = evtData;
                //事件的生命周期是发送节点 到下次一次tick 发送节点，总共一个Tick
                if (evtData.SendNode == null)
                {
                    //当前Tick访问都认为是触发事件
                    return TotalTickCount == evtData.SendTick;
                }
                else
                {
                    if (TotalTickCount > evtData.SendTick)
                    {
                        //下一次Tick 除了 发送节点之前的节点和发送节点本身 认为可以收到事件
                        return IsBehind(evtData.SendNode, checkNode) == false;
                    }
                    else
                    {
                        //当前Tick 发送节点之后执行的节点认为可以收到事件
                        return IsBehind(evtData.SendNode, checkNode);
                    }
                }

            }
            eventData = null;
            return false;
        }

        /// <summary>
        /// 测试节点是不是在给定节点之后
        /// </summary>
        /// <param name="positionNode"></param>
        /// <param name="checkNode"></param>
        /// <returns></returns>
        public bool IsBehind(BTNode positionNode, BTNode checkNode)
        {
            if (positionNode == null)
            {
                return true;
            }

            if (checkNode == null)
            {
                return false;
            }

            if (positionNode.Tree == checkNode.Tree)
            {
                return positionNode.Index < checkNode.Index;
            }
            else
            {
                //Todo 子树。
                throw new NotImplementedException();
            }

            //return false;
        }
    }
}





