using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;

namespace NorenRestApiWrapper
{
    public delegate void OnResponse(NorenResponseMsg Response, bool ok);

    public class BaseApiResponse
    {
        public virtual void OnMessageNotify(HttpResponseMessage httpResponse, string data)
        {

        }
    }
    public class NorenApiResponse<T> :  BaseApiResponse where T : NorenResponseMsg, new()
    {
        public NorenApiResponse(OnResponse Response)
        {
            onResponse = Response;
        }
        OnResponse onResponse;
        public override void OnMessageNotify(HttpResponseMessage httpResponse, string data)
        {
            //T Message = Helpers.ToObject<T>(PayLoad);
            T Message = new T();

            if (Message == null)
                return;

            if (httpResponse.IsSuccessStatusCode)
            { 
                Message = JsonConvert.DeserializeObject<T>(data);
                onResponse(Message, true);
            }
            else
            {
                Message.stat = data;
                onResponse(Message, false);
            }


        }
    }

}
