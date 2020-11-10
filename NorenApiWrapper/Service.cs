using System;
using System.IO;            //Needs to be added
using System.Net;           //Needs to be added
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;          //Needs to be added
using Websocket.Client;

namespace NorenRestApiWrapper
{
    public class RESTClient
    {
        private HttpClient client = new HttpClient();
        private string _endPoint;
        public string endPoint
        {
            get => _endPoint;

            set
            {
                _endPoint = value;

                client.BaseAddress = new Uri(value);
            }
        }


        //Default Constructor
        public RESTClient()
        {            
            client.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
        }

       

        public async void makeRequest(BaseApiResponse response,string uri, string message)
        {
            string strResponseValue = string.Empty;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(message,
                                                Encoding.UTF8,
                                                "application/json");//CONTENT-TYPE header


            await client.SendAsync(request)
                  .ContinueWith(async responseTask =>
                  {
                      string data = String.Empty;
                      Console.WriteLine("Response: {0}", responseTask.Result);

                      //if(responseTask.IsCompleted && responseTask.Result.IsSuccessStatusCode)
                      data = await responseTask.Result.Content.ReadAsStringAsync();

                      Console.WriteLine("Response data: {0}", data);
                      response.OnMessageNotify(responseTask.Result, data);
                      

                  });
            

            return;

        }
    }
}