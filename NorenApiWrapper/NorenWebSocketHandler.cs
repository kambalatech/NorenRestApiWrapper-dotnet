using Newtonsoft.Json;
using Websocket.Client;

namespace NorenRestApiWrapper
{
    public delegate void OnStreamMesssage(NorenStreamMessage Feed);
    public class BaseWSMessage
    {
        public virtual void OnMessageNotify(ResponseMessage msg, string data)
        {

        }
    }
    public class NorenStreamMessage<T> : BaseWSMessage where T : NorenStreamMessage, new()
    {
        public OnStreamMesssage MessageHandler;
        public NorenStreamMessage(OnStreamMesssage Response)
        {
            MessageHandler = Response;
        }

    }
}
