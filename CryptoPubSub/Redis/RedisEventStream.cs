using PubSub.Core;

namespace PubSub.Redis
{
    /// <summary>
    /// Redis事件流
    /// </summary>
    public class RedisEventStream : EventStream
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="utcTimeStamp"></param>
        public RedisEventStream(string type, string data, long utcTimeStamp)
        {
            Type = type;
            Data = data;
            UtcTimeStamp = utcTimeStamp;
        }
    }

    /// <summary>
    /// Redis處理事件介面
    /// </summary>
    public interface IRedisPubSubHandler : IPubSubHandler<RedisEventStream>
    {

    }
}
