using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
namespace WebSocket
{
    partial class WebSocketApp
    {
        /// <summary>
        /// Generates a Unique No. for a Room
        /// </summary>
        public int GenerateRoomNo()
        {
            int RoomID = 0;
            bool IsRoomID = true,flag=false;
            Random rand = new Random();
            while (IsRoomID)
            {
                RoomID = rand.Next(0, 1000);
                foreach (var pair in RoomKey)
                {
                    if (pair.Value == RoomID)
                        flag = true;
 
                }
                if (flag)
                    IsRoomID = true;
                else
                    IsRoomID = false;
                flag = false;
            }
            return RoomID;
        }
        /// <summary>
        /// Returns Room No. Associated with the given Socket
        /// </summary>
        public int GetRoomInfo(Socket Key)
        {
            int i=-1;
            /*
            foreach (var pair in SocketKey)
            {
                if (Key == pair.Key)
                {
                    i = pair.Value;
                    break;
                }

            }*/
            i = SocketKey[Key];
            return i;

        }
        /// <summary>
        /// Broadcasts a given message to every one except the one who created it.
        /// </summary>
        public void SendToAllExceptOne(string MessageData, Socket ExceptSocket, int SendRoom)
        {
            foreach (var pair in SocketKey)
            {
                if (pair.Value == SendRoom)
                {
                    if (null != pair.Key)
                    {
                        if (pair.Key != ExceptSocket)
                        {
                            if (!ChowkaWebSocket[ExceptSocket])
                                pair.Key.Send(Send(MessageData));
                            else
                            {
                                JSONObjects SendMessage = new JSONObjects() {ServerMessage = MessageData };
                                pair.Key.Send(Send(SendMessage.ToJsonString()));
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Broadcasts a given message to every one except the one who created it & removes the socket.
        /// </summary>
        public void SendToAllExceptOne(string MessageData, Socket ExceptSocket, int SendRoom, bool remove)
        {
            if (remove)
            {
                foreach (var pair in SocketKey)
                {
                    if (pair.Value == SendRoom)
                    {
                        if (null != pair.Key)
                        {
                            if (pair.Key != ExceptSocket)
                            {
                                if (!ChowkaWebSocket[ExceptSocket])
                                    pair.Key.Send(Send(MessageData));
                                else
                                {
                                    JSONObjects SendMessage = new JSONObjects() { ServerMessage = MessageData };
                                    pair.Key.Send(Send(SendMessage.ToJsonString()));
                                }

                            }
                        }

                    }
                }
                SocketKey.Remove(ExceptSocket);
                PingKey.Remove(ExceptSocket);
                Console.WriteLine("connection close with:" + ExceptSocket.RemoteEndPoint);
                //ExceptSocket.Close();
                
            }
        }
        /// <summary>
        /// Sends a ping frame
        /// </summary>
        public void PingFrame(Socket PingEndPoint)
        {
            byte[] sendclient = new byte[2];
            sendclient[0] = 0x89;
            sendclient[1] = 0x00;
            PingEndPoint.Send(sendclient);
        }
        /// <summary>
        /// Sends a pong frame
        /// </summary>
        /// <param name="PongEndPoint">Pong End point</param>
        public void PongFrame(Socket PongEndPoint)
        {
            byte[] sendclient = new byte[2];
            sendclient[0] = 0x8A;
            sendclient[1] = 0x00;
            PongEndPoint.Send(sendclient);
        }
        
    }
}
