using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IJsonConverter
    {
        string Serialize(object obj);
        T Deserialize<T>(string json);
        object Deserialize(string json,Type type);
    }
}
