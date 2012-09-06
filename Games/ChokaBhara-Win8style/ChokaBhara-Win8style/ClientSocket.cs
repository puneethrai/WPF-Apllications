using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using Newtonsoft.Json;
namespace ChokaBhara_Win8style
{
    public partial class MainWindow
    {
        Socket ClientSocket = null;
        byte[] RecvBuffer = null;
        string InitialMessage = null;
        string AckMessage = null;
        int WhoIAm = 0;
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
                JSONObjects JS = new JSONObjects() {RoomID = 0,HandShake = true,KayiNo = 0,KayiMove = 0,WhoIAm = 0,ClientVersion = System.Windows.Forms.Application.ProductVersion };
                string js = JS.ToJsonString();
                JS = JsonConvert.DeserializeObject<JSONObjects>(js);
                Console.WriteLine(JS.ClientVersion);
               
                
                
            }
            return Connected;
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
        private int Send(Socket ClientSocket,string Message)
        {
            return(ClientSocket.Send(Encoding.ASCII.GetBytes(Message)));
        }
    }
}
