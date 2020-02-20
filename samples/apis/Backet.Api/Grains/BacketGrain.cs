using Backet.Api.Domain.Event;
using Backet.Api.Grains.Abstraction;
using Pole.Core.Grains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Grains
{
    public class BacketGrain : PoleGrain<Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet>, IBacketGrain
    {
        public async Task<bool> AddBacket(BacketDto backetDto)
        {
            if (State != null) return false;

            Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet backet = new Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet
            {
                Id = backetDto.Id,
                UserId = backetDto.UserId
            };
            if (backetDto.BacketItems == null || backetDto.BacketItems.Count == 0) return false;
            backetDto.BacketItems.ForEach(item =>
            {
                backet.AddBacketItem(item.ProductId, item.ProductName, item.Price);
            });
            Add(backet);
            backet.AddDomainEvent(new BacketCreatedEvent() { BacketId = backet.Id });
            await WriteStateAsync();
            return true;
        }

        public async Task<bool> AddBacketItem(string productId, string productName, long price)
        {
            if (State == null) return false;

            State.AddBacketItem(productId, productName, price);
            Update(State);
            await WriteStateAsync();
            return true;
        }

        public async Task<bool> RemoveFirstItem()
        {
            State.RemoveFirstItem();
            Update(State);
            await WriteStateAsync();
            return true;
        }

        public async Task<bool> UpdateBacket(string userId)
        {
            if (State == null) return false;
            State.UserId = userId;
            State.ModifyItemProductId(userId);
            Update(State);
            await WriteStateAsync();
            return true;
        }
    }
}
