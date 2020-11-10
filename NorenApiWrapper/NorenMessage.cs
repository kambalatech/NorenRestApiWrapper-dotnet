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
            Debug.WriteLine(json);
            return json;
        }
    }

    public class NorenMessage
    {
        public virtual string toJson()
        {
            string json = JsonConvert.SerializeObject(this);
            string prefix = "jData=";

            Console.WriteLine(prefix + json);
            Debug.WriteLine(prefix + json);
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
        public string jKey;
        public string uid;
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
            t = "t";
        }
        public string k;

    }

}
