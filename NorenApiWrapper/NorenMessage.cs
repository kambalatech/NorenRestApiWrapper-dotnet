using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
            string json = JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            return json;
        }
    }
    public class NorenMessage
    {
        public virtual string toJson()
        {
            string json = JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            string prefix = "jData=";
            return prefix + json;
        }
    }
    public class NorenListResponseMsg<T> : StandardResponse
    {
        public NorenListResponseMsg()
        {
            list = new List<T>();
        }

        public DataView dataView
        {
            get
            {
                DataTable dataTable = new DataTable(typeof(T).Name);

                //Get all the properties
                FieldInfo[] Props = typeof(T).GetFields();

                foreach (FieldInfo prop in Props)
                {
                    //Setting column names as Property names
                    dataTable.Columns.Add(prop.Name);
                }

                foreach (T item in list)
                {
                    var values = new object[Props.Length];

                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item);
                    }

                    dataTable.Rows.Add(values);
                }

                //put a breakpoint here and check datatable

                return dataTable.DefaultView;
            }
        }

        public List<T> list;
    }
    #region WebSocket Stream Messages
    public class NorenStreamMessage
    {
        public string t;
        public virtual string toJson()
        {
            string json = JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
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
    public class ScripItem
    {
        public string exch;
        public string tsym;
        public string token;
        public string pp;
        public string ti;
        public string ls;
    }

    public class SearchScripResponse : NorenResponseMsg
    {
        public List<ScripItem> values;
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
    public class ModifyOrder : NorenMessage
    {
        public string exch;
        public string norenordno;
        public string prctyp;
        public string prc;
        public string qty;
        public string tsym;
        public string ret;
        public string trgprc;
        public string uid;
        public string bpprc;
        public string blprc;
        public string trailprc;
    }
    public class ModifyOrderResponse : NorenResponseMsg
    {
        public string request_time;
        public string emsg;
        public string norenordno;
    }
    public class CancelOrder : NorenMessage
    {
        public string norenordno;
        public string uid;        
    }
    public class CancelOrderResponse : NorenResponseMsg
    {
        public string request_time;
        public string emsg;
        public string norenordno;
    }

    public class OrderBook : NorenMessage
    {
        public string uid;
        public string prd;
    }
    public class OrderBookItem 
    {
        public string exch;
        public string tsym;
        public string norenordno;
        public string prc;
        public string qty;
        public string prd;
        public string status;
        public string trantype;
        public string prctyp;
        public string fillshares;
        public string avgprc;
        public string rejreason;
        public string exchordid;
        public string cancelqty;
        public string remarks;
        public string dscqty;
        public string trgprc;
        public string ret;
        public string uid;
        public string actid;
        public string bpprc;
        public string blprc;
        public string trailprc;
        public string amo;
        public string pp;
        public string ti;
        public string ls;
        public string token;
        public string orddttm;
        public string ordenttm;
        public string extm;
        public string request_time;
        public string emsg;
    }
    public class OrderBookResponse : NorenListResponseMsg<OrderBookItem>
    {
        public List<OrderBookItem> Orders => list;      

    }
    public class MultiLegOrderBook : NorenMessage
    {
        public string uid;
        public string prd;
    }

    public class MultiLegOrderBookResponse : NorenListResponseMsg<MultiLegOrderBookItem>
    {
        public List<MultiLegOrderBookItem> mlorders =>list;
    }
    public class MultiLegOrderBookItem
    {
        public string exch;
        public string tsym;
        public string norenordno;
        public string prc;
        public string qty;
        public string prd;
        public string status;
        public string trantype;
        public string prctyp;
        public string fillshares;
        public string avgprc;
        public string rejreason;
        public string exchordid;
        public string cancelqty;
        public string remarks;
        public string dscqty;
        public string trgprc;
        public string ret;
        public string uid;
        public string actid;
        public string bpprc;
        public string blprc;
        public string trailprc;
        public string amo;
        public string pp;
        public string ti;
        public string ls;
        public string tsym2;
        public string trantype2;
        public string qty2;
        public string prc2;
        public string tsym3;
        public string trantype3;
        public string qty3;
        public string prc3;
        public string fillshares2;
        public string avgprc2;
        public string fillshares3;
        public string avgprc3;
        public string request_time;
        public string emsg;
    }

    public class TradeBook : NorenMessage
    {
        public string uid;
        public string prd;
    }
    

    public class TradeBookResponse : NorenListResponseMsg<TradeBookItem>
    {
        public List<TradeBookItem> trades =>list;
    }
    public class TradeBookItem
    {
        public string exch;
        public string tsym;
        public string norenordno;
        public string qty;
        public string prd;
        public string trantype;
        public string prctyp;
        public string fillshares;
        public string avgprc;
        public string exchordid;
        public string remarks;
        public string ret;
        public string uid;
        public string actid;
        public string pp;
        public string ti;
        public string ls;
        public string cstFrm;
        public string fldttm;
        public string flid;
        public string flleg;
        public string flqty;
        public string flprc;
        public string ordersource;
        public string token;
        public string request_time;
        public string emsg;
    }
    public class ExchMsg : NorenMessage
    {
        public string uid;
        public string exch;
    }   

    public class ExchMsgResponse : NorenListResponseMsg<ExchMsgItem>
    {
        public List<ExchMsgItem> messages => list;
    }
    public class ExchMsgItem
    {
        public string exchmsg;
        public string exchtm;
    }
    public class PositionBook : NorenMessage
    {
        public string uid;
        public string actid;
    }

    public class PositionBookResponse : NorenListResponseMsg<PositionBookItem>
    {
        public List<PositionBookItem> positions => list;
    }
    public class PositionBookItem
    {
        public string exch;
        public string tsym;
        public string token;
        public string uid;
        public string actid;
        public string prd;
        public string netqty;
        public string netavgprc;
        public string daybuyqty;
        public string daysellqty;
        public string daybuyavgprc;
        public string daysellavgprc;
        public string daybuyamt;
        public string daysellamt;
        public string cfbuyqty;
        public string cforgavgprc;
        public string cfsellqty;
        public string cfbuyavgprc;
        public string cfsellavgprc;
        public string cfbuyamt;
        public string cfsellamt;
        public string lp;
        public string rpnl;
        public string urmtom;
        public string bep;
        public string openbuyqty;
        public string opensellqty;
        public string openbuyamt;
        public string opensellamt;
        public string openbuyavgprc;
        public string opensellavgprc;
        public string mult;
        public string pp;
        public string prcftr;
        public string ti;
        public string ls;        
    }
    public class Holdings : NorenMessage
    {
        public string uid;
        public string actid;
        public string prd;
    }
    public class HoldingsItem
    {
        public List<ScripItem> exch_tsym;
        public string holdqty;
        public string colqty;
        public string btstqty;
        public string btstcolqty;
        public string usedqty;

    }
    public class HoldingsResponse : NorenListResponseMsg<HoldingsItem>
    {
        public List<HoldingsItem> holdings => list;
    }
    public class Limits : NorenMessage
    {
        public string uid;
        public string actid;
        public string prd;
        public string seg;
        public string exch;
    }
    public class LimitsResponse : StandardResponse
    {
        public string actid;
        public string prd;
        public string seg;
        public string exch;
        //-------------------------Cash Primary Fields-------------------------------
        public string cash;
        public string payin;
        public string payout;
        //-------------------------Cash Additional Fields-------------------------------
        public string brkcollamt;
        public string unclearedcash;
        public string daycash;
        //-------------------------Margin Utilized----------------------------------
        public string marginused;
        public string mtomcurper;
        //-------------------------Margin Used components---------------------
        public string cbu;
        public string csc;
        public string rpnl;
        public string unmtom;
        public string marprt;
        public string span;
        public string expo;
        public string premium;
        public string varelm;
        public string grexpo;
        public string greexpo_d;
        public string scripbskmar;
        public string addscripbskmrg;
        public string brokerage;
        public string collateral;
        public string grcoll;
        //-------------------------Additional Risk Limits---------------------------
        public string turnoverlmt;
        public string pendordvallmt;
        //-------------------------Additional Risk Indicators---------------------------
        public string turnover;
        public string pendordval;
        //-------------------------Margin used detailed breakup fields-------------------------
        public string rzpnl_e_i;
        public string rzpnl_e_m;
        public string rzpnl_e_c;
        public string rzpnl_d_i;
        public string rzpnl_d_m;
        public string rzpnl_f_i;
        public string rzpnl_f_m;
        public string rzpnl_c_i;
        public string rzpnl_c_m;
        public string uzpnl_e_i;
        public string uzpnl_e_m;
        public string uzpnl_e_c;
        public string uzpnl_d_i;
        public string uzpnl_d_m;
        public string uzpnl_f_i;
        public string uzpnl_f_m;
        public string uzpnl_c_i;
        public string uzpnl_c_m;
        public string span_d_i;
        public string span_d_m;
        public string span_f_i;
        public string span_f_m;
        public string span_c_i;
        public string span_c_m;
        public string expo_d_i;
        public string expo_d_m;
        public string expo_f_i;
        public string expo_f_m;
        public string expo_c_i;
        public string expo_c_m;
        public string premium_d_i;
        public string premium_d_m;
        public string premium_f_i;
        public string premium_f_m;
        public string premium_c_i;
        public string premium_c_m;
        public string varelm_e_i;
        public string varelm_e_m;
        public string varelm_e_c;
        public string marprt_e_h;
        public string marprt_e_b;
        public string marprt_d_h;
        public string marprt_d_b;
        public string marprt_f_h;
        public string marprt_f_b;
        public string marprt_c_h;
        public string marprt_c_b;
        public string scripbskmar_e_i;
        public string scripbskmar_e_m;
        public string scripbskmar_e_c;
        public string addscripbskmrg_d_i;
        public string addscripbskmrg_d_m;
        public string addscripbskmrg_f_i;
        public string addscripbskmrg_f_m;
        public string addscripbskmrg_c_i;
        public string addscripbskmrg_c_m;
        public string brkage_e_i;
        public string brkage_e_m;
        public string brkage_e_c;
        public string brkage_e_h;
        public string brkage_e_b;
        public string brkage_d_i;
        public string brkage_d_m;
        public string brkage_d_h;
        public string brkage_d_b;
        public string brkage_f_i;
        public string brkage_f_m;
        public string brkage_f_h;
        public string brkage_f_b;
        public string brkage_c_i;
        public string brkage_c_m;
        public string brkage_c_h;
        public string brkage_c_b;
    }


}
