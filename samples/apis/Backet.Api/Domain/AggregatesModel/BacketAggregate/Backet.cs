using Pole.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Domain.AggregatesModel.BacketAggregate
{
    public class Backet: Entity,IAggregateRoot
    {
        public string UserId { get; set; }
        public IReadOnlyCollection<BacketItem> BacketItems { get; private set; }
        public long TotalPrice { get; set; }
    }
}
