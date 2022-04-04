namespace PubSub.Core
{
    /// <summary>
    /// 處裡事件介面
    /// </summary>
    /// <typeparam name="TEventStream"></typeparam>
    public interface IPubSubHandler<TEventStream>
        where TEventStream : EventStream
    {
        /// <summary>
        /// 處裡事件內容
        /// </summary>
        /// <param name="stream">事件流</param>
        bool Handle(TEventStream stream);
    }
}
