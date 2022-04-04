namespace PubSub.Core
{
    /// <summary>
    /// 事件流
    /// </summary>
    public class EventStream
    {
        /// <summary>
        /// 事件名稱
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 事件內容
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 事件產生時間
        /// </summary>
        public long UtcTimeStamp { get; set; }
    }
}
