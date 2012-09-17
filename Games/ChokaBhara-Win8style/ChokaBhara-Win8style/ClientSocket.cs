using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using Newtonsoft.Json;
using SocketIOClient;
using WebSocket4Net;
using WebSocketSharp;
using WebSocketSharp.Frame;
namespace ChowkaBaraWin8Style
{
    public partial class MainWindow
    {
        Socket ClientSocket = null;
        Client NodeSocket;
        byte[] RecvBuffer = null;
        string InitialMessage = null;
        string AckMessage = null;
        int WhoIAm = 0;
        int RoomID = 0;
        WebSocket4Net.WebSocket websocket;
        WebSocketSharp.WebSocket Websocket;
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
                /*
                //NodeSocket = new Client("http://54.251.6.123:8085");
                NodeSocket = new Client("http://"+ServerAddress+":"+ServerPort); // url to nodejs 
                NodeSocket.Opened += new EventHandler(NodeSocket_Opened);
                NodeSocket.Message += new EventHandler<MessageEventArgs>(NodeSocket_Message);
                NodeSocket.Error += new EventHandler<ErrorEventArgs>(NodeSocket_Error);
                NodeSocket.SocketConnectionClosed += new EventHandler(NodeSocket_SocketConnectionClosed);
                
                NodeSocket.Connect();
                

                
                websocket = new WebSocket4Net.WebSocket("ws://localhost:8080/");
                websocket.Opened += new EventHandler(websocket_Opened);
                websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
                websocket.Closed += new EventHandler(webSocket_SocketConnectionClosed);
                websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
                
                websocket.Open();
                
                Websocket = new WebSocketSharp.WebSocket("ws://localhost:8080/");
                Websocket.OnOpen += (sender, e) =>
                {
                    ServerConnectionStatus = (ushort)eServerConnectionStatus.ESTABLISHING;
                    Console.WriteLine("Socket opened");
                    if (ServerConnectionStatus == (ushort)eServerConnectionStatus.ESTABLISHING)
                    {
                        JSONObjects HandShakeMessage = new JSONObjects() 
                        { RoomID = RoomID, HandShake = true, KayiNo = 0, KayiMove = 0, WhoIAm = WhoIAm, ClientVersion = ClientVersion };
                        websocket.Send(HandShakeMessage.ToJsonString());
                        
                    }
                };
                Websocket.OnMessage += (sender, e) =>
                {
                    JSONObjects Message = JsonConvert.DeserializeObject<JSONObjects>(e.Data);
                    if (ServerConnectionStatus == (ushort)eServerConnectionStatus.ESTABLISHING)
                    {
                        RoomID = Message.RoomID;
                        WhoIAm = Message.WhoIAm;
                        Console.WriteLine(RoomID + ":" + WhoIAm);
                        ServerConnectionStatus = (ushort)eServerConnectionStatus.ESTABLISHED;
                        JSONObjects ACKMesssage = new JSONObjects() { RoomID = RoomID, HandShake = false, KayiNo = 0, KayiMove = 0, WhoIAm = WhoIAm, ClientVersion = ClientVersion, ClientMessage = "ACK" };
                        Websocket.Send(ACKMesssage.ToJsonString());
                    }
                    if (ServerConnectionStatus == (ushort)eServerConnectionStatus.ESTABLISHED)
                    {
                        if (Message.ServerMessage == "ACK")
                        {
                            ServerConnectionStatus = (ushort)eServerConnectionStatus.CONNECTED;
                            Console.WriteLine("Connected");
                        }
                    }
                };
                Websocket.OnError += (sender, e) =>
                {
                    ServerConnectionStatus = (ushort)eServerConnectionStatus.ERROR;
                    Console.WriteLine("Error:" + e.Message);
                };
                Websocket.OnClose += (sender, e) =>
                {
                    ServerConnectionStatus = (ushort)eServerConnectionStatus.DISCONNECTED;
                    Console.WriteLine("Socket Closed");
                };
                Websocket.Connect();
                */
                /*
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);
                ClientSocket.BeginConnect(ServerAddress, ServerPort, (IAsyncResult ar) =>
                    {
                        ServerConnectionStatus = (UInt16)eServerConnectionStatus.ESTABLISHING;
                        //HandShake(ClientSocket);
                        if (ClientSocket.Connected)
                        {
                            
                            ClientSocket.BeginReceive(RecvBuffer, 0, RecvBuffer.Length, 0, SocketReceive, null);
                            Connected = true;
                            Console.WriteLine("Conected to server");
                        }
                        else
                        {
                            Console.WriteLine("Failed to Conect to server" + ServerAddress);
                            ServerConnectionStatus = (UInt16)eServerConnectionStatus.ERROR;
                            ClientSocket = null;
                            Connected = false;
                        }
                    }, null);
                */
                
            }
            return Connected;
        }
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
    }
}
