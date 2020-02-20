using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Grains.Abstraction
{
    public interface IBacketGrain: IGrainWithStringKey
    {
        Task<bool> AddBacket(BacketDto backet);
        Task<bool> UpdateBacket(string userId);
        Task<bool> AddBacketItem(string productId, string productName, long price);
        Task<bool> RemoveFirstItem();
    }
    public class BacketItemDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public long Price { get; set; }
    }
    public class BacketDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public List<BacketItemDto> BacketItems { get; set; }
    }
}
