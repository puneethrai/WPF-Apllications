using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using Newtonsoft.Json;
using SocketIOClient;
//using WebSocket4Net;
//using WebSocketSharp;
//using WebSocketSharp.Frame;
using WebSocketClient;
namespace ChowkaBaraWin8Style
{
    public partial class MainWindow
    {
        Socket ClientSocket = null;
        Client NodeSocket;
        byte[] RecvBuffer = null;
        string InitialMessage = null;
        string AckMessage = null;
        byte WhoIAm = 0;
        int RoomID = 0;
        WebSocket ws= null;

        public UInt16 ServerConnectionStatus = 0;
        public enum eServerConnectionStatus { STARTING, ESTABLISHING, ESTABLISHED, CONNECTED, DISCONNECTED, STOPPED, ERROR };
        public enum ePlayStatus {IDLE,PLAYING };
        public ePlayStatus PlayStatus;
        private bool ConnectToServer()
        {
            bool Connected = false;
            
            ServerConnectionStatus = (UInt16)eServerConnectionStatus.STARTING;
            if (RecvBuffer == null)
            {
                RecvBuffer = new byte[1024];
            }
            if (ClientSocket == null) 
            {
                
                ws = new WebSocket(new Uri("ws://localhost:8080/"));
                ws.Error += new EventHandler<WebSocketClient.ErrorEventArgs>(ws_Error);
                ws.Opened += new EventHandler(ws_Opened);
                ws.MessageReceived += new EventHandler<MessageReceivedEventArgs>(ws_MessageReceived);
                ws.DebugMessage += new EventHandler<DebugMessages>(ws_DebugMessage);
                ws.Closed += new EventHandler<WebSocketClose>(ws_Closed);
                ws.BeginConnect();
               
                
            }
            return Connected;
        }

        void ws_Closed(object sender, WebSocketClose e)
        {
            ServerConnectionStatus = (ushort)eServerConnectionStatus.STOPPED;
            Console.WriteLine("Connection Closed,Reason:"+e.Reason);
        }

        void ws_DebugMessage(object sender, DebugMessages e)
        {
            Console.WriteLine(e.Message);
        }

        void ws_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);

            JSONObjects Message = JsonConvert.DeserializeObject<JSONObjects>(e.Message);
            if (ServerConnectionStatus == (ushort)eServerConnectionStatus.ESTABLISHING)
            {
                RoomID = Message.RoomID;
                WhoIAm = Message.WhoIAm;
                Console.WriteLine(RoomID + ":" + WhoIAm);
                ServerConnectionStatus = (ushort)eServerConnectionStatus.ESTABLISHED;
                JSONObjects ACKMesssage = new JSONObjects() { RoomID = RoomID, HandShake = false, KayiNo = 0, KayiMove = 0, WhoIAm = WhoIAm, ClientVersion = ClientVersion, ClientMessage = "ACK" };
                ws.Send(ACKMesssage.ToJsonString());
            }
            if (ServerConnectionStatus == (ushort)eServerConnectionStatus.ESTABLISHED)
            {
                if (Message.ServerMessage == "ACK")
                {
                    ServerConnectionStatus = (ushort)eServerConnectionStatus.CONNECTED;
                    Console.WriteLine("Connected");
                    Display("Connected to Server",2000);
                    Display("Your Turn:"+WhoIAm);
                }
            }
            if (ServerConnectionStatus == (ushort)eServerConnectionStatus.CONNECTED)
            {
                if (Message.ServerMessage == "START")
                {
                    if (WhoIAm == 1)
                    {
                        Message.ClientMessage = "STARTED";
                        ws.Send(Message.ToJsonString());
                    }
                    PlayStatus = ePlayStatus.PLAYING;
                }
                else if (PlayStatus == ePlayStatus.PLAYING)
                {
                    if (Message.TurnState == WhoIAm - 1)
                    {

                    }
                }
            }
        }

        void ws_Opened(object sender, EventArgs e)
        {
            ServerConnectionStatus = (ushort)eServerConnectionStatus.ESTABLISHING;
            Console.WriteLine("Socket opened");
            if (ServerConnectionStatus == (ushort)eServerConnectionStatus.ESTABLISHING)
            {
                JSONObjects HandShakeMessage = HandShake();
                //ws.Send("Hey");
                ws.Send(HandShakeMessage.ToJsonString());
                Console.WriteLine(ws.State);
                
            }
        }

        void ws_Error(object sender, WebSocketClient.ErrorEventArgs e)
        {
            ServerConnectionStatus = (ushort)eServerConnectionStatus.ERROR;
            Console.WriteLine("Error:" + e.Message);
        }
        private JSONObjects HandShake()
        {
            JSONObjects HandShakeMesssage = new JSONObjects() { RoomID = RoomID, HandShake = true, KayiNo = 0, KayiMove = 0, WhoIAm = WhoIAm, ClientVersion = ClientVersion };

            return HandShakeMesssage;
        }
/*
        #region WebSocketOp

        void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            ServerConnectionStatus = (ushort)eServerConnectionStatus.ERROR;
            Console.WriteLine("Error:"+e.Exception.Message);
        }

        void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
            
            JSONObjects Message = JsonConvert.DeserializeObject<JSONObjects>(e.Message);
            if (ServerConnectionStatus == (ushort)eServerConnectionStatus.ESTABLISHING)
            {
                RoomID = Message.RoomID;
                WhoIAm = Message.WhoIAm;
                Console.WriteLine(RoomID+":"+WhoIAm);
                ServerConnectionStatus = (ushort)eServerConnectionStatus.ESTABLISHED;
                JSONObjects ACKMesssage = new JSONObjects() 
                { RoomID = RoomID, HandShake = false, KayiNo = 0, KayiMove = 0, WhoIAm = WhoIAm, ClientVersion = ClientVersion,ClientMessage ="ACK" };
                websocket.Send(ACKMesssage.ToJsonString());
            }
            if (ServerConnectionStatus == (ushort)eServerConnectionStatus.ESTABLISHED)
            {
                if(Message.ServerMessage == "ACK")
                {
                    ServerConnectionStatus = (ushort)eServerConnectionStatus.CONNECTED;
                    Console.WriteLine("Connected");
                }
            }
            
        }
        private void websocket_Opened(object sender, EventArgs e)
        {
            ServerConnectionStatus = (ushort)eServerConnectionStatus.ESTABLISHING;
            Console.WriteLine("Socket opened");
            if (ServerConnectionStatus == (ushort)eServerConnectionStatus.ESTABLISHING)
            {
                JSONObjects HandShakeMessage = HandShake(websocket);
                websocket.Send(HandShakeMessage.ToJsonString());
                Console.WriteLine(websocket.State);
                websocket.Send(HandShakeMessage.ToJsonString());               
            }
            
        }
        void webSocket_SocketConnectionClosed(object sender, EventArgs e)
        {
            ServerConnectionStatus = (ushort)eServerConnectionStatus.DISCONNECTED;
            Console.WriteLine("Socket Closed");
            //websocket.Open();
        }
        private JSONObjects HandShake(WebSocket4Net.WebSocket ClientSocket)
        {
            JSONObjects HandShakeMesssage = new JSONObjects() { RoomID = RoomID, HandShake = true, KayiNo = 0, KayiMove = 0, WhoIAm = WhoIAm, ClientVersion = ClientVersion };

            return HandShakeMesssage;
        }
        #endregion
        #region TCPSocket
        private void SocketReceive(IAsyncResult ar)
        {
            string sRecvBuffer = Encoding.ASCII.GetString(RecvBuffer);
            JSONObjects JS = new JSONObjects();
            JS = JsonConvert.DeserializeObject<JSONObjects>(sRecvBuffer);
            
        }
        private void HandShake(Socket ClientSocket)
        {
            byte[] HandShakeReceiveBuffer = new byte[1024];
            Send(ClientSocket, InitialMessage);
            ClientSocket.Receive(HandShakeReceiveBuffer);
            Send(ClientSocket,AckMessage);
        }
        
        private int Send(Socket ClientSocket,string Message)
        {
            return(ClientSocket.Send(Encoding.ASCII.GetBytes(Message)));
        }

        #endregion
*/
    }
}
