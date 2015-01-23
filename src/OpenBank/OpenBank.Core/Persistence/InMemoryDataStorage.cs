using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenBank.Core.Data;

namespace OpenBank.Core.Persistence
{
    public sealed class InMemoryDataStorage : IDataStorage
    {
        readonly ICollection<TimedData> m_Storage = new Collection<TimedData>();

        public void Save(TimedData data)
        {
            m_Storage.Add(data);
        }

        public ICollection<TimedData> LoadAll(DateTime from, DateTime to)
        {
            return m_Storage.Where(x => x.Timestamp >= from && x.Timestamp <= to).OrderBy(x => x.Timestamp).ToArray();
        }
    }
}