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
        WebSocket websocket;
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
                */


                websocket = new WebSocket("ws://localhost:8080/");
                websocket.Opened += new EventHandler(NodeSocket_Opened);
                websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
                websocket.Closed += new EventHandler(NodeSocket_SocketConnectionClosed);
                websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
                websocket.Open();


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

        void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.WriteLine("Error:"+e.Exception.Message);
        }

        void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
        private void websocket_Opened(object sender, EventArgs e)
        {
            websocket.Send("Hello World!");
        }
        void NodeSocket_SocketConnectionClosed(object sender, EventArgs e)
        {
            ServerConnectionStatus = (ushort)eServerConnectionStatus.DISCONNECTED;
        }

        void NodeSocket_Error(object sender, ErrorEventArgs e)
        {
            ServerConnectionStatus = (ushort)eServerConnectionStatus.ERROR;
        }

        void NodeSocket_Message(object sender, MessageEventArgs ServerMessage)
        {
            if (ServerConnectionStatus == (ushort)eServerConnectionStatus.ESTABLISHING)
            {
                JSONObjects HandShakeMessage =  HandShake(NodeSocket);
            }
        }

        void NodeSocket_Opened(object sender, EventArgs e)
        {
            ServerConnectionStatus = (ushort)eServerConnectionStatus.ESTABLISHING;
            
        }
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
        private JSONObjects HandShake(Client ClientSocket)
        {
            JSONObjects HandShakeMesssage = new JSONObjects()
             { RoomID = 0, HandShake = true, KayiNo = 0, KayiMove = 0, WhoIAm = 0, ClientVersion = ClientVersion };
            
            return HandShakeMesssage;
        }
        private int Send(Socket ClientSocket,string Message)
        {
            return(ClientSocket.Send(Encoding.ASCII.GetBytes(Message)));
        }
    }
}
