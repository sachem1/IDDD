using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Msmq
{
    public class MsmqManager
    {
        internal static string MqPath = @".\private$\mqtest";
        public void Sender()
        {
            MessageQueue mq = CreateMessageQueue(MqPath);
            mq.
        }

        /// <summary>
        /// 创建消息队列
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static MessageQueue CreateMessageQueue(string path)
        {
            MessageQueue mq = null;
            if (MessageQueue.Exists(path))
            {
                mq=new MessageQueue(path);
            }
            else
            {
                mq=MessageQueue.Create(path);
            }

            return mq;
        }
    }
}
