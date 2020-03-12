using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.EventBus.EventStorage
{
    public class EventEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime Added { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int Retries { get; set; }
        public string StatusName { get; set; }
    }
}
