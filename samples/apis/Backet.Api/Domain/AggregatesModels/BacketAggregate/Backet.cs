﻿using Pole.Core.Domain;
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
        public void ModifyItemProductId(string productId)
        {
            BacketItems.ForEach(m => m.ProductId = productId);
        }
        private void SetBacketTotalPrice()
        {
            foreach (var item in BacketItems)
            {
                TotalPrice = BacketItems.Sum(m=>m.Price);
            }
        }
        public string UserId { get; set; }
        public List<BacketItem> BacketItems { get; private set; } = new List<BacketItem>();
        public long TotalPrice { get;  set; }

        internal void RemoveFirstItem()
        {
            var first = BacketItems.FirstOrDefault();
            if (first != null)
            {
                BacketItems.Remove(first);
                SetBacketTotalPrice();
            }
        }
    }
}
