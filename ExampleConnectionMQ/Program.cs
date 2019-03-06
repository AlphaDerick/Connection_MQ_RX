using Engine.Interfaces;
using Engine.Listeners;
using System;
using IBM.XMS;
using System.Reactive.Linq;
using System.Configuration;

namespace ExampleConnectionMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            EngineListener session = new EngineListener(GetParameters());
            session.ReceiveMessages();

            var suscribe = session.MyObservableMessage.Subscribe(WriteText);

            Console.Read();
            session.UnReceiveMessages();
            Console.WriteLine("close connection");
            Console.Read();
        }

        private static void WriteText(ITextMessage textMessage)
        {
            Console.WriteLine(textMessage.Text);
        }

        private static IDataConnection GetParameters()
        {
            return new DataConnection()
            {
                QueueManagerName = GetAppConfig("QueueManager"),
                Port = int.Parse(GetAppConfig("Port")),
                ChannelName = GetAppConfig("Channel"),
                HostName = GetAppConfig("Host"),
                TopicName = GetAppConfig("Topic"),
            };
        }

        public static string GetAppConfig(string value)
        {
            try
            {
                return ConfigurationManager.AppSettings[value];
            }
            catch (Exception )
            {
                //..
               return string.Empty;
            }
        }


    }

    public class DataConnection : IDataConnection
    {
        public string QueueManagerName { get; set; }

        public string HostName { get; set; }

        public int Port { get; set; }

        public string ChannelName { get; set; }

        public string TopicName { get; set; }
    }


}
