using StackExchange.Redis;

namespace PubSub.Redis
{
    public static class RedisFactory
    {
        private static ConnectionMultiplexer redisConn;

        private static int _dataBase;

        private static string _affixKey;

        /// <summary>
        /// 前墜詞
        /// </summary>
        public static string AffixKey
        {
            get
                => _affixKey;
        }

        /// <summary>
        /// 訂閱者
        /// </summary>
        public static ISubscriber RedisSubscriber
        {
            get
            {
                if (redisConn == null)
                {
                    return null;
                }

                redisConn.GetDatabase(_dataBase);
                return redisConn.GetSubscriber();
            }
        }

        /// <summary>
        /// 啟動
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="affixKey"></param>
        /// <param name="dataBase"></param>
        public static void Start(ConnectionMultiplexer conn, string affixKey, int dataBase)
        {
            if (redisConn != null)
            {
                return;
            }

            redisConn = conn;
            _affixKey = affixKey;
            _dataBase = dataBase;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public static void Stop()
        {
            if (redisConn == null)
            {
                return;
            }

            redisConn.Close();
            redisConn.Dispose();
        }
    }
}
