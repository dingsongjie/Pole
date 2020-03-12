using Pole.Core.Serialization;
using Pole.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.EventBus.Event
{
    public readonly struct EventBytesTransport
    {
        public EventBytesTransport(string eventCode, string eventId, byte[] eventBytes)
        {
            EventTypeCode = eventCode;
            EventBytes = eventBytes;
            EventId = eventId;
        }
        /// <summary>
        /// 每个类型的Event 全局唯一
        /// </summary>
        public string EventId { get; }
        /// <summary>
        /// 事件TypeCode
        /// </summary>
        public string EventTypeCode { get; }
        /// <summary>
        /// 事件本身的bytes
        /// </summary>
        public byte[] EventBytes { get; }
        public byte[] GetBytes()
        {
            var eventTypeBytes = Encoding.UTF8.GetBytes(EventTypeCode);
            var eventIdBytes = Encoding.UTF8.GetBytes(EventId);
            using var ms = new PooledMemoryStream();
            ms.WriteByte((byte)TransportType.Event);
            ms.Write(BitConverter.GetBytes((ushort)eventTypeBytes.Length));
            ms.Write(BitConverter.GetBytes((ushort)eventIdBytes.Length));
            ms.Write(BitConverter.GetBytes(EventBytes.Length));
            ms.Write(eventTypeBytes);
            ms.Write(eventIdBytes);
            ms.Write(EventBytes);
            return ms.ToArray();
        }
        public static (bool success, EventBytesTransport transport) FromBytes(byte[] bytes)
        {
            if (bytes[0] == (byte)TransportType.Event)
            {
                var bytesSpan = bytes.AsSpan();
                var eventTypeCodeLength = BitConverter.ToUInt16(bytesSpan.Slice(1, sizeof(ushort)));
                var eventIdLength = BitConverter.ToUInt16(bytesSpan.Slice(1 + sizeof(ushort), sizeof(ushort)));
                var eventBytesLength = BitConverter.ToInt32(bytesSpan.Slice(1 + 2 * sizeof(ushort), sizeof(int)));
                var skipLength = 2 * sizeof(ushort) + 1 + sizeof(int);
                return (true, new EventBytesTransport(
                    Encoding.UTF8.GetString(bytesSpan.Slice(skipLength, eventTypeCodeLength)),
                    Encoding.UTF8.GetString(bytesSpan.Slice(skipLength + eventTypeCodeLength, eventIdLength)),
                    bytesSpan.Slice(skipLength + eventTypeCodeLength + eventIdLength, eventBytesLength).ToArray()
                    ));
            }
            return (false, default);
        }
    }
}
