using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NorenRestApiWrapper;

namespace NorenRestSample
{    
    public static class Program
    {
        #region dev  credentials

        public const string endPoint     = "";
        public const string wsendpoint   = "";
        public const string uid          = "";
        public const string actid        = "";
        public const string pwd          = "";
        public const string factor2      = dob;
        public const string pan          = "";
        public const string dob          = "";
        public const string imei         = "";
        public const string vc           = "";
        

        public const string appkey       = "";
        public const string newpwd       = "";
        #endregion
        public static bool loggedin = false;

      
        public static void OnStreamConnect(NorenStreamMessage msg)
        {
            Program.loggedin = true;
            nApi.SubscribeOrders(Handlers.OnOrderUpdate, uid);
            nApi.SubscribeToken("NSE", "22");
            
        }
        public static NorenRestApi nApi = new NorenRestApi();
        
        static void Main(string[] args)
        {
            LoginMessage loginMessage = new LoginMessage();
            loginMessage.apkversion = "1.0.0";
            loginMessage.uid = uid;
            loginMessage.pwd = pwd;
            loginMessage.factor2 = factor2;
            loginMessage.imei = imei;
            loginMessage.vc = vc;
            loginMessage.source = "MOB";
            loginMessage.appkey = appkey;
            nApi.SendLogin(Handlers.OnAppLoginResponse, endPoint, loginMessage);

            nApi.SessionCloseCallback = Handlers.OnAppLogout;
            nApi.onStreamConnectCallback = Program.OnStreamConnect;

            while (Program.loggedin == false)
            {
                //dont do anything till we get a login response         
                Thread.Sleep(5);
            }          
            
            bool dontexit = true;
            while(dontexit)
            {                
                var input = Console.ReadLine();
                var opts = input.Split(' ');
                foreach (string opt in opts)
                {
                    switch (opt.ToUpper())
                    {
                        case "C":
                            // process argument...
                            ActionPlaceCOorder();
                            break;
                        case "T":
                            nApi.SendGetTradeBook(Handlers.OnTradeBookResponse, actid);
                            break;
                        case "O":
                            nApi.SendGetOrderBook(Handlers.OnOrderBookResponse, "");
                            break;
                        case "S":
                            string exch;
                            string token;
                            Console.WriteLine("Enter exch:");
                            exch = Console.ReadLine();
                            Console.WriteLine("Enter Token:");
                            token = Console.ReadLine();
                            nApi.SendGetSecurityInfo(Handlers.OnResponseNOP, exch, token);
                            break;
                        case "H":
                            //check order
                            Console.WriteLine("Enter OrderNo:");
                            var orderno = Console.ReadLine();
                            nApi.SendGetOrderHistory(Handlers.OnOrderHistoryResponse, orderno);
                            break;
                        case "Q":
                            nApi.SendLogout(Handlers.OnAppLogout);
                            dontexit = false;
                        return;
                        case "B":
                            ActionPlaceBuyorder();
                            break;
                        case "U":
                            //get user details
                            nApi.SendGetUserDetails(Handlers.OnUserDetailsResponse);
                            break;
                        case "G":
                            nApi.SendGetHoldings(Handlers.OnHoldingsResponse, actid, "C");
                            break;
                        case "L":
                            nApi.SendGetLimits(Handlers.OnResponseNOP, actid);
                            break;

                        case "W":
                            nApi.SendSearchScrip(Handlers.OnResponseNOP, "NSE", "INFY");
                            break;
                        case "P":
                            ProductConversion productConversion = new ProductConversion();
                            productConversion.actid = actid;
                            productConversion.exch = "NSE";
                            productConversion.ordersource = "MOB";
                            productConversion.prd = "C";
                            productConversion.prevprd = "I";
                            productConversion.qty = "1";
                            productConversion.trantype = "B";
                            productConversion.tsym = "YESBANK-EQ";
                            productConversion.uid = uid;
                            productConversion.postype = "Day";
                            nApi.SendProductConversion(Handlers.OnResponseNOP, productConversion);
                            break;
                        case "FP":                            
                            nApi.SendForgotPassword(Handlers.OnResponseNOP,endPoint, uid, pan, dob);
                            break;
                        default:
                            // do other stuff...
                            ActionOptions();
                            break;
                    }
                }
                

                //var kp = Console.ReadKey();
                //if (kp.Key == ConsoleKey.Q)
                //    dontexit = false;
                //Console.WriteLine("Press q to exit.");
            }            
        }
        


        #region actions
        public static void ActionPlaceCOorder()
        {
            //sample cover order
            PlaceOrder order = new PlaceOrder();
            order.uid = uid;
            order.actid = actid;
            order.exch = "NSE";
            order.tsym = "USDINR27JAN21F";
            order.qty = "10";
            order.dscqty = "0";
            order.prc = "76.0025";
            order.blprc = "74.0025";
            order.prd = "H";
            order.trantype = "B";
            order.prctyp = "LMT";
            order.ret = "DAY";
            order.ordersource = "MOB";

            nApi.SendPlaceOrder(Handlers.OnResponseNOP, order);
        }

        public static void ActionPlaceBuyorder()
        {
            //sample cover order
            PlaceOrder order = new PlaceOrder();
            order.uid = uid;
            order.actid = actid;
            order.exch = "NSE";
            order.tsym = "M&M-EQ";
            order.qty = "10";
            order.dscqty = "0";
            order.prc = "100.5";
            
            order.prd = "I";
            order.trantype = "B";
            order.prctyp = "LMT";
            order.ret = "DAY";
            order.ordersource = "MOB";

            nApi.SendPlaceOrder(Handlers.OnResponseNOP, order);
        }

        public static void ActionOptions()
        {
            Console.WriteLine("Q: logout.");
            Console.WriteLine("O: get OrderBook");
            Console.WriteLine("T: get TradeBook");
            Console.WriteLine("B: place a buy order");
            Console.WriteLine("C: place a cover order");
            Console.WriteLine("S: get security info");
            Console.WriteLine("H: get order history");
            Console.WriteLine("G: get holdings");
            Console.WriteLine("L: get limits");
            Console.WriteLine("W: search for scrips (min 3 chars)");
            Console.WriteLine("P: position convert");
            Console.WriteLine("U: get user details");
            
        }
        #endregion
    }

}
