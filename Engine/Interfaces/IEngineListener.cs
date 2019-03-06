
using System;

namespace Engine.Interfaces
{
    public interface IEngineListener<T> 
    {
        IObservable<T> MyObservableMessage { get; }

        void ReceiveMessages();

        void UnReceiveMessages();

    }
}
