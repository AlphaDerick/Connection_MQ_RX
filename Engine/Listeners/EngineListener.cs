using Engine.Interfaces;
using IBM.XMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Listeners
{
   public class EngineListener : EngineBase<ITextMessage>
    {

        public EngineListener(IDataConnection connection)
        {
            QueueManagerName = connection.QueueManagerName;
            Port = connection.Port;
            ChannelName = connection.ChannelName;
            HostName = connection.HostName;
            TopicName = connection.TopicName;

            Subject = new Subject<ITextMessage>();
        }

        public override IObservable<ITextMessage> MyObservableMessage
        {
            get { return Subject.AsObservable(); }
        }


        protected IConnectionFactory MyCreateConnectionFactory()
        {

            XMSFactoryFactory factoryFactory = XMSFactoryFactory.GetInstance(XMSC.CT_WMQ);
            IConnectionFactory cf = factoryFactory.CreateConnectionFactory();
            cf.SetStringProperty(XMSC.WMQ_HOST_NAME, HostName);
            cf.SetIntProperty(XMSC.WMQ_PORT, Port);
            cf.SetIntProperty(XMSC.WMQ_CONNECTION_MODE, XMSC.WMQ_CM_CLIENT);
            cf.SetStringProperty(XMSC.WMQ_QUEUE_MANAGER, QueueManagerName);
            cf.SetIntProperty(XMSC.WMQ_BROKER_VERSION, XMSC.WMQ_BROKER_V1);
            cf.SetStringProperty(XMSC.WMQ_CHANNEL, ChannelName);

            return (cf);
        }

        protected override IConnection MyCreateConnection(IConnectionFactory cf)
        {
            IConnection connection = cf.CreateConnection();
            connection.ExceptionListener = new ExceptionListener(OnException);

            return (connection);
        }

        protected override ISession MyCreateSession(IConnection connection)
        {
            ISession session = connection.CreateSession(false, AcknowledgeMode.AutoAcknowledge);
            return (session);
        }

        protected override IDestination MyCreateDestination(ISession session)
        {
            IDestination iDestination;
            iDestination = session.CreateTopic(TopicName);
            iDestination.SetIntProperty(XMSC.DELIVERY_MODE, XMSC.WMQ_CLIENT_NONJMS_MQ);

            return (iDestination);
        }

        protected override IMessageProducer MyCreateProducer(ISession session, IDestination destination)
        {
            IMessageProducer producer = session.CreateProducer(destination);
            return (producer);
        }

        protected override IMessageConsumer MyCreateConsumer(ISession session, IDestination destination)
        {
            IMessageConsumer consumer = session.CreateConsumer(destination);
            consumer.MessageListener = new MessageListener(OnMessage);
            return (consumer);
        }


        public override void ReceiveMessages()
        {
            try
            {
                ConnectionFactory = MyCreateConnectionFactory();
                Connection = MyCreateConnection(ConnectionFactory);
                Session = MyCreateSession(Connection);
                Destination = MyCreateDestination(Session);
                Consumer = MyCreateConsumer(Session, Destination);
                Connection.Start();
            }
            catch (Exception)
            {
                //...
            }
        }

        public override void UnReceiveMessages()
        {
            try
            {
                Connection.Close();
            }
            catch (Exception)
            {
                //...
            }
            finally
            {
                // Dispose Objects ??
            }

        }

        protected override void MyMessage(ITextMessage message)
        {
            Subject.OnNext(message);
        }

        protected override void OnException(Exception e)
        {
            // Logic Kill o Reconnect
        }

        protected override void OnMessage(IMessage message)
        {
            ITextMessage msg = message as ITextMessage;
            MyMessage(msg);
        }

    }
}
