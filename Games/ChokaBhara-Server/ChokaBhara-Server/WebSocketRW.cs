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
            JSONObjects JS = new JSONObjects{ClientVersion = Application.ProductName,HandShake = true,}
            SendSocket.Send(Encoding.ASCII.GetBytes(SendData));
        }
    }
}
