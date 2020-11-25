using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NorenRestApiWrapper;


namespace NorenRestSample
{
    class Program
    {
        #region credentials
        public static string uid = "MOBKUMAR";
        public static string pwd = "Qaws@12345";
        public static string pan = "AAAA45AAAA";
        
        public static string newpwd = "Qaws@123456";
        #endregion
        public static void OnAppLoginResponse(NorenResponseMsg Response, bool ok)
        {
            //do all work here
            LoginResponse loginResp = Response as LoginResponse;

            if (loginResp.stat == "Not_Ok")
            {
                if (loginResp.emsg == "Invalid Input : Change Password")
                {
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
            //login is ok                
            ///nApi.SendGetUserDetails(Program.OnUserDetailsResponse);
            //
            // send get order book
            nApi.SendGetOrderBook(Program.OnOrderBookResponse, "h");
            PlaceOrder order = new PlaceOrder();
            order.uid = uid;
            order.actid = "ACCT1";
            order.exch = "NSE";
            order.tsym = "INFY-EQ";
            order.qty = "10";
            order.dscqty = "0";
            order.prc = "200000";
            order.prd = "M";
            order.trantype = "B";
            order.prctyp = "LMT";
            order.ret = "DAY";
            order.ordersource = "MOB";
            


           //// nApi.SendPlaceOrder(Program.OnResponseNOP, order);

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
            string endPoint = "http://kurma.kambala.co.in:9957/NorenWClient/";

            LoginMessage loginMessage = new LoginMessage();
            loginMessage.apkversion = "1.0.0";
            loginMessage.uid = uid;
            loginMessage.pwd = pwd;
            loginMessage.factor2 = pan;
            loginMessage.imei = "134243434";
            loginMessage.source = "MOB";

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
            
            Console.WriteLine(Response.toJson());
        }

        public static void OnOrderBookResponse(NorenResponseMsg Response, bool ok)
        {
            OrderBookResponse orderBook = Response as OrderBookResponse;
            
            Console.WriteLine(Response.toJson());
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
