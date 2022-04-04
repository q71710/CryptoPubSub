using PubSub.Core;

namespace PubSub.RabbitMq
{
    /// <summary>
    /// RMQ事件流
    /// </summary>
    public class RabbitMqEventStream : EventStream
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="utcTimeStamp"></param>
        public RabbitMqEventStream(string type, string data, long utcTimeStamp)
        {
            Type = type;
            Data = data;
            UtcTimeStamp = utcTimeStamp;
        }
    }

    /// <summary>
    /// RMQ處裡事件介面
    /// </summary>
    public interface IRabbitMqPubSubHandler : IPubSubHandler<RabbitMqEventStream>
    {
    }
}
