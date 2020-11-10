using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorenRestApiWrapper;
namespace NorenRestSample
{
    class Program
    {
        public static void OnResponse(NorenResponseMsg Response, bool ok)
        {
            LoginRespMessage loginResp = Response as LoginRespMessage;
            Console.WriteLine(loginResp.toJson());
        }
        static void Main(string[] args)
        {
            NorenRestApi nApi = new NorenRestApi();
            string endPoint = "http://kurma.kambala.co.in:9957/NorenWClient/";

            LoginMessage loginMessage = new LoginMessage();
            loginMessage.apkversion = "1.0.0";
            loginMessage.uid = "GURURAJ";
            loginMessage.pwd = "3a17be5bc13d62ccea01c818ac243532d9de839fbcbaf4fa5a20b95ffdad804c";
            loginMessage.factor2 = "AAAAA1234A";
            loginMessage.imei = "134243434";
            loginMessage.source = "MOB";

            nApi.SendLogin(Program.OnResponse, endPoint, loginMessage);

            string feedws = "ws://kurma.kambala.co.in:9655/NorenStream/NorenWS";
            

            nApi.AddFeedDevice(feedws);

            nApi.SubscribeToken("NSE", "NIFTY");
            nApi.SubscribeToken("NSE", "22");
            //Console.Read();
            //Console.WriteLine("Sending Logout..");
            //nApi.SendLogout();
            Console.WriteLine("Logout Exit..");
            Console.Read();
        }
    }
}
