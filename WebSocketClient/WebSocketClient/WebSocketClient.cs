using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Threading;
namespace WebSocketClient
{
    public class WebSocket
    {
        #region private
        private Uri mUrl;
        private TcpClient mClient;
        private NetworkStream mStream;
        private byte[] DataBuffer;
        private bool mHandshakeComplete;
        private Dictionary<string, string> mHeaders;
        private EventHandler<MessageReceivedEventArgs> m_MessageReceived;
        private EventHandler m_Opened;
        private EventHandler<ErrorEventArgs> m_Error;
        private EventHandler<DebugMessages> m_Debug;
        private EventHandler<WebSocketClose> m_Closed;
        private void OnError(ErrorEventArgs e)
        {
            if (m_Error == null)
                return;

            m_Error(this, e);
        }
        
        private void OnError(Exception e)
        {
            OnError(new ErrorEventArgs(e));
            State = eWebSocketClientState.ERROR;
        }
        private void onOpened()
        {
            if (m_Opened == null)
                return;
            m_Opened(this, new WebSocketOpened());
            mStream.BeginRead(DataBuffer, 0, DataBuffer.Length,RecvData, null);
            State = eWebSocketClientState.CONNECTED;
        }
        private void onDebugMessage(string Message)
        {
            if (m_Debug == null)
                return;
            m_Debug(this,new DebugMessages(Message));
        }
        private void onClose(string Reason)
        {
            if (m_Closed == null)
                return;
            m_Closed(this, new WebSocketClose(Reason));

        }
        private TcpClient CreateSocket(Uri url)
        {
            string scheme = url.Scheme;
            string host = url.DnsSafeHost;
            int port = url.Port;
            TcpClient Client = null;
            try
            {

                if (port <= 0)
                {
                    if (scheme.Equals("wss"))
                        port = 443;
                    else if (scheme.Equals("ws"))
                        port = 80;
                    else
                        throw new ArgumentException("Unsupported scheme");
                }

                if (scheme.Equals("wss"))
                    throw new Exception("SSL support not implemented yet");
                else
                    Client = new TcpClient(host, port);
                State = eWebSocketClientState.STARTED;
            }
            catch(Exception ex)
            {
                OnError(ex);
                Client = null;
            }
            return Client;
        }
        private const string m_NotOpenSendingMessage = "You must send data by websocket after websocket is opened!";
        private bool EnsureWebSocketOpen()
        {
            if (!mHandshakeComplete)
            {
                OnError(new Exception(m_NotOpenSendingMessage));
                return false;
            }

            return true;
        }
        private const string m_ServerClosedMessage = "Server closed the connection";
        /// <summary>
        /// Sends a ping frame
        /// </summary>
        /// <param name="PingEndPoint">Ping end point as <see cref="System.IO.Stream"/></param>
        private void PingFrame(Stream PingEndPoint)
        {
            byte[] SendPing = new byte[2];
            SendPing[0] = 0x89;
            SendPing[1] = 0x00;
            PingEndPoint.Write(SendPing,0,SendPing.Length);
        }
        /// <summary>
        /// Sends a pong frame
        /// </summary>
        /// <param name="PongEndPoint">Pong end point as <see cref="System.IO.Stream"/></param>
        private void PongFrame(Stream PongEndPoint)
        {
            byte[] SendPong = new byte[2];
            SendPong[0] = 0x8A;
            SendPong[1] = 0x00;
            PongEndPoint.Write(SendPong,0,SendPong.Length);
        }
        
        #endregion
        #region public
        public bool HandShaked { get { return this.mHandshakeComplete; } private set { value = this.mHandshakeComplete; } }
        public enum eWebSocketClientState {STARTING,STARTED,CONNECTED,ERROR,DISCONNECTING,DISCONNECTED};
        public eWebSocketClientState State;
        public enum eWebSocketOpcode {CONTINUATION,TEXT,BINARY,NCRESERVED,CLOSE=8,PING,PONG,CRESERVED,INVALID};
        public event EventHandler<ErrorEventArgs> Error
        {
            add { m_Error += value; }
            remove { m_Error -= value; }
        }

        public event EventHandler Opened
        {
            add { m_Opened += value; }
            remove { m_Opened -= value; }
        }
        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add { m_MessageReceived += value; }
            remove { m_MessageReceived -= value; }
        }
        public event EventHandler<DebugMessages> DebugMessage
        {
            add { m_Debug += value; }
            remove { m_Debug += value; }
        }
        public event EventHandler<WebSocketClose> Closed
        {
            add { m_Closed += value; }
            remove { m_Closed -= value; }
        }
        public void SetHeaders(Dictionary<string, string> headers)
        {
            mHeaders = headers;
        }
        /// <summary>
        /// Creates a new WebSocketClient
        /// </summary>
        /// <param name="url">WebSocket URL in <see cref="System.Uri"/>format</param>
        public WebSocket(Uri url)
        {
            mUrl = url;
                string protocol = mUrl.Scheme;
                DataBuffer = new byte[4084];
                if (!protocol.Equals("ws") && !protocol.Equals("wss"))
                    throw new ArgumentException("Unsupported protocol: " + protocol);
                State = eWebSocketClientState.STARTED;
        }
        /// <summary>
        /// Begins a asynchronous WebSocket connection to remote host connection
        /// </summary>
        public void BeginConnect()
        {
            Thread Connection = new Thread(new ThreadStart(Connect));
            Connection.Name = "WebSocket Connection Thread";
            Connection.Start();
        }
        /// <summary>
        /// Begins a WebSocket connection to remote host connection
        /// </summary>
        public void Connect()
        {
            try
            {
                string host = mUrl.DnsSafeHost;
                string path = mUrl.PathAndQuery;
                string origin = "http://" + host;

                mClient = CreateSocket(mUrl);
                mStream = mClient.GetStream();

                int port = ((IPEndPoint)mClient.Client.RemoteEndPoint).Port;
                if (port != 80)
                    host = host + ":" + port;

                StringBuilder extraHeaders = new StringBuilder();
                if (mHeaders != null)
                {
                    foreach (KeyValuePair<string, string> header in mHeaders)
                        extraHeaders.Append(header.Key + ": " + header.Value + "\r\n");
                }

                string request = "GET " + path + " HTTP/1.1\r\n" +
                                 "Upgrade: WebSocket\r\n" +
                                 "Connection: Upgrade\r\n" +
                                 "Host: " + host + "\r\n" +
                                 "Origin: " + origin + "\r\n" +
                                 "Sec-WebSocket-Key: " + GenerateWebSocketHandshakeSecurityHash09() + "\r\n" +
                                 extraHeaders.ToString() + "\r\n";
                byte[] sendBuffer = Encoding.UTF8.GetBytes(request);

                mStream.Write(sendBuffer, 0, sendBuffer.Length);

                StreamReader reader = new StreamReader(mStream);
                {
                    string header = reader.ReadLine();
                    if (!header.Equals("HTTP/1.1 101 Switching Protocols"))
                        throw new IOException("Invalid handshake response");

                    header = reader.ReadLine();
                    if (!header.Equals("Upgrade: websocket"))
                        throw new IOException("Invalid handshake response");

                    header = reader.ReadLine();
                    if (!header.Equals("Connection: Upgrade"))
                        throw new IOException("Invalid handshake response");
                }
                mHandshakeComplete = true;
            }
            catch(Exception ex)
            {
                OnError(ex);
            }
            if (mHandshakeComplete)
            {
                onOpened();
            }
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
                mStream.Write(header.Concat(data).ToArray(), 0, header.Concat(data).ToArray().Length);
                mStream.Flush();
                return header.Concat(data).ToArray();

            }
            catch (Exception ex)
            {
                Console.WriteLine("websocket transport protocol Send exception: " + ex.ToString());
                OnError(ex);
            }

            return null;
        }
        /// <summary>
        /// Asynchronous callback for BeginReader
        /// </summary>
        /// <param name="ar">AsyncResult</param>
        private void RecvData(IAsyncResult ar)
        {
            Recv();
        }
        /// <summary>
        /// Decodes the received message & triggers the message received event
        /// </summary>
        /// <returns>Returns decoded message in string </returns>
        private string Recv()
        {
            StringBuilder recvBuffer = new StringBuilder();
            eWebSocketOpcode Opcode = eWebSocketOpcode.CRESERVED;
            if (EnsureWebSocketOpen())
            {
                
                byte payload = (byte)((DataBuffer[1] & 0x40) | (DataBuffer[1] & 0x20) | (DataBuffer[1] & 0x10) | (DataBuffer[1] & 0x8) | (DataBuffer[1] & 0x4) | (DataBuffer[1] & 0x2) | (DataBuffer[1] & 0x1));
				byte SocStatus = (byte)(DataBuffer[0] & 0x0F);
				bool mask = (DataBuffer[1] & 0x80) == 0x80;
				byte[] data = null, maskKeys = new byte[4];
				int maskstart = 0, masklenght = 4;
				switch (SocStatus)
				{
                    case 0:Opcode = eWebSocketOpcode.CONTINUATION;
                        onDebugMessage("continuation frame");
                        break;
                    case 1: Opcode = eWebSocketOpcode.TEXT;
                        Console.WriteLine("text frame");
                        break;
                    case 2: Opcode = eWebSocketOpcode.BINARY;
                        onDebugMessage("binary frame");
                        break;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7: Opcode = eWebSocketOpcode.NCRESERVED;
                        onDebugMessage("reserved for further non-control frames");
                        break;
                    case 8: 
                        break;
                    case 9:Opcode = eWebSocketOpcode.PING;
                        onDebugMessage("Ping");
                        PongFrame(mStream);
                        break;
                    case 10: Opcode = eWebSocketOpcode.PONG;
                        onDebugMessage("Pong");
                        PingFrame(mStream);
                        break;
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15: Opcode = eWebSocketOpcode.CRESERVED;
                        onDebugMessage("reserved for further control frame");
                        break;
                    default: onDebugMessage("Invalid Opcode:" + SocStatus);
                        break;
                }
                if ((8 != SocStatus) && (10 != SocStatus) && (9 != SocStatus))
                {
                    onDebugMessage("Payload Lenght:");
                    onDebugMessage(""+payload);
                    ulong length = 0;
                    switch (payload)
                    {
                        case 126:
                            maskstart = 4;
                            byte[] bytesUShort = new byte[2];
                            for (int i = 0; i <= 1; i++)
                                bytesUShort[i] = DataBuffer[i + 2];
                            if (bytesUShort != null)
                            {
                                length = BitConverter.ToUInt16(bytesUShort.Reverse().ToArray(), 0);
                            }
                            break;
                        case 127:
                            maskstart = 9;
                            byte[] bytesULong = new byte[8];
                            for (int i = 0; i <= 7; i++)
                                bytesULong[i] = DataBuffer[i + 2];
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
                        onDebugMessage("mask bit is set ,searching for Masking key from position" + maskstart);
                        onDebugMessage("Masking Keys are:");
                        for (int i = maskstart; i < (maskstart + masklenght); i++)
                        {
                            maskKeys[i - maskstart] = DataBuffer[i];
                            onDebugMessage(""+maskKeys[i - maskstart]);
                        }
                    }
                    onDebugMessage("Lenght of data:" + length);
                    if (data == null)
                        data = new byte[length + 1];
                    
                    for (int i = 0; i < (int)length; i++)
                    {
                        if (mask)
                        {
                            data[i] = DataBuffer[(maskstart + masklenght) + i];
                            data[i] = (byte)(data[i] ^ maskKeys[i % 4]);
                        }
                        else
                            data[i] = DataBuffer[maskstart + i];
                        
                    }

                    recvBuffer.Append(Encoding.UTF8.GetString(data, 0, (int)length));
                    onDebugMessage("Data Recevied:" + recvBuffer.ToString());
                    FireMessageReceived(recvBuffer.ToString(), Opcode);
                }
                               
                
                if (Opcode != eWebSocketOpcode.CLOSE) 
                    mStream.BeginRead(DataBuffer, 0, DataBuffer.Length, RecvData, null);
                else if(Opcode == eWebSocketOpcode.CLOSE)
                {
                    Close(m_ServerClosedMessage);
                }


            }
            
            return recvBuffer.ToString();
        }

        private void Close(string reason)
        {
            State = eWebSocketClientState.DISCONNECTING;
            mStream.Dispose();
            mClient.Close();
            mStream = null;
            mClient = null;
            mHandshakeComplete = false;
            State = eWebSocketClientState.DISCONNECTED;
            onDebugMessage("Websocket connection closed"+Environment.NewLine+"Reason:"+reason);
        }
        public void Close()
        {
            State = eWebSocketClientState.DISCONNECTING;
            byte[] SendClose = new byte[2];
            SendClose[0] = 0x88;
            SendClose[1] = 0x00;
            mStream.Write(SendClose, 0, SendClose.Length);
            State = eWebSocketClientState.DISCONNECTED;
            onDebugMessage("Websocket connection closed" + Environment.NewLine + "Reason:Client Closed");

        }
        #endregion
        #region internal
        internal void FireMessageReceived(string message,eWebSocketOpcode Opcode)
        {
            if (m_MessageReceived == null)
                return;

            m_MessageReceived(this, new MessageReceivedEventArgs(message,Opcode));
        }
        internal string GenerateWebSocketHandshakeSecurityHash09()
        {
            byte[] SecWebSocketKey = new byte[16];
            string sSecWebSocketKey = null;
            Random NextRandByte = new Random();
            NextRandByte.NextBytes(SecWebSocketKey);
            sSecWebSocketKey = Convert.ToBase64String(SecWebSocketKey);
            return sSecWebSocketKey;
        }
        #endregion
    }
}


