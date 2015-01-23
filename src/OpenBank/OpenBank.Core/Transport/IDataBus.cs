using System;

namespace OpenBank.Core.Transport
{
    public interface IDataBus
    {
        void Publish<T>(T data);
        IDisposable Subscribe<T>(Action<T> handler);
    }
}