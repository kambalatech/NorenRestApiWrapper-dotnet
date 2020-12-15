using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NorenRestApiWrapper;


namespace NorenRestSample
{
    static class Program
    {
        #region uat  credentials
        public static string endPoint = "http://<url>/NorenWClient/"; 
        public static string uid = "MOBKUMAR";
        public static string pwd = "Demo@1234";
        public static string pan = "AAAAA1111D";

        public static string newpwd = "Demo@12345";
        public static string appkey = "12be8cef3b1758f5";
        #endregion

        #region dev  credentials
        //string endPoint = "http://kurma.kambala.co.in:9957/NorenWClient/";
        //public static string uid = "MOBKUMAR";
        //public static string pwd = "Qaws@34567";
        //public static string pan = "AAAA45AAAA";

        //public static string newpwd = "Qaws@45678";
        //public static string appkey = "12be8cef3b1758f5";
        #endregion
        public static void OnAppLoginResponse(NorenResponseMsg Response, bool ok)
        {
            //do all work here
            LoginResponse loginResp = Response as LoginResponse;

            if (loginResp.stat == "Not_Ok")
            {
                if (loginResp.emsg == "Invalid Input : Change Password" || loginResp.emsg == "Invalid Input : Password Expired")
                {
                    //
                    Changepwd changepwd = new Changepwd();
                    changepwd.uid = uid;
                    changepwd.oldpwd = pwd;
                    changepwd.pwd = newpwd;
                    nApi.Changepwd(Program.OnResponseNOP, changepwd);
                    //this will change pwd. restart to relogin with new pwd
                    return;
                }
                return;
            }
            nApi.SendGetSecurityInfo(Program.OnResponseNOP, "NSE", "22");
            //nApi.SendSearchScrip(Program.OnResponseNOP, "CDS", "USDINR");
            //nApi.SendGetOrderHistory(Program.OnOrderHistoryResponse, "20121400000007");
            //nApi.SendGetOrderBook(Program.OnOrderBookResponse, "");
            //nApi.SendGetPositionBook(Program.OnResponseNOP, "MOBKUMAR");
            return;
            PlaceOrder order = new PlaceOrder();
            order.uid = uid;
            order.actid = "MOBKUMAR";
            order.exch = "CDS";
            order.tsym = "USDINR27JAN21F";
            order.qty = "10";
            order.dscqty = "0";
            order.prc = "80.0025";
            order.prd = "M";
            order.trantype = "B";
            order.prctyp = "LMT";
            order.ret = "DAY";
            order.ordersource = "MOB";
            nApi.SendPlaceOrder(Program.OnResponseNOP, order);
            //
            return;
            ModifyOrder modifyOrder = new ModifyOrder();
            modifyOrder.norenordno = "20121400000010";
            modifyOrder.exch = "NSE";
            modifyOrder.tsym = "INFY-EQ";
            modifyOrder.prctyp = "SL-LMT";
            modifyOrder.qty = "2";
            modifyOrder.prc = "1250.20";
            modifyOrder.trgprc = "1250.00";
            nApi.SendModifyOrder(Program.OnResponseNOP, modifyOrder);
            nApi.SendGetOrderBook(Program.OnOrderBookResponse, "");
            

            
            //login is ok   
            //nApi.SendGetTradeBook(Program.OnTradeBookResponse, "MOBKUMAR");
            //send getsecurityinfo
            //

           

            return;
            // send get order book
            //nApi.SendGetOrderBook(Program.OnOrderBookResponse, "h");
            

            ///
            //nApi.SendGetHoldings(Program.OnHoldingsResponse, "MOBKUMAR", "C");

            
            //
            //nApi.SendGetTradeBook(Program.OnResponseNOP, "MOBKUMAR");
            
            
            
            //
            
            
            //get user details
            nApi.SendGetUserDetails(Program.OnUserDetailsResponse);
            //
            

            
            ///
            string account = "MOBKUMAR";
            nApi.SendGetLimits(Program.OnResponseNOP, account);

            

            //nApi.SendSearchScrip(Program.OnResponseNOP, "NSE", "INFY");
            //add the feed device
            string feedws = "ws://kurma.kambala.co.in:9655/NorenStream/NorenWS";
            nApi.onStreamConnectCallback = Program.OnStreamConnect;
            //nApi.AddFeedDevice(feedws, Program.OnFeed);
            
        }

        public static void OnStreamConnect(NorenStreamMessage msg)
        {
            nApi.SubscribeOrders(Program.OnOrderUpdate, uid);
            nApi.SubscribeToken("NSE", "22");
        }
        static NorenRestApi nApi = new NorenRestApi();
        
        static void Main(string[] args)
        {
            LoginMessage loginMessage = new LoginMessage();
            loginMessage.apkversion = "1.0.0";
            loginMessage.uid = uid;
            loginMessage.pwd = pwd;
            loginMessage.factor2 = pan;
            loginMessage.imei = "134243434";
            loginMessage.source = "MOB";
            loginMessage.appkey = appkey;
            nApi.SendLogin(Program.OnAppLoginResponse, endPoint, loginMessage);


            //dont do anything till we get a login response            
            Console.WriteLine("Press any key to logout.");
            Console.Read();

            nApi.SendLogout(Program.OnResponseNOP);

            bool dontexit = true;
            while(dontexit)
            { 
                var kp = Console.ReadKey();
                if (kp.Key == ConsoleKey.Q)
                    dontexit = false;
                Console.WriteLine("Press q to exit.");
            }            
        }
        public static void OnUserDetailsResponse(NorenResponseMsg Response, bool ok)
        {
            UserDetailsResponse userDetailsResponse = Response as UserDetailsResponse;
            Console.WriteLine(userDetailsResponse.toJson());
        }
        public static void OnResponseNOP(NorenResponseMsg Response, bool ok)
        {            
            Console.WriteLine("app handler :" + Response.toJson());
        }

        
        public static void OnHoldingsResponse(NorenResponseMsg Response, bool ok)
        {
            HoldingsResponse holdingsResponse = Response as HoldingsResponse;

            Console.WriteLine("Holdings Response:" + holdingsResponse.toJson());

            
            printDataView(holdingsResponse.dataView);
        }

        public static void printDataView(DataView dv)
        {
            string order;
            foreach (DataRow dataRow in dv.Table.Rows)
            {
                order = "order:";
                foreach (var item in dataRow.ItemArray)
                {
                    order += item + " ,";
                }
                Console.WriteLine(order);
            }
            Console.WriteLine();
        }
        public static void OnOrderHistoryResponse(NorenResponseMsg Response, bool ok)
        {
            OrderHistoryResponse orderhistory = Response as OrderHistoryResponse;

            if (orderhistory.list != null)
            {
                DataView dv = orderhistory.dataView;

                //    for (int i = 0; i < dv.Count; i++)
                printDataView(dv);
            }
            else
            {
                Console.WriteLine("app handler: no orders");
            }
        }
        public static void OnTradeBookResponse(NorenResponseMsg Response, bool ok)
        {
            TradeBookResponse orderBook = Response as TradeBookResponse;

            if (orderBook.trades != null)
            {
                DataView dv = orderBook.dataView;

                //    for (int i = 0; i < dv.Count; i++)
                printDataView(dv);
            }
            else
            {
                Console.WriteLine("app handler: no trades");
            }
        }
        public static void OnOrderBookResponse(NorenResponseMsg Response, bool ok)
        {
            OrderBookResponse orderBook = Response as OrderBookResponse;

            if(orderBook.Orders != null)
            { 
                DataView dv = orderBook.dataView;

            //    for (int i = 0; i < dv.Count; i++)
                printDataView(dv);
            }
            else
            {
                Console.WriteLine("app handler: no orders");
            }
        }
        public static void OnFeed(NorenFeed Feed)
        {
            NorenFeed feedmsg = Feed as NorenFeed;
            Console.WriteLine(Feed.toJson());
            if (feedmsg.t == "dk")
            {
                //acknowledgment
            }
            if (feedmsg.t == "df")
            {
                //feed
                Console.WriteLine($"Feed received: {Feed.toJson()}");
            }
        }
        public static void OnOrderUpdate(NorenOrderFeed Order)
        {
            Console.WriteLine(Order.toJson());
        }
    }
}
