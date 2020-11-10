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
        LoginMessage loginReq;
        public NorenRestApi()
        {            
            rClient = new RESTClient();            
        }

        private string getJKey
        {
            get
            {
                return "jKey=" + loginResp?.susertoken;
            }
        }
        public void OnLoginResponseNotify(NorenResponseMsg responseMsg)
        {
            loginResp = responseMsg as LoginRespMessage;
        }
        public bool SendLogin(OnResponse response, string endPoint,LoginMessage login)
        {
            loginReq = login;
            rClient.endPoint = endPoint;
            string uri = "QuickAuth";
            var ResponseHandler = new NorenApiResponse<LoginRespMessage>(response);
            ResponseHandler.ResponseNotifyInstance += OnLoginResponseNotify;


            rClient.makeRequest(ResponseHandler, uri, login.toJson());
            return true;
        }

        public bool SendLogout(OnResponse response)
        {
            if (loginResp == null)
                return false;

            LogoutMessage logout = new LogoutMessage();
            logout.uid = loginReq.uid;
            
            string uri = "Logout";
            var ResponseHandler = new NorenApiResponse<LogoutRespMessage>(response);
            rClient.makeRequest(ResponseHandler, uri, logout.toJson(), getJKey);
            return true;
        }

        public bool SendGetUserDetails(OnResponse response)
        {
            if (loginResp == null)
                return false;

            UserDetailsMessage userDetails = new UserDetailsMessage();
            userDetails.uid  = loginReq.uid;
            string uri = "UserDetails";
            
            rClient.makeRequest(new NorenApiResponse<UserDetailsRespMessage>(response), uri, userDetails.toJson(), getJKey);
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
            connect.uid = loginReq.uid;
            connect.actid = loginReq.uid;
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
