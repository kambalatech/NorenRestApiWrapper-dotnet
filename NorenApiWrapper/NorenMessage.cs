using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorenRestApiWrapper
{    
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

    public class NorenWSMessage
    {
        public string t;
        public virtual string toJson()
        {
            string json = JsonConvert.SerializeObject(this);
            Debug.WriteLine(json);
            return json;
        }
    }

    public class NorenFeed : NorenWSMessage
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

    public class LoginRespMessage: NorenResponseMsg
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

    public class LogoutRespMessage : NorenResponseMsg
    {
        public string request_time;
        
        public string emsg;
    }
    public class UserDetailsMessage : NorenMessage
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

    public class UserDetailsRespMessage : NorenResponseMsg
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
    public class ConnectMessage : NorenWSMessage
    {
        
        public string uid;
        public string actid;
        public string susertoken;      
    }

    public class SubsTouchline : NorenWSMessage
    {
        public SubsTouchline()
        {
            t = "d";
        }
        public string k;

    }

}
