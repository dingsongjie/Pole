using System;
using System.Collections.Generic;

namespace Pole.EventBus
{
    public interface IObserverUnitContainer
    {
        List<IObserverUnit<PrimaryKey>> GetUnits<PrimaryKey>(string observerName);
        List<object> GetUnits(string observerName);
        void Register<PrimaryKey>(string observerName,IGrainID followUnit);
    }
}
