using System;

namespace Coralcode.Framework.MessageBus.Event
{
    public abstract class EventData: IEventData
    {
         
        #region Ctor

        /// <summary>
        /// Initializes a new instance of <c>EventBus</c> class.
        /// </summary>
        public EventData()
        {
            this.Timestamp = DateTime.Now;
        }
        /// <summary>
        /// Initializes a new instace of <c>EventBus</c> class.
        /// </summary>
        /// <param name="entity">The source entity which raises the domain event.</param>
        public EventData(IMessage entity):base()
        {
            this.MessageEntity = entity;
        }
        #endregion

        #region IDomainEvent Members
        /// <summary>
        /// Gets or sets the source entity from which the domain event was generated.
        /// </summary>

        public IMessage MessageEntity
        {
            get;
            set;
        }

        public string MessageIdentity { get; set; }

        /// <summary>
        /// Gets or sets the assembly qualified type name of the event.
        /// </summary>
        public virtual string AssemblyQualifiedEventType { get; set; }
        #endregion

        #region IEventData Members
        /// <summary>
        /// Gets or sets the date and time on which the event was produced.
        /// </summary>
        /// <remarks>The format of this date/time value could be various between different
        /// systems. Apworks recommend system designer or architect uses the standard
        /// UTC date/time format.</remarks>
        public virtual DateTime Timestamp { get; set; }

        public virtual long EventDataId { get; set; }
        #endregion


    }
}