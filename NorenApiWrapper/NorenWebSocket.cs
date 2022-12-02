using Newtonsoft.Json;
using System;

namespace NorenRestApiWrapper
{
    public class NorenWebSocket
    {
        WebSocket _ws = new WebSocket();
        string _uid;
        string _susertoken;
        string _endpoint;

        // Delegates for connection callbacks
        public OnStreamConnect onStreamConnectCallback;
        public OnCloseHandler onStreamCloseCallback;
        public OnErrorHandler onStreamErrorCallback;
        public OnReconnectHandler OnReconnect;
        public OnNoReconnectHandler OnNoReconnect;

        // Delegates for data callbacks
        public OnFeed OnFeedCallback;
        public OnOrderFeed OnOrderCallback;

        // A watchdog timer for monitoring the connection of ticker.
        private bool _isReconnect = true;
        private int _interval = 5;
        private int _retries = 50;
        private int _retryCount = 0;

        private System.Timers.Timer _timer;
        private int _timerTick = 5;

        #region initialize
        public NorenWebSocket()
        {            
            // Add handlers to events
            _ws.OnConnect += _onConnect;
            _ws.OnData += _onData;
            _ws.OnClose += _onClose;
            _ws.OnError += _onError;           
        }

        public void Start(string url, string uid, string susertoken, OnFeed marketdataHandler, OnOrderFeed orderHandler)
        {
            //member init
            _endpoint = url;
            _uid = uid;
            _susertoken = susertoken;
           
            //app initializers
            OnFeedCallback = marketdataHandler;
            OnOrderCallback = orderHandler;
            
            _ws.Connect(_endpoint);

            // initializing  watchdog timer
            _timer = new System.Timers.Timer();
            _timer.Elapsed += _onTimerTick;
            _timer.Interval = 1000; // checks connection every second
            _timer.Start();
        }


        private void ReStart()
        {
            _ws = new WebSocket();
            _ws.OnConnect += _onConnect;
            _ws.OnData += _onData;
            _ws.OnClose += _onClose;
            _ws.OnError += _onError;

            _ws.Connect(_endpoint);
        }

        public void Stop()
        {
            _ws.Close();
        }
        
        private void _onError(string Message)
        {
            Console.WriteLine($"Error websocket: {Message}");
            //if (Message == "Lost ")
                //_onTimerTick(null, null);

            onStreamErrorCallback?.Invoke(Message);
        }

        private void _onClose()
        {            
            Console.WriteLine("websocket closed");
            onStreamCloseCallback?.Invoke();
        }

        private void _onConnect()
        {
            //once websocket is connected, lets create a app session
            ConnectMessage connect = new ConnectMessage();
            connect.t = "c";
            connect.uid = _uid;
            connect.actid = _uid;
            connect.susertoken = _susertoken;
            _ws.Send(connect.toJson());
            Console.WriteLine($"Create Session: {connect.toJson()}");
        }

        /// <summary>
        /// Tells whether websocket is connected to server not.
        /// </summary>
        public bool IsConnected
        {
            get { return _ws.IsConnected(); }
        }

        private void _onTimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsConnected)
                return;
            // For each timer tick count is reduced. If count goes below 0 then reconnection is triggered.

            _timerTick--;
            if (_timerTick < 0)
            {
                _timer.Stop();
                if (_isReconnect)
                    Reconnect();
            }
            Console.WriteLine(_timerTick);
        }

        /// <summary>
        /// Reconnect WebSocket connection in case of failures
        /// </summary>
        private void Reconnect()
        {
            if (IsConnected)
                _ws.Close(true);

            if (_retryCount > _retries)
            {
                _ws.Close(true);
                DisableReconnect();
                OnNoReconnect?.Invoke();
            }
            else
            {
                OnReconnect?.Invoke();
                _retryCount += 1;
                _ws.Close(true);
                ReStart();
                _timerTick = (int)Math.Min(Math.Pow(2, _retryCount) * _interval, 60);
                Console.WriteLine("New interval " + _timerTick);
                _timer.Start();
            }
        }
        /// <summary>
        /// Disable WebSocket autreconnect.
        /// </summary>
        public void DisableReconnect()
        {
            _isReconnect = false;
            if (IsConnected)
                _timer.Stop();
            _timerTick = _interval;
        }
        #endregion
        public bool Send(string data)
        {
            if (_ws.IsConnected())
            { 
                _ws.Send(data);
                return true;
            }
            else
                Console.WriteLine($"send failed as websocket is not connected: {data}");
            return false;
        }
        public static T Deserialize<T>(byte[] data, int count) where T : class
        {
            string str = System.Text.Encoding.UTF8.GetString(data, 0, count);
            return JsonConvert.DeserializeObject<T>(str);
        }

        private void _onData(byte[] Data, int Count, string MessageType)
        {
            NorenStreamMessage wsmsg;
            try
            {
                if (Count == 0)
                    return;
                wsmsg = Deserialize<NorenStreamMessage>(Data, Count);

                if (wsmsg.t == "ck")
                {
                    Console.WriteLine("session established");
                    onStreamConnectCallback?.Invoke(wsmsg);
                }
                else if (wsmsg.t == "om" || wsmsg.t == "ok")
                {
                    NorenOrderFeed ordermsg = Deserialize<NorenOrderFeed>(Data, Count);
                    OnOrderCallback?.Invoke(ordermsg);
                }
                else
                {
                    NorenFeed feedmsg = Deserialize<NorenFeed>(Data, Count);
                    OnFeedCallback?.Invoke(feedmsg);
                }
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Error deserializing data {ex.ToString()}");
                onStreamErrorCallback?.Invoke(ex.ToString());
                return;
            }           
        }
    }

}
