using System;

namespace Coralcode.Framework.Data.Core
{

    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract class Entity:IEntity
    {
        #region Members

        int? _requestedHashCode;

        #endregion

        #region Properties

        /// <summary>
        /// Get the persisten object identifier
        /// </summary>
        public virtual long Id { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if this entity is transient, ie, without identity at this moment
        /// </summary>
        /// <returns>True if entity is transient, else false</returns>
        public bool IsTransient()
        {
            return this.Id == IdentityGenerator.NewSequentialDomainId();
        }

        /// <summary>
        /// Generate identity for this entity
        /// </summary>
        public void GenerateNewIdentity()
        {
            if (IsTransient())
                this.Id = IdentityGenerator.NewSequentialDomainId();
        }

        /// <summary>
        /// Change current identity for a new non transient identity
        /// </summary>
        /// <param name="identity">the new identity</param>
        public void ChangeCurrentIdentity(long identity)
        {
            if (identity != 0)
                this.Id = identity;
        }

        #endregion

        #region Overrides Methods

        /// <summary>
        /// <see cref="M:System.Object.Equals"/>
        /// </summary>
        /// <param name="obj"><see cref="M:System.Object.Equals"/></param>
        /// <returns><see cref="M:System.Object.Equals"/></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        /// <summary>
        /// <see cref="M:System.Object.GetHashCode"/>
        /// </summary>
        /// <returns><see cref="M:System.Object.GetHashCode"/></returns>
        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (Equals(left, null))
                return (Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        #endregion

        public static bool IsChild( Type type)
        {
            return type.IsSubclassOf(typeof(Entity));
        }
    }
}
