using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace PubSub.RabbitMq
{
    /// <summary>
    /// RMQ生產者
    /// </summary>
    public static class RabbitMqProducer
    {
        /// <summary>
        /// 保存發送紀錄，驗證Ack、Nack事件指的是哪個事件
        /// </summary>
        private static ConcurrentDictionary<ulong, RabbitMqEventStream> pubConfirmDic = new ConcurrentDictionary<ulong, RabbitMqEventStream>();

        /// <summary>
        /// AMQP協議行為
        /// </summary>
        private static IModel channel;

        /// <summary>
        /// 取得發送編號的Lock
        /// </summary>
        private static object mLock = new object();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="topicName"></param>
        public static void Init(string topicName)
        {
            channel = RabbitMqFactory.GetChannel(topicName);
            channel.ConfirmSelect();

            //發送成功委派事件
            channel.BasicAcks += (o, eventArgs) =>
            {
                if (!pubConfirmDic.ContainsKey(eventArgs.DeliveryTag))
                {
                    RabbitMqFactory.ActionBasicAck(false, null); //找不到Tag
                }
                else
                {
                    pubConfirmDic.TryRemove(eventArgs.DeliveryTag, out RabbitMqEventStream data);
                    RabbitMqFactory.ActionBasicAck(true, data); //發送成功
                }
            };

            //發送失敗委派事件
            channel.BasicNacks += (o, eventArgs) =>
            {
                pubConfirmDic.TryRemove(eventArgs.DeliveryTag, out RabbitMqEventStream data);
                RabbitMqFactory.ActionBasicNack(data);
            };
        }

        /// <summary>
        /// 發布事件
        /// </summary>
        /// <typeparam name="T"></typeparam>`
        /// <param name="topicName">發布Topic目標</param>
        /// <param name="data">事件內容</param>
        /// <param name="exchangeType">事件類型</param>
        /// <param name="rmqExpiration">訊息存活時間(預設1天)</param>
        public static void Publish<T>(string topicName, T data, string exchangeType = ExchangeType.Direct, string rmqExpiration = "86400000")
        {
            var channel = RabbitMqFactory.GetChannel(topicName, exchangeType);
            var rabbitMqEventStream = new RabbitMqEventStream(
                typeof(T).Name,
                JsonConvert.SerializeObject(data),
                TimeStampHelper.ToUtcTimeStamp(DateTime.Now));

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rabbitMqEventStream));
            var prop = channel.CreateBasicProperties();
            prop.Expiration = rmqExpiration;

            lock (mLock)
            {
                var nextPublishSeqNo = channel.NextPublishSeqNo;

                try
                {
                    pubConfirmDic.TryAdd(nextPublishSeqNo, rabbitMqEventStream);

                    channel.BasicPublish(
                        $"Exchange-{exchangeType}-{topicName}",
                        string.Empty,
                        prop,
                        body);
                }
                catch (Exception ex)
                {
                    pubConfirmDic.TryRemove(nextPublishSeqNo, out _);
                    throw ex;
                }
            }
        }
    }
}
