using Pole.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Domain.AggregatesModel.BacketAggregate
{
    public class BacketItem : Entity
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public long Price { get; set; }
        public string BacketId { get; set; }
    }
}
