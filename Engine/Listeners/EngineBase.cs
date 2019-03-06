using Engine.Interfaces;
using IBM.XMS;
using System;
using System.Reactive.Subjects;

namespace Engine.Listeners
{
    public abstract class EngineBase<T> : IEngineListener<T>, IDataConnection
    {


        protected  Subject<T> Subject { get; set; }
        public  abstract IObservable<T> MyObservableMessage { get; }

        public string QueueManagerName { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public string ChannelName { get; set; }
        public string TopicName { get; set; }

        public abstract void ReceiveMessages();
        public abstract void UnReceiveMessages();

        protected abstract void MyMessage(T message);

        protected  IConnectionFactory ConnectionFactory { get; set; }
        protected  IConnection Connection { get; set; }
        protected  ISession Session { get; set; }
        protected  IDestination Destination { get; set; }
        protected  IMessageConsumer Consumer { get; set; }

     

        protected abstract IConnection  MyCreateConnection(IConnectionFactory cf);
        protected abstract ISession MyCreateSession(IConnection connection);
        protected abstract IDestination MyCreateDestination(ISession session);
        protected abstract IMessageProducer MyCreateProducer(ISession session, IDestination destination);
        protected abstract IMessageConsumer MyCreateConsumer(ISession session, IDestination destination);


        protected abstract void OnException(Exception e);
        protected abstract void OnMessage(IMessage message);


    }
}
