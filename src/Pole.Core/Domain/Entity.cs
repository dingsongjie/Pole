using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.Domain
{
    public abstract class Entity
    {
        string _id;
        public virtual string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public List<IEvent> DomainEvents { get; private set; }

        public bool IsPersisted { get; set; }
        public bool IsTransient()
        {
            return string.IsNullOrEmpty(this._id);
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }
        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
        public void AddDomainEvent(IEvent eventItem)
        {
            DomainEvents = DomainEvents ?? new List<IEvent>();
            DomainEvents.Add(eventItem);
        }
        public void RemoveDomainEvent(IEvent eventItem)
        {
            if (DomainEvents is null) return;
            DomainEvents.Remove(eventItem);
        }
        public void ClearDomainEvents()
        {
            DomainEvents?.Clear();
        }
    }
}
