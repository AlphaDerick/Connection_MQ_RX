

namespace Engine.Interfaces
{
   public interface IDataConnection
    {
        string QueueManagerName { get; }
        string HostName { get; }
        int Port { get; }
        string ChannelName { get; }
        string TopicName { get; }

    }
}
