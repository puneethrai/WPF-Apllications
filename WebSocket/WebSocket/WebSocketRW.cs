using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace WebSocket
{
    public partial class WebSocketApp : Form
    {
        /// <summary>
        /// Gets the socket used for the connection
        /// </summary>
        public Socket ConnectionSocket { get; private set; }
        /// <summary>
        /// reads the incomming data and broadcast it to other user
        /// </summary>
        private void Read(IAsyncResult ar)
        {
            byte[] sendclient = new byte[2];
            byte SocStatus = 0;
            Socket ConnectionSocket = (Socket)ar.AsyncState;
            try
            {

                //Console.WriteLine("Hey I Read Something");
                if (ConnectionSocket.Connected)
                {
                    int sizeOfReceivedData = dataBuffer.Length;
                    //int sizeOfReceivedData = ConnectionSocket.EndReceive(ar);
                    byte payload = (byte)((dataBuffer[1] & 0x40) | (dataBuffer[1] & 0x20) | (dataBuffer[1] & 0x10) | (dataBuffer[1] & 0x8) | (dataBuffer[1] & 0x4) | (dataBuffer[1] & 0x2) | (dataBuffer[1] & 0x1));

                    SocStatus = (byte)(dataBuffer[0] & 0x0F);
                    bool mask = (dataBuffer[1] & 0x80) == 0x80;
                    byte[] data = new byte[BUFFER_SIZE], maskKeys = new byte[4];

                    int maskstart = 0, masklenght = 4;
                    /*
                    foreach (int ch in dataBuffer)
                    {
                        if (ch != 0)
                            Console.WriteLine("Value:" + ch);
                    }*/
                    switch (SocStatus)
                    {
                        case 0: Console.WriteLine("continuation frame");
                            break;
                        case 1: Console.WriteLine("text frame");
                            break;
                        case 2: Console.WriteLine("binary frame");
                            break;
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7: Console.WriteLine("reserved for further non-control frames");
                            break;
                        case 8: CurrentConectedUsers--;
                            Status.Text = "No of connection:" + CurrentConectedUsers;
                            Console.WriteLine("connection close with:" + ConnectionSocket.RemoteEndPoint);

                            int i = 0;
                            i = GetRoomInfo(ConnectionSocket);
                            if (-1 != i)
                            {
                                SendToAllExceptOne("User Disconnected:" + ConnectionSocket.RemoteEndPoint, ConnectionSocket, i, true);

                            }
                            break;
                        case 9: Console.WriteLine("Ping");
                            PongFrame(ConnectionSocket);
                            break;
                        case 10: //Console.WriteLine("Pong from:" + ConnectionSocket.RemoteEndPoint);
                            PingKey[ConnectionSocket] = true;
                            break;
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15: Console.WriteLine("reserved for further control frame");
                            break;
                        default: Console.WriteLine("Invalid Opcode:" + SocStatus);
                            break;
                    }
                    if ((8 != SocStatus) && (10 != SocStatus) && (9 != SocStatus))
                    {
                        Console.WriteLine("Payload Lenght:");
                        Console.WriteLine(payload);
                        Console.WriteLine("Data Size:");
                        Console.WriteLine(sizeOfReceivedData);
                        ulong length = 0;
                        switch (payload)
                        {
                            case 126:
                                maskstart = 4;
                                byte[] bytesUShort = new byte[2];
                                for (int i = 0; i <= 1; i++)
                                    bytesUShort[i] = dataBuffer[i + 2];
                                if (bytesUShort != null)
                                {
                                    length = BitConverter.ToUInt16(bytesUShort.Reverse().ToArray(), 0);
                                }
                                break;
                            case 127:
                                maskstart = 9;
                                byte[] bytesULong = new byte[8];
                                for (int i = 0; i <= 7; i++)
                                    bytesULong[i] = dataBuffer[i + 2];
                                if (bytesULong != null)
                                {
                                    length = BitConverter.ToUInt16(bytesULong.Reverse().ToArray(), 0);
                                }
                                break;

                            default:
                                maskstart = 2;
                                length = payload;
                                data = new byte[payload + 1];
                                break;
                        }

                        if (mask)
                        {
                            Console.WriteLine("mask bit is set ,searching for Masking key from position" + maskstart);
                            Console.WriteLine("Masking Keys are:");
                            for (int i = maskstart; i < (maskstart + masklenght); i++)
                            {
                                maskKeys[i - maskstart] = dataBuffer[i];
                                //Console.WriteLine(maskKeys[i - maskstart]);
                            }
                        }
                        Console.WriteLine("Lenght of Payload:" + length);
                        for (int i = 0; i < (int)length; i++)
                        {
                            if (mask)
                            {
                                data[i] = dataBuffer[(maskstart + masklenght) + i];
                                data[i] = (byte)(data[i] ^ maskKeys[i % 4]);
                            }
                            else
                                data[i] = dataBuffer[maskstart + i];
                            //Console.WriteLine("Decoded Data:" + Convert.ToChar(data[i]));
                        }
                        StringBuilder dataString1 = new StringBuilder();
                        dataString1.Append(Encoding.UTF8.GetString(data, 0, (int)length));
                        Console.WriteLine("Data Recevied:" + dataString1.ToString());
                        if (ChowkaWebSocket[ConnectionSocket])
                        {
                            HandleChowkaWebSocket(ConnectionSocket, dataString1.ToString());
                            
                        }
                        else
                        {
                            int key = GetRoomInfo(ConnectionSocket);
                            SendToAllExceptOne(dataString1.ToString(), ConnectionSocket, key);
                        }
                    }
                }
                else
                {
                    if(9 != SocStatus||10 != SocStatus)
                        SocStatus = 8;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occured:" + e.Message);

            }
            finally
            {
                if (8 != SocStatus)
                {
                    try
                    {
                        ConnectionSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, 0, Read, ConnectionSocket);
                    }
                    catch (Exception ex)
                    {
                        int i = GetRoomInfo(ConnectionSocket);
                        if (-1 != i)
                        {
                            SendToAllExceptOne("User Disconnected:" + ConnectionSocket.RemoteEndPoint, ConnectionSocket, i, true);

                        }
                    }
                }
            }
           
        }
        /// <summary>
        /// Displays the received message
        /// </summary>

        public void DataReceived(int size, String message)
        {
            Console.WriteLine("Data Received:" + message);
        }
        /// <summary>
        /// Listens for incomming data
        /// </summary>
        private void Listen(Socket ReadSocket)
        {
            ReadSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, 0, Read, null);
        }
        /// <summary>
        /// Converts the given string to byte array of websocket protocol
        /// </summary>
        public byte[] Send(string SendData)
        {
            try
            {
                byte[] binary = Encoding.ASCII.GetBytes(SendData);
                ulong headerLength = 2;
                byte[] data = binary;

                bool mask = false;
                byte[] maskKeys = null;

                if (mask)
                {
                    headerLength += 4;
                    data = (byte[])data.Clone();

                    Random random = new Random(Environment.TickCount);
                    maskKeys = new byte[4];
                    for (int i = 0; i < 4; ++i)
                    {
                        maskKeys[i] = (byte)random.Next(byte.MinValue, byte.MaxValue);
                    }
                    for (int i = 0; i < data.Length; ++i)
                    {
                        data[i] = (byte)(data[i] ^ maskKeys[i % 4]);
                    }
                }

                byte payload;
                if (data.Length >= 65536)
                {
                    headerLength += 8;
                    payload = 127;
                }
                else if (data.Length >= 126)
                {
                    headerLength += 2;
                    payload = 126;
                }
                else
                {
                    payload = (byte)data.Length;
                }

                byte[] header = new byte[headerLength];

                header[0] = 0x80 | 0x1;
                if (mask)
                {
                    header[1] = 0x80;
                }
                header[1] = (byte)(header[1] | payload & 0x40 | payload & 0x20 | payload & 0x10 | payload & 0x8 | payload & 0x4 | payload & 0x2 | payload & 0x1);

                if (payload == 126)
                {
                    byte[] lengthBytes = BitConverter.GetBytes((ushort)data.Length).Reverse().ToArray();
                    header[2] = lengthBytes[0];
                    header[3] = lengthBytes[1];

                    if (mask)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            header[i + 4] = maskKeys[i];
                        }
                    }
                }
                else if (payload == 127)
                {
                    byte[] lengthBytes = BitConverter.GetBytes((ulong)data.Length).Reverse().ToArray();
                    for (int i = 0; i < 8; ++i)
                    {
                        header[i + 2] = lengthBytes[i];
                    }
                    if (mask)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            header[i + 10] = maskKeys[i];
                        }
                    }
                }

                return header.Concat(data).ToArray();

            }
            catch (Exception ex)
            {
                Console.WriteLine("websocket transport protocol Send exception: " + ex.ToString());
            }

            return null;
        }
    }
}
