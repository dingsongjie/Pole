using Pole.ReliableMessage.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Messaging
{
    class DefaultJsonConverter : IJsonConverter
    {
        public T Deserialize<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public object Deserialize(string json, Type type)
        {
            return System.Text.Json.JsonSerializer.Deserialize(json, type);
        }

        public string Serialize(object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }
    }
}
