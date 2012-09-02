using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
namespace ChokaBhara_Win8style
{
    public partial class MainWindow
    {
        Socket ClientSocket = null;
        byte[] RecvBuffer = null;
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
                        
                        if (ClientSocket.Connected)
                        {
                            ServerConnectionStatus = (UInt16)eServerConnectionStatus.CONNECTED;
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
                
                
            }
            return Connected;
        }
        private void SocketReceive(IAsyncResult ar)
        {
            string sRecvBuffer = Encoding.ASCII.GetString(RecvBuffer);
        }
    }
}
