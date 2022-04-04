using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;

namespace PubSub.RabbitMq
{
    /// <summary>
    /// RMQ工廠
    /// </summary>
    public static class RabbitMqFactory
    {
        private static ConnectionFactory factory;

        private static IConnection connection;

        /// <summary>
        /// Ack委派
        /// </summary>
        public static Action<bool, RabbitMqEventStream> ActionBasicAck;

        /// <summary>
        /// Nack委派
        /// </summary>
        public static Action<RabbitMqEventStream> ActionBasicNack;

        private static ConcurrentDictionary<string, IModel> models = new ConcurrentDictionary<string, IModel>();

        private static bool TryAddModel(string topicName, string exchangeType)
        {
            if (!models.ContainsKey(topicName))
            {
                var chennel = connection.CreateModel();
                chennel.ExchangeDeclare($"Exchange-{exchangeType}-{topicName}", exchangeType);
                models.TryAdd(topicName, chennel);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 取得channel
        /// </summary>
        /// <param name="topicName">Topic</param>
        /// <param name="exchangeType">事件類型</param>
        /// <returns></returns>
        public static IModel GetChannel(string topicName, string exchangeType = ExchangeType.Direct)
        {
            TryAddModel(topicName, exchangeType);
            return models[topicName];
        }

        /// <summary>
        /// 工廠上班
        /// </summary>
        /// <param name="userName">RMQ帳號</param>
        /// <param name="password">RMQ密碼</param>
        /// <param name="rabbitMqUri">RMQ服務網址</param>
        /// <param name="actionBasicAck">Ack委派</param>
        /// <param name="actionBasicNack">Nack委派</param>
        /// <param name="shutdownEvent">RMQ掛了委派</param>
        /// <returns>是否成功建立AMQP的連線</returns>
        public static bool Start(
            string userName,
            string password,
            string rabbitMqUri,
            Action<bool, RabbitMqEventStream> actionBasicAck = null,
            Action<RabbitMqEventStream> actionBasicNack = null,
            EventHandler<ShutdownEventArgs> shutdownEvent = null
        )
        {
            if (actionBasicAck != null)
            {
                ActionBasicAck = actionBasicAck;
            }

            if (actionBasicNack != null)
            {
                ActionBasicNack = actionBasicNack;
            }

            factory = new ConnectionFactory()
            {
                UserName = userName,
                Password = password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            connection = factory.CreateConnection(AmqpTcpEndpoint.ParseMultiple(rabbitMqUri));

            if (shutdownEvent != null)
            {
                connection.ConnectionShutdown += shutdownEvent;
            }

            return connection.IsOpen;
        }

        /// <summary>
        /// 工廠下班
        /// </summary>
        /// <param name="timeout">
        /// 若有設定超時，則時間內未完成會強制關閉連線，此參數為milliseconds
        /// </param>
        public static void Stop(int? timeout = null)
        {
            if (factory == null || models == null)
            {
                return;
            }

            if (timeout.HasValue)
            {
                connection.Abort(new TimeSpan(0, 0, 0, 0, timeout.Value));
            }
            else
            {
                connection.Abort();
            }

            models.Clear();
        }
    }
}
