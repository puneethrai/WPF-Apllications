using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using WebSocketClient;
namespace ChowkaBaraWin8Style
{
    public partial class MainWindow
    {
        Socket ClientSocket = null;
        
        byte[] RecvBuffer = null;
        string InitialMessage = null;
        string AckMessage = null;
        byte WhoIAm = 0;
        int RoomID = 0;
        WebSocket ws= null;
        JSONObjects SendObject = null;
        public UInt16 ServerConnectionStatus = (ushort)eServerConnectionStatus.STOPPED;
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
                    SendObject = Message;
                    this.Dispatcher.Invoke((Action)delegate()
                    {
                        ChatGrid.Visibility = Visibility.Visible;
                        StartAnimation(CreateAnimation(this, Window.WidthProperty, MainWindowWidth, MainWindowWidth + 200, 1000));
                        StartAnimation(CreateAnimation(ChatGrid, Grid.WidthProperty, ChatGrid.Width, ChatGrid.Width + 200, 1000));
                        StartAnimation(CreateAnimation(ChatBox, TextBox.WidthProperty, 0, 150, 1000));
                        TimeOutBarGrid.Width += 200; 
                    }, null);
                }
            }
            if (ServerConnectionStatus == (ushort)eServerConnectionStatus.CONNECTED)
            {
                if (Message.ChatMessage)
                {
                    this.Dispatcher.Invoke((Action)delegate()
                    {
                        ChatLabel.Items.Add(Message.ClientMessage);
                    }, null);
                }
                if (Message.ServerMessage == "START")
                {
                    StartPlay = true;
                    MaxPlayer = MaxKayi;
                    if (WhoIAm != 1)
                        this.Dispatcher.Invoke((Action)delegate()
                        {
                            WaitingGIF.Play();
                        }, null);
                    if (WhoIAm == 1)
                    {
                        Message.ClientMessage = "STARTED";
                        ws.Send(Message.ToJsonString());
                        isMoved = false;
                        ThreadStart start = new ThreadStart(TimerStart);
                        TimerThread = new Thread(start);
                        TimerThread.Name = "Timer Thread";
                        TimerThread.Start();
                        PlayerStatus = ePlayerStatus.HisTurn;
                    }
                    PlayStatus = ePlayStatus.PLAYING;
                }
                else if (PlayStatus == ePlayStatus.PLAYING)
                {
                    this.Dispatcher.Invoke((Action)delegate()
                    {
                        if (Message.TurnState == WhoIAm - 1)
                        {
                            isMoved = false;
                            ThreadStart start = new ThreadStart(TimerStart);
                            TimerThread = new Thread(start);
                            TimerThread.Name = "Timer Thread";
                            TimerThread.Start();
                            PlayerStatus = ePlayerStatus.HisTurn;
                            WaitingGIF.Visibility = Visibility.Hidden;
                            WaitingGIF.Stop();
                        }
                        else
                        {
                            PlayerStatus = ePlayerStatus.OthersTurn;
                            /*
                            if (Message.ClientMessage == "TURN")
                            {
                                TurnState = Message.TurnState;
                                Turn();
                            }*/
                            WaitingGIF.Visibility = Visibility.Visible;
                            WaitingGIF.Play();
                            if (Message.ClientMessage == "KAYIMOVE")
                                LetsMoveKayi(Message.KayiMove, Message.KayiNo);
                            
                        }
                    }, null);
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
                ws.Send(HandShakeMessage.ToJsonString());
                Console.WriteLine(ws.State);
                
            }
        }

        void ws_Error(object sender, WebSocketClient.ErrorEventArgs e)
        {
            ServerConnectionStatus = (ushort)eServerConnectionStatus.ERROR;
            Console.WriteLine("Error:" + e.Message);
            if (ServerConnectionStatus == (ushort)eServerConnectionStatus.ERROR)
            {
                if (GoOffLine.Dispatcher.CheckAccess())
                {
                    GoOffLine.IsEnabled = true;
                    GoOnLine.IsChecked = false;
                }
                else
                    GoOffLine.Dispatcher.Invoke((Action)delegate()
                    {
                        GoOffLine.IsEnabled = true;
                        GoOnLine.IsChecked = false;
                    }, null);
                StartPlay = false;
            }
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
