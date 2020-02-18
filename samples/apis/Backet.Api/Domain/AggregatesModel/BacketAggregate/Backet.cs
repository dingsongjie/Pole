using Pole.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Domain.AggregatesModel.BacketAggregate
{
    public class Backet : Entity, IAggregateRoot
    {
        public void AddBacketItem(string productId, string productName, long Price)
        {
            BacketItem backetItem = new BacketItem()
            {
                Id = Guid.NewGuid().ToString("N"),
                Price = Price,
                ProductId = productId,
                ProductName = productName
            };
            BacketItems.Add(backetItem);
            SetBacketTotalPrice();
        }
        private void SetBacketTotalPrice()
        {
            foreach (var item in BacketItems)
            {
                TotalPrice += item.Price;
            }
        }
        public string UserId { get; set; }
        public List<BacketItem> BacketItems { get; private set; } = new List<BacketItem>();
        public long TotalPrice { get; private set; }
    }
}
