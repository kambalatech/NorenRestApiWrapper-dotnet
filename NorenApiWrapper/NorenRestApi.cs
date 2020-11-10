using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Websocket.Client;

namespace NorenRestApiWrapper
{
    public class NorenRestApi
    {
        RESTClient rClient;
        WebsocketClient wsclient;
        bool loggedin;
        LoginRespMessage loginResp;
        public NorenRestApi()
        {            
            rClient = new RESTClient();            
        }
        public void OnLoginResponse(NorenResponseMsg Response, bool ok)
        {
            if(ok)
            { 
                loginResp = Response as LoginRespMessage;
            }

        }
        public bool SendLogin(OnResponse response, string endPoint,LoginMessage login)
        {
            
            rClient.endPoint = endPoint;
            string uri = "QuickAuth";
            var ResponseHandler = new NorenApiResponse<LoginRespMessage>(response);

            
            rClient.makeRequest(ResponseHandler, uri, login.toJson());
            return true;
        }

        public bool SendLogout()
        {
            LogoutMessage logout = new LogoutMessage();
            logout.uid = _user.uid;
            logout.jKey = "1234";
            string uri = "Logout";
            rClient.makeRequest(null, uri, logout.toJson());
            return true;
        }

        public bool AddFeedDevice(string uri)
        {
            var url = new Uri(uri);
            wsclient = new WebsocketClient(url);

            wsclient.ReconnectTimeout = TimeSpan.FromSeconds(30);
            wsclient.ReconnectionHappened.Subscribe(info =>
                Console.WriteLine($"Reconnection happened, type: {info.Type}"));

            ConnectMessage connect = new ConnectMessage();
            connect.t = "c";
            connect.uid = _user.uid;
            connect.actid = _user.uid;
            connect.susertoken = "54321";

            wsclient.MessageReceived.Subscribe(msg => Console.WriteLine($"Message received: {msg}"));
            wsclient.Start();
            wsclient.Send(connect.toJson());
            return true;
        }

        public bool SubscribeToken(string exch, string token)
        {
            SubsTouchline subs = new SubsTouchline();

            subs.k = exch + "|" + token;

            wsclient.Send(subs.toJson());
            return true;
        }
    }
}
