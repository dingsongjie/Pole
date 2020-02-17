using Orleans;
using Pole.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.Grains
{
    public abstract class PoleGrain<TAggregateRoot> : Grain<TAggregateRoot>, IGrainWithStringKey
        where TAggregateRoot : Entity, IAggregateRoot, new()
    {
        public void Update(TAggregateRoot aggregateRoot)
        {
            aggregateRoot.IsPersisted = true;
        }
        public void Add(TAggregateRoot aggregateRoot)
        {
            aggregateRoot.IsPersisted = false;
        }
    }
}
