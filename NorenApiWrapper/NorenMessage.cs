using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorenRestApiWrapper
{

    #region basetypes
    public class NorenResponseMsg
    {//all resapi messages will be returned here
        public string stat;
        public virtual string toJson()
        {
            string json = JsonConvert.SerializeObject(this);

            return json;
        }
    }
    public class NorenMessage
    {
        public virtual string toJson()
        {
            string json = JsonConvert.SerializeObject(this);
            string prefix = "jData=";
            return prefix + json;
        }
    }
    #region WebSocket Stream Messages
    public class NorenStreamMessage
    {
        public string t;
        public virtual string toJson()
        {
            string json = JsonConvert.SerializeObject(this);
            Debug.WriteLine(json);
            return json;
        }
    }
    #endregion

    public class ConnectMessage : NorenStreamMessage
    {
        public string uid;
        public string actid;
        public string susertoken;
    }

    public class SubsTouchline : NorenStreamMessage
    {
        public SubsTouchline()
        {
            t = "d";
        }
        public string k;

    }

    public class OrderSubscribeMessage : NorenStreamMessage
    {
        public OrderSubscribeMessage()
        {
            t = "o";
        }
        public string actid;
    }
    public class NorenFeed : NorenStreamMessage
    {
        public string e;
        public string tk;
        public string pp;
        public string ts;
        public string ti;
        public string ls;
        public string lp;
        public string pc;
        public string v;
        public string o;
        public string h;
        public string l;
        public string c;
        public string ap;
    }

    public class NorenOrderFeed : NorenStreamMessage
    {
        public string norenordno;
        public string uid;
        public string actid;
        public string exch;
        public string tsym;
        public string qty;
        public string prc;
        public string prd;
        public string status;
        public string reporttype;
        public string trantype;
        public string prctyp;
        public string ret;
        public string fillshares;
        public string avgprc;
        public string rejreason;
        public string exchordid;
        public string cancelqty;
        public string remarks;
        public string dscqty;
        public string trgprc;
    }

#endregion
    public class LoginMessage : NorenMessage
    {
        public string apkversion;
        public string uid;
        public string pwd;
        public string factor2;
        public string imei;
        public string ip_address;
        public string source;
    }

    public class LoginResponse: NorenResponseMsg
    {
        public string request_time;
        public string susertoken;
        public string lastaccesstime;
        public string emsg;
    }

    public class LogoutMessage : NorenMessage
    {
        public string uid;
    }

    public class LogoutResponse : NorenResponseMsg
    {
        public string request_time;

        public string emsg;
    }
    public class UserDetails : NorenMessage
    {
        public string uid;
    }

    public class ProductInfo
    {
        //[{"prd":"C","s_prdt_ali":"CNC","exch":["NSE","BSE"]
        public string prd;
        public string s_prdt_ali;
        public List<string> exch;
    }

    public class UserDetailsResponse : NorenResponseMsg
    {
        public List<string> exarr;
        public List<string> orarr;
        public List<ProductInfo> prarr;
        public string brkname;
        public string brnchid;
        public string email;
        public string actid;
        public string uprev;
        public string request_time;
        public string emsg;
    }
    public class ForgotPassword : NorenMessage
    {
        public string uid;
        public string pan;
        public string dob;
    }
    public class StandardResponse : NorenResponseMsg
    {
        public string request_time;
        public string emsg;
    }

    public class Changepwd : NorenMessage
    {
        public string uid;
        public string oldpwd;
        public string pwd;
    }
    public class ChangepwdResponse : NorenResponseMsg
    {
        public string request_time;
        public string emsg;
        public string dmsg;
    }
    
    public class MWList : NorenMessage
    {      
        //no params
    }
    public class MWListResponse : NorenResponseMsg
    {
        public List<string> values;
        public string request_time;
        public string emsg;        
    }
    public class MarketWatch : NorenMessage
    {
        public string uid;
        public string wlname;
    }
    public class MarketWatchItem
    {
        public string exch;
        public string tsym;
        public string token;
        public string pp;
        public string ti;
        public string ls;
    }
    public class MarketWatchResponse : NorenResponseMsg
    {
        public List<MarketWatchItem> values;
        public string request_time;
        public string emsg;
    }
    public class SearchScrip : NorenMessage
    {
        public string uid;
        public string stext;
        public string exch;
    }
    public class SearchScripResponse : NorenResponseMsg
    {
        public List<string> values;
        public string request_time;
        public string emsg;
    }

    public class AddMultiScripsToMW : NorenMessage
    {
        public string uid;
        public string wlname;
        public string scrips;
    }

    public class PlaceOrder : NorenMessage
    {
        public string uid;
        public string actid;
        public string exch;
        public string tsym;
        public string qty;
        public string prc;
        public string trgprc;
        public string dscqty;
        public string prd;
        public string trantype;
        public string prctyp;
        public string ret;
        public string remarks;
        public string ordersource;
        public string bpprc;
        public string blprc;
        public string trailprc;
        public string amo;
        public string tsym2;
        public string trantype2;
        public string qty2;
        public string prc2;
        public string tsym3;
        public string trantype3;
        public string qty3;
        public string prc3;
    }
    public class PlaceOrderResponse : NorenResponseMsg
    {
        public string request_time;
        public string emsg;
        public string norenordno;
    }
}
