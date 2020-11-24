using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public class NorenApiResponseList<T, U> : BaseApiResponse where T : NorenListResponseMsg<U>, new()
    {
        //watchers interested in the response
        public delegate void ResponseNotify(NorenResponseMsg httpResponse); // declare a delegate  
        public ResponseNotify ResponseNotifyInstance; // create a delegate instance  
        //the handler who will process the response
        OnResponse ResponseHandler;

        public NorenApiResponseList(OnResponse Response)
        {
            ResponseHandler = Response;
        }
        public override void OnMessageNotify(HttpResponseMessage httpResponse, string data)
        {
            //T Message = Helpers.ToObject<T>(PayLoad);
            T Message = new T();

            if (Message == null)
                return;

            if (httpResponse.IsSuccessStatusCode)
            {
                try
                {
                    Message.list = JsonConvert.DeserializeObject<List<U>>(data);
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine($"Error deserializing data {ex.ToString()}");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing data {ex.ToString()}");
                    return;
                }
                ResponseNotifyInstance?.Invoke(Message);
                ResponseHandler(Message, true);
            }
            else
            {
                Message.stat = data;
                ResponseHandler(Message, false);
            }
        }
    }

    public class NorenApiResponse<T> :  BaseApiResponse where T : NorenResponseMsg, new()
    {
        //watchers interested in the response
        public delegate void ResponseNotify(NorenResponseMsg httpResponse); // declare a delegate  
        public ResponseNotify ResponseNotifyInstance; // create a delegate instance  
        //the handler who will process the response
        OnResponse ResponseHandler;

        public NorenApiResponse(OnResponse Response)
        {
            ResponseHandler = Response;
        }
        public override void OnMessageNotify(HttpResponseMessage httpResponse, string data)
        {
            //T Message = Helpers.ToObject<T>(PayLoad);
            T Message = new T();

            if (Message == null)
                return;

            if (httpResponse.IsSuccessStatusCode)
            {
                try
                {
                    if (Message is NorenListResponseMsg<T>)
                    {
                        NorenListResponseMsg<T> Response = new NorenListResponseMsg<T>();
                        Response.list = JsonConvert.DeserializeObject<List<T>>(data);
                        ResponseNotifyInstance?.Invoke(Response);
                        ResponseHandler(Response, true);
                    }
                    else
                    {
                        Message = JsonConvert.DeserializeObject<T>(data);
                        ResponseNotifyInstance?.Invoke(Message);
                        ResponseHandler(Message, true);
                    }
                } 
                catch(JsonReaderException  ex)
                {
                    Console.WriteLine($"Error deserializing data {ex.ToString()}");
                    return;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error deserializing data {ex.ToString()}");
                    return;
                }
                
            }
            else
            {
                Message.stat = data;
                ResponseHandler(Message, false);
            }

            
        }
    }

}
