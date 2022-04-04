using Newtonsoft.Json;
using PubSub.Core;
using System.Collections.Generic;
using System.Linq;

namespace PubSub.Redis
{
    /// <summary>
    /// 消費者
    /// </summary>
    public class RedisConsumer
    {
        private IEnumerable<string> topics;

        private IPubSubDispatcher<RedisEventStream> dispatcher;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="dispatcher"></param>
        public RedisConsumer(IEnumerable<string> topics, IPubSubDispatcher<RedisEventStream> dispatcher)
        {
            this.topics = topics;
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// Redis訂閱設定
        /// </summary>
        public void Register()
        {
            this.topics.ToList().ForEach(t =>
            {
                var sub = RedisFactory.RedisSubscriber;
                sub.Subscribe($"{RedisFactory.AffixKey}:{t}", (topic, message) =>
                {
                    var @event = JsonConvert.DeserializeObject<RedisEventStream>(message.ToString());
                    this.dispatcher.DispatchMessage(@event);
                });
            });
        }
    }
}
