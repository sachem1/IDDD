using System;

namespace Coralcode.Framework.MessageBus.Event
{
    public interface IEventData
    {
        /// <summary>
        /// 事件消息序号
        /// </summary>
        long EventDataId { get; set; }

        /// <summary>
        /// 消息信息
        /// </summary>
        IMessage MessageEntity { get; set; }

        /// <summary>
        /// Gets or sets the date and time on which the event was produced.
        /// </summary>
        /// <remarks>The format of this date/time value could be various between different
        /// systems. Apworks recommend system designer or architect uses the standard
        /// UTC date/time format.</remarks>
        DateTime Timestamp { get; set; }

        /// <summary>
        /// 消息的标识
        /// </summary>
        string MessageIdentity { get; set; }

        /// <summary>
        /// 消息对应的类型信息
        /// </summary>
        string AssemblyQualifiedEventType { get; set; }
        
    }
}