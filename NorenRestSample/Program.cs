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
        public static void OnResponseNOP(NorenResponseMsg Response, bool ok)
        {

        }
        public static void OnResponse(NorenResponseMsg Response, bool ok)
        {
            LoginRespMessage loginResp = Response as LoginRespMessage;
            //Console.WriteLine(loginResp.toJson());
            
            nApi.SendGetUserDetails(Program.OnResponseNOP);
            //Console.WriteLine("Logout Exit..");
        }
        static NorenRestApi nApi = new NorenRestApi();
        static void Main(string[] args)
        {
           
            string endPoint = "http://kurma.kambala.co.in:9957/NorenWClient/";

            LoginMessage loginMessage = new LoginMessage();
            loginMessage.apkversion = "1.0.0";
            loginMessage.uid = "GURURAJ";
            loginMessage.pwd = "378145cba3272104505a3c776d10a1b64947742e3d4966831cb4c8746015acac";
            loginMessage.factor2 = "AAAAA1234A";
            loginMessage.imei = "134243434";
            loginMessage.source = "MOB";

            nApi.SendLogin(Program.OnResponse, endPoint, loginMessage);

            Console.Read();
            string feedws = "ws://kurma.kambala.co.in:9655/NorenStream/NorenWS";
            

            nApi.AddFeedDevice(feedws);

            nApi.SubscribeToken("NSE", "NIFTY");
            nApi.SubscribeToken("NSE", "22");
            
            
            
            Console.Read();
        }
    }
}
