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

namespace ChokaBhara_Server
{
    partial class Server
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
            Socket ConnectionSocket = (Socket)ar.AsyncState;
            string DataBuffer = Encoding.ASCII.GetString(dataBuffer);

            ConnectionSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, 0, Read, ConnectionSocket);
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
        public void Send(Socket SendSocket,string SendData)
        {
            JSONObjects JS = new JSONObjects { ClientVersion = Application.ProductName, HandShake = true, };
            SendSocket.Send(Encoding.ASCII.GetBytes(SendData));
        }
        /// <summary>
        /// Converts the given string to byte array of websocket protocol
        /// </summary>
        public byte[] Send(string SendData)
        {
            JSONObjects JS = new JSONObjects { ClientVersion = Application.ProductName, HandShake = true, };
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
