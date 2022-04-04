using Newtonsoft.Json;
using System;

namespace PubSub.Redis
{
    /// <summary>
    /// Redis事件生產者
    /// </summary>
    public static class RedisProducer
    {
        /// <summary>
        /// 發布事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topicName">發布Topic目標</param>
        /// <param name="data">事件內容</param>
        /// <param name="rmqExpiration">訊息存活時間(預設1天)</param>
        public static void Publish<T>(string topicName, T data, string rmqExpiration = "86400000")
        {
            var sub = RedisFactory.RedisSubscriber;
            var content = new RedisEventStream(
                typeof(T).Name,
                JsonConvert.SerializeObject(data),
                TimeStampHelper.ToUtcTimeStamp(DateTime.Now));

            sub.Publish($"{RedisFactory.AffixKey}:{topicName}", JsonConvert.SerializeObject(content));
        }
    }
}
