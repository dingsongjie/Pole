﻿using Backet.Api.Grains.Abstraction;
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
                UserId= backetDto.UserId
            };
            if (backetDto.BacketItems == null || backetDto.BacketItems.Count == 0) return false;
            backetDto.BacketItems.ForEach(item => {
                backet.AddBacketItem(item.ProductId, item.ProductName, item.Price);
            });
            Add(backet);
            await WriteStateAsync();
            return true;
        }
    }
}