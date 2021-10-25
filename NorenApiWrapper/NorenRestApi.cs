using Newtonsoft.Json;
using System;
using System.Text;
using Websocket.Client;
using System.Security.Cryptography;


namespace NorenRestApiWrapper
{
    /// <summary>
    /// Callback when feed is received
    /// </summary>
    /// <param name="Feed"></param>
    public delegate void OnFeed(NorenFeed Feed);
    /// <summary>
    /// callback when order updates are received
    /// </summary>
    /// <param name="Feed"></param>
    public delegate void OnOrderFeed(NorenOrderFeed Feed);
    /// <summary>
    /// only after this call back feed and orders are to be subscribed
    /// </summary>
    /// <param name="msg"></param>
    public delegate void OnStreamConnect(NorenStreamMessage msg);

    public class NorenRestApi
    {
        RESTClient rClient;
        WebsocketClient wsclient;
        
        LoginResponse loginResp;
        LoginMessage loginReq;
        public OnFeed OnFeedCallback;
        public OnOrderFeed OnOrderCallback;
        public OnStreamConnect onStreamConnectCallback;

        public OnResponse SessionCloseCallback
        {
            set
            {
                rClient.onSessionClose = value;
            }
        }
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
        #region response handlers
        private void OnWSHandler(ResponseMessage msg)
        {
            NorenStreamMessage wsmsg;
            try
            {
                wsmsg = JsonConvert.DeserializeObject<NorenStreamMessage>(msg.Text);
                if(wsmsg.t =="ck")
                {
                    onStreamConnectCallback?.Invoke(wsmsg);
                }
                else if (wsmsg.t == "om" || wsmsg.t == "ok")
                {
                    NorenOrderFeed ordermsg = JsonConvert.DeserializeObject<NorenOrderFeed>(msg.Text);
                    OnOrderCallback?.Invoke(ordermsg);
                }
                else
                { 
                    NorenFeed feedmsg = JsonConvert.DeserializeObject<NorenFeed>(msg.Text);
                    OnFeedCallback?.Invoke(feedmsg);
                }
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Error deserializing data {ex.ToString()}");
                return;
            }
            
            //
        }
        internal void OnLoginResponseNotify(NorenResponseMsg responseMsg)
        {
            loginResp = responseMsg as LoginResponse;
        }
        #endregion
        #region helpers
        string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        #endregion
        #region user login/logout
        /// <summary>
        /// This should be the first request. No further requests to be sent before login success
        /// </summary>
        /// <param name="response"></param>
        /// <param name="endPoint"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public bool SendLogin(OnResponse response, string endPoint,LoginMessage login)
        {
            loginReq = login;
            login.pwd = ComputeSha256Hash(login.pwd);
            

            login.appkey = ComputeSha256Hash(login.uid + "|" + login.appkey);

            rClient.endPoint = endPoint;
            string uri = "QuickAuth";
            var ResponseHandler = new NorenApiResponse<LoginResponse>(response);
            ResponseHandler.ResponseNotifyInstance += OnLoginResponseNotify;


            rClient.makeRequest(ResponseHandler, uri, login.toJson());
            return true;
        }
        /// <summary>
        /// Logout the current user
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool SendLogout(OnResponse response)
        {
            if (loginResp == null)
                return false;

            LogoutMessage logout = new LogoutMessage();
            logout.uid = loginReq.uid;
            
            string uri = "Logout";
            var ResponseHandler = new NorenApiResponse<LogoutResponse>(response);
            rClient.makeRequest(ResponseHandler, uri, logout.toJson(), getJKey);
            return true;
        }

        public bool Changepwd(OnResponse response, Changepwd changepwd)
        {
            if (loginResp == null)
                return false;

            
            changepwd.uid = loginReq.uid;
            changepwd.oldpwd = ComputeSha256Hash(changepwd.oldpwd);
            string uri = "Changepwd";
            var ResponseHandler = new NorenApiResponse<ChangepwdResponse>(response);
            rClient.makeRequest(ResponseHandler, uri, changepwd.toJson());
            return true;
        }
        public bool SendProductConversion(OnResponse response, ProductConversion productConversion)
        {
            if (loginResp == null)
                return false;

            productConversion.uid = loginReq.uid;
            
            string uri = "ProductConversion";
            var ResponseHandler = new NorenApiResponse<ProductConversionResponse>(response);
            rClient.makeRequest(ResponseHandler, uri, productConversion.toJson(), getJKey);
            return true;
        }
        public bool SendForgotPassword(OnResponse response, string endpoint, string user, string pan, string dob)
        {            
            ForgotPassword forgotPassword = new ForgotPassword();
            forgotPassword.uid = user;
            forgotPassword.pan = pan;
            forgotPassword.dob = dob;

            string uri = "ForgotPassword";
            rClient.endPoint = endpoint;
            var ResponseHandler = new NorenApiResponse<ForgotPasswordResponse>(response);
            rClient.makeRequest(ResponseHandler, uri, forgotPassword.toJson());
            return true;
        }

        #endregion
        public bool SendGetUserDetails(OnResponse response)
        {
            if (loginResp == null)
                return false;

            UserDetails userDetails = new UserDetails();
            userDetails.uid  = loginReq.uid;
            string uri = "UserDetails";
            
            rClient.makeRequest(new NorenApiResponse<UserDetailsResponse>(response), uri, userDetails.toJson(), getJKey);
            return true;
        }
        public bool SendGetMWList(OnResponse response)
        {
            if (loginResp == null)
                return false;

            UserDetails userDetails = new UserDetails();
            userDetails.uid = loginReq.uid;
            string uri = "MWList";

            rClient.makeRequest(new NorenApiResponse<MWListResponse>(response), uri, userDetails.toJson(), getJKey);
            return true;
        }
        public bool SendSearchScrip(OnResponse response, string exch, string searchtxt)
        {
            if (loginResp == null)
                return false;

            SearchScrip searchScrip = new SearchScrip();
            searchScrip.uid = loginReq.uid;
            searchScrip.exch = exch;
            searchScrip.stext = searchtxt;
            string uri = "SearchScrip";

            rClient.makeRequest(new NorenApiResponse<SearchScripResponse>(response), uri, searchScrip.toJson(), getJKey);
            return true;
        }
        
        public bool SendGetSecurityInfo(OnResponse response, string exch, string token)
        {
            if (loginResp == null)
                return false;

            GetSecurityInfo getsecurityinfo = new GetSecurityInfo();
            getsecurityinfo.uid = loginReq.uid;
            getsecurityinfo.exch = exch;
            getsecurityinfo.token = token;
            string uri = "GetSecurityInfo";

            rClient.makeRequest(new NorenApiResponse<GetSecurityInfoResponse>(response), uri, getsecurityinfo.toJson(), getJKey);
            return true;
        }
        public bool SendAddMultiScripsToMW(OnResponse response, string watchlist, string scrips)
        {
            if (loginResp == null)
                return false;

            AddMultiScripsToMW addMultiScripsToMW = new AddMultiScripsToMW();
            addMultiScripsToMW.uid  = loginReq.uid;
            addMultiScripsToMW.wlname = watchlist;
            addMultiScripsToMW.scrips = scrips;
            string uri = "AddMultiScripsToMW";

            rClient.makeRequest(new NorenApiResponse<StandardResponse>(response), uri, addMultiScripsToMW.toJson(), getJKey);
            return true;
        }
        public bool SendDeleteMultiMWScrips(OnResponse response, string watchlist, string scrips)
        {
            if (loginResp == null)
                return false;

            AddMultiScripsToMW addMultiScripsToMW = new AddMultiScripsToMW();
            addMultiScripsToMW.uid = loginReq.uid;
            addMultiScripsToMW.wlname = watchlist;
            addMultiScripsToMW.scrips = scrips;
            string uri = "DeleteMultiMWScrips";

            rClient.makeRequest(new NorenApiResponse<StandardResponse>(response), uri, addMultiScripsToMW.toJson(), getJKey);
            return true;
        }
        #region order methods
        public bool SendPlaceOrder(OnResponse response ,PlaceOrder order)
        {
            if (loginResp == null)
                return false;

            string uri = "PlaceOrder";

            rClient.makeRequest(new NorenApiResponse<PlaceOrderResponse>(response), uri, order.toJson(), getJKey);
            return true;
        }
        public bool SendModifyOrder(OnResponse response, ModifyOrder order)
        {
            if (loginResp == null)
                return false;

            order.uid = loginReq.uid;
            string uri = "ModifyOrder";

            rClient.makeRequest(new NorenApiResponse<ModifyOrderResponse>(response), uri, order.toJson(), getJKey);
            return true;            
        }
        public bool SendCancelOrder(OnResponse response, string norenordno)
        {
            if (loginResp == null)
                return false;

            string uri = "CancelOrder";
            CancelOrder cancelOrder = new CancelOrder();
            cancelOrder.norenordno = norenordno;
            cancelOrder.uid = loginReq.uid;

            rClient.makeRequest(new NorenApiResponse<CancelOrderResponse>(response), uri, cancelOrder.toJson(), getJKey);
            return true;
        }
        public bool SendExitSNOOrder(OnResponse response, string norenordno, string product)
        {
            if (loginResp == null)
                return false;

            string uri = "ExitSNOOrder";
            ExitSNOOrder exitSNOOrder = new ExitSNOOrder();
            exitSNOOrder.norenordno = norenordno;
            exitSNOOrder.prd = product;
            exitSNOOrder.uid = loginReq.uid;

            rClient.makeRequest(new NorenApiResponse<ExitSNOOrderResponse>(response), uri, exitSNOOrder.toJson(), getJKey);
            return true;
        }
        public bool SendGetOrderBook(OnResponse response, string product)
        {
            if (loginResp == null)
                return false;

            string uri = "OrderBook";
            OrderBook orderbook = new OrderBook();
            orderbook.uid = loginReq.uid;
            orderbook.prd = product;
            rClient.makeRequest(new NorenApiResponseList<OrderBookResponse, OrderBookItem>(response), uri, orderbook.toJson(), getJKey);
            return true;
        }

        public bool SendGetMultiLegOrderBook(OnResponse response, string product)
        {
            if (loginResp == null)
                return false;

            string uri = "MultiLegOrderBook";
            MultiLegOrderBook mlorderbook = new MultiLegOrderBook();
            mlorderbook.uid = loginReq.uid;
            mlorderbook.prd = product;

            rClient.makeRequest(new NorenApiResponseList<MultiLegOrderBookResponse, MultiLegOrderBookItem>(response), uri, mlorderbook.toJson(), getJKey);
            return true;
        }
        public bool SendGetTradeBook(OnResponse response, string account)
        {
            if (loginResp == null)
                return false;

            string uri = "TradeBook";
            TradeBook tradebook = new TradeBook();
            tradebook.uid = loginReq.uid;
            tradebook.actid = account;
            //tradebook.prd = product;            

            rClient.makeRequest(new NorenApiResponseList<TradeBookResponse, TradeBookItem>(response), uri, tradebook.toJson(), getJKey);
            return true;
        }
        public bool SendGetOrderHistory(OnResponse response, string norenordno)
        {
            if (loginResp == null)
                return false;

            string uri = "SingleOrdHist";
            SingleOrdHist orderhistory = new SingleOrdHist();
            orderhistory.uid = loginReq.uid;
            orderhistory.norenordno = norenordno;

            rClient.makeRequest(new NorenApiResponseList<OrderHistoryResponse, SingleOrdHistItem>(response), uri, orderhistory.toJson(), getJKey);
            return true;
        }
        public bool SendGetPositionBook(OnResponse response, string account)
        {
            if (loginResp == null)
                return false;

            string uri = "PositionBook";
            PositionBook positionBook = new PositionBook();
            positionBook.uid   = loginReq.uid;
            positionBook.actid = account;

            rClient.makeRequest(new NorenApiResponseList<PositionBookResponse, PositionBookItem>(response), uri, positionBook.toJson(), getJKey);
            return true;
        }
        public bool SendGetHoldings(OnResponse response, string account, string product)
        {
            if (loginResp == null)
                return false;

            string uri = "Holdings";
            Holdings holdings = new Holdings();
            holdings.uid = loginReq.uid;
            holdings.actid = account;
            holdings.prd = product;

            rClient.makeRequest(new NorenApiResponseList<HoldingsResponse, HoldingsItem>(response), uri, holdings.toJson(), getJKey);
            return true;
        }
        public bool SendGetLimits(OnResponse response, string account, string product = "", string segment = "", string exchange = "")
        {
            if (loginResp == null)
                return false;

            Limits limits = new Limits();
            limits.actid = account;
            limits.uid = loginReq.uid;
            if(product != "")
                limits.prd = product;
            if (segment != "")
                limits.seg = segment;
            if (exchange != "")
                limits.exch = exchange;
            string uri = "Limits";

            rClient.makeRequest(new NorenApiResponse<LimitsResponse>(response), uri, limits.toJson(), getJKey);
            return true;
        }

        public bool SendGetOrderMargin(OnResponse response, OrderMargin order)
        {
            if (loginResp == null)
                return false;
            string uri = "GetOrderMargin";
            rClient.makeRequest(new NorenApiResponse<GetOrderMarginResponse>(response), uri, order.toJson(), getJKey);
            return true;
        }

        public bool SendGetBasketMargin(OnResponse response, BasketMargin basket)
        {
            if (loginResp == null)
                return false;
            string uri = "GetBasketMargin";

            rClient.makeRequest(new NorenApiResponse<GetBasketMarginResponse>(response), uri, basket.toJson(), getJKey);
            return true;
        }

        #endregion
        #region others
        public bool SendGetExchMsg(OnResponse response, ExchMsg exchmsg)
        {
            if (loginResp == null)
                return false;

            string uri = "ExchMsg";

            rClient.makeRequest(new NorenApiResponseList<ExchMsgResponse, ExchMsgItem>(response), uri, exchmsg.toJson(), getJKey);
            return true;
        }
        #endregion
        #region feed methods
        public bool SendGetQuote(OnResponse response, string exch, string token)
        {
            if (loginResp == null)
                return false;
            string uri = "GetQuotes";

            Quote quote = new Quote();

            quote.uid = loginReq.uid;
            quote.exch = exch;
            quote.token = token;

            rClient.makeRequest(new NorenApiResponse<GetQuoteResponse>(response), uri, quote.toJson(), getJKey);
            return true;
        }

        public bool SendGetTPSeries(OnResponse response, string exch, string token, string starttime = null, string endtime = null)
        {
            if (loginResp == null)
                return false;
            string uri = "TPSeries";

            TPSeries quote = new TPSeries();

            quote.uid = loginReq.uid;
            quote.exch = exch;
            quote.token = token;
            if (String.IsNullOrEmpty(starttime) != true)
                quote.st = starttime;
            if(String.IsNullOrEmpty(endtime) != true)
                quote.et = endtime;

            rClient.makeRequest(new NorenApiResponseList<GetTPSeriesResponse, TPSeriesItem>(response), uri, quote.toJson(), getJKey);
            return true;
        }

        public bool SendGetIndexList(OnResponse response, string exch)
        {
            if (loginResp == null)
                return false;
            string uri = "GetIndexList";

            IndexList index = new IndexList();

            index.uid = loginReq.uid;
            index.exch = exch;

            rClient.makeRequest(new NorenApiResponse<GetIndexListResponse>(response), uri, index.toJson(), getJKey);
            return true;
        }

        public bool GetDailyTPSeries(OnResponse response, string endpoint, string exch, string token, string starttime, string endtime)
        {           
            return true;
        }


        public bool ConnectWatcher(string uri, OnFeed marketdataHandler, OnOrderFeed orderHandler)
        {
            var url = new Uri(uri);
            wsclient = new WebsocketClient(url);
            OnFeedCallback = marketdataHandler;
            OnOrderCallback = orderHandler;
            wsclient.ReconnectTimeout = TimeSpan.FromSeconds(30);
            wsclient.ReconnectionHappened.Subscribe(info =>
                Console.WriteLine($"Reconnection happened, type: {info.Type}"));

            ConnectMessage connect = new ConnectMessage();
            connect.t = "c";
            connect.uid = loginReq.uid;
            connect.actid = loginReq.uid;
            connect.susertoken = loginResp?.susertoken;

            wsclient.MessageReceived.Subscribe(msg => OnWSHandler(msg));
            wsclient.Start();
            wsclient.Send(connect.toJson());
            Console.WriteLine($"Add Watcher: {connect.toJson()}");
            return true;
        }
        public bool AddFeedDevice(string uri, OnFeed handler)
        {
            var url = new Uri(uri);
            wsclient = new WebsocketClient(url);

            OnFeedCallback = handler;

            wsclient.ReconnectTimeout = TimeSpan.FromSeconds(30);
            wsclient.ReconnectionHappened.Subscribe(info =>
                Console.WriteLine($"Reconnection happened, type: {info.Type}"));

            ConnectMessage connect = new ConnectMessage();
            connect.t = "c";
            connect.uid = loginReq.uid;
            connect.actid = loginReq.uid;
            connect.susertoken = loginResp?.susertoken;

            wsclient.MessageReceived.Subscribe(msg => OnWSHandler(msg));
            wsclient.Start();
            wsclient.Send(connect.toJson());
            Console.WriteLine($"Add Feed Device: {connect.toJson()}");
            return true;
        }
        /// <summary>
        /// Subscribes to the token of interest
        /// </summary>
        /// <param name="exch"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool SubscribeToken(string exch, string token)
        {
            SubsTouchline subs = new SubsTouchline();

            subs.k = exch + "|" + token;

            wsclient.Send(subs.toJson());
            Console.WriteLine($"Sub Token: {subs.toJson()}");
            return true;
        }


        public bool SubscribeOrders(OnOrderFeed orderFeed, string account)
        {            
            OrderSubscribeMessage orderSubscribe = new OrderSubscribeMessage();
            orderSubscribe.actid = account;
            wsclient.Send(orderSubscribe.toJson());

            Console.WriteLine($"Sub Order: {orderSubscribe.toJson()}");
            return true;
        }
        #endregion
        
    }
}
