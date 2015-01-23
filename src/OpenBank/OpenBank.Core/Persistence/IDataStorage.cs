using System;
using System.Collections.Generic;
using OpenBank.Core.Data;

namespace OpenBank.Core.Persistence
{
    public interface IDataStorage
    {
        void Save(TimedData data);
        ICollection<TimedData> LoadAll(DateTime from, DateTime to);
    }
}