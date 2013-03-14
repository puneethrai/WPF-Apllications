using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
namespace WebSocketServer
{
    public class WebSocket
    {
        private const Int16 defaultPort = 8080;
        private const Int16 defaultPingDuration = 5;
        private IPAddress serverIPAddress = null;
        private Socket serverSocket = null;
        private Int16 serverPort = defaultPort;
        private Int16 gpingDuration = defaultPingDuration;//in seconds
        private eWebSocketServerState State = eWebSocketServerState.DISCONNECTED;
        private struct ThreadHandler
        {
            public Thread ThreadHandle;
            public Socket ClientSocket;
            public byte[] FirstByte;
        };
        #region PrivateEventHandlers
        private EventHandler<MessageReceivedEventArgs> m_MessageReceived;
        private EventHandler m_Opened;
        private EventHandler<ErrorEventArgs> m_Error;
        private EventHandler<DebugMessages> m_Debug;
        private EventHandler<WebSocketClose> m_Closed;
        private EventHandler<NewUser> m_NewUser;
        private EventHandler<UserLeft> m_UserLeft;
        
        private void OnError(ErrorEventArgs e)
        {
            if (m_Error == null)
                return;

            m_Error(this, e);
        }

        private void OnError(Exception e)
        {
            OnError(new ErrorEventArgs(e));
            this.State = eWebSocketServerState.ERROR;
        }
        private void onOpened()
        {
            this.State = eWebSocketServerState.STARTED;
            if (m_Opened == null)
                return;
            m_Opened(this, new WebSocketOpened());
            //mStream.BeginRead(DataBuffer, 0, DataBuffer.Length, RecvData, null);
            
        }
        private void DebugMessage(string Message)
        {
            if (m_Debug == null)
                return;
            m_Debug(this, new DebugMessages(Message));
        }
        private void onNewConection(Socket userSocket,string ExtraField)
        {
            if (m_NewUser == null)
                return;
            m_NewUser(this, new NewUser(userSocket,ExtraField));
        }
        private void onConnectionRemove(Socket userSocket)
        {
            if (m_UserLeft == null)
                return;
            m_UserLeft(this, new UserLeft(userSocket));
        }
        private void onClose(string Reason)
        {
            if (m_Closed == null)
                return;
            m_Closed(this, new WebSocketClose(Reason));

        }
        private void onMessage(string message,eWebSocketOpcode OpCode, Socket messageFrom)
        {
            if (m_MessageReceived == null)
                return;
            m_MessageReceived(this,new MessageReceivedEventArgs(message,OpCode,messageFrom));
        }
        #endregion

        public enum eWebSocketOpcode { CONTINUATION, TEXT, BINARY, NCRESERVED, CLOSE = 8, PING, PONG, CRESERVED, INVALID };
        public enum eWebSocketServerState { STARTING, STARTED, ERROR, DISCONNECTING, DISCONNECTED };
        public enum eWebSocketServerSetValue { PORT };

        #region PublicEventHandlers
        public event EventHandler<ErrorEventArgs> onError
        {
            add { m_Error += value; }
            remove { m_Error -= value; }
        }

        public event EventHandler onOpen
        {
            add { m_Opened += value; }
            remove { m_Opened -= value; }
        }
        public event EventHandler<MessageReceivedEventArgs> onMessageReceived
        {
            add { m_MessageReceived += value; }
            remove { m_MessageReceived -= value; }
        }
        public event EventHandler<DebugMessages> onDebugMessage
        {
            add { m_Debug += value; }
            remove { m_Debug -= value; }
        }
        public event EventHandler<WebSocketClose> onClosed
        {
            add { m_Closed += value; }
            remove { m_Closed -= value; }
        }
        public event EventHandler<NewUser> onNewUser
        {
            add { m_NewUser += value; }
            remove { m_NewUser += value; }
        }
        public event EventHandler<UserLeft> onUserLeft
        {
            add { m_UserLeft += value; }
            remove { m_UserLeft -= value; }
        }
        #endregion

        public Dictionary<Socket, bool> authenticatedClient = new Dictionary<Socket, bool>();
        /// <summary>
        /// WebSocket Server constructor
        /// </summary>
        /// <param name="port">Port number on which server should listen</param>
        /// <param name="pingDuration">ping duration in sec (0 means no ping)</param>
        public WebSocket(Int16 port,Int16 pingDuration)
        {
            this.gpingDuration = pingDuration;
            this.serverPort = port;
            this.serverIPAddress = IPAddress.Any;
            this.State = eWebSocketServerState.STARTING;

        }
        /// <summary>
        /// WebSocket Server constructor
        /// </summary>
        /// <param name="serverAddress">IP Address the server should listen</param>
        /// <param name="port">Port number on which server should listen</param>
        /// <param name="pingDuration">ping duration in sec (0 means no ping)</param>
        public WebSocket(string serverAddress,Int16 port, Int16 pingDuration)
        {
            this.serverIPAddress = IPAddress.Parse(serverAddress);
            this.gpingDuration = pingDuration;
            this.serverPort = port;
            this.State = eWebSocketServerState.STARTING;
            
        }
        /// <summary>
        /// WebSocket Server constructor
        /// </summary>
        public WebSocket()
        {
            this.serverPort = defaultPort;
            this.gpingDuration = defaultPingDuration;
            this.State = eWebSocketServerState.STARTING;
        }
        /// <summary>
        /// Initializes the websocket server
        /// </summary>
        public void BeginInitialize()
        {
            if (this.State == eWebSocketServerState.STARTING || this.State >= eWebSocketServerState.DISCONNECTING)
            {
                try
                {
                    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    serverSocket.Bind(new IPEndPoint(this.serverIPAddress, this.serverPort));
                    serverSocket.Listen(128);
                    ListenForClients();
                    this.State = eWebSocketServerState.STARTED;
                    onOpened();
                }
                catch(Exception ex)
                {
                    OnError(ex);
                    DebugMessage("Error Occurred in Initialization check the error messsage ,try changing port");
                    Close();
                }
            }
        }
        public eWebSocketServerState GetServerStatus()
        {
            return this.State;
        }
        /// <summary>
        /// Sets given value
        /// </summary>
        /// <param name="set">Set type</param>
        /// <param name="value">Value to be set</param>
        /// <returns></returns>
        public bool Set(eWebSocketServerSetValue set, dynamic value)
        {
            if (set == eWebSocketServerSetValue.PORT)
                if (this.State != eWebSocketServerState.STARTED)
                {
                    this.serverPort = (Int16)value;
                    return true;
                }
            return false;
        }
        private const string m_NotOpenSendingMessage = "You must send data by websocket after websocket is opened!";
        /// <summary>
        /// Ensures websocketserver is started
        /// </summary>
        /// <returns></returns>
        private bool EnsureWebSocketOpen()
        {
            if (this.State != eWebSocketServerState.STARTED)
            {
                OnError(new Exception(m_NotOpenSendingMessage));
                return false;
            }

            return true;
        }
        /// <summary>
        /// Listens for incoming clients
        /// </summary>
        private void ListenForClients()
        {
            serverSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);

        }
        /// <summary>
        /// Performs neccessary operation when client connects to server
        /// </summary>
        /// <param name="result">Async callback value containing client socket</param>
        private void OnClientConnect(IAsyncResult result)
        {

            try
            {
                Socket client = null;
                if (serverSocket != null && serverSocket.IsBound)
                {
                    client = serverSocket.EndAccept(result);

                }
                if (client != null)
                {
                    /* Handshaking and managing ClientSocket */
                    HandShake(client);

                }
            }
            catch (SocketException exception)
            {
                DebugMessage("Exception Occured:" + exception.Message);
            }
            finally
            {
                if (serverSocket != null && serverSocket.IsBound)
                {
                    ListenForClients();
                }
            }

        }
        /// <summary>
        /// Sends intial websocket handshake message
        /// </summary>
        /// <param name="clientSocket">Socket with which to shake hands</param>
        private void HandShake(Socket clientSocket)
        {
            string ExtraField = "";
            using (var stream = new NetworkStream(clientSocket))
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            {
                string r = null;
                string clienthandshake = null;
                string[] ar1 = new string[3];
                string clientKey = null;
                while (r != "")
                {
                    r = reader.ReadLine();
                    Console.WriteLine(r);
                    //LogLine(r, ServerLogLevel.Verbose);
                    clienthandshake += r + "\n";
                    if (String.Compare(r, 0, "Sec-WebSocket-Key:", 0, "Sec-WebSocket-Key:".Length) == 0)
                    {
                        ar1 = r.Split(':');
                        ar1[1] = ar1[1].Trim();
                        clientKey = ar1[1];
                    }
                    if (String.Compare(r, 0, "GET", 0, "GET".Length) == 0)
                    {
                        ar1 = r.Split(' ');
                        ar1 = ar1[1].Split('?');
                        if(ar1.Length > 1)
                            ExtraField = ar1[1];
                    }
                }
                if (clientKey != null)
                {
                    // send handshake to the client
                    string handshakemessage = "HTTP/1.1 101 Switching Protocols\r\n" +
                        "Upgrade: websocket\r\n" +
                        "Connection: Upgrade\r\n" +
                        "Sec-WebSocket-Accept: " + ComputeWebSocketHandshakeSecurityHash09(clientKey) + "\r\n\r\n";
                    writer.Write(handshakemessage);
                    authenticatedClient[clientSocket] = true;
                }

            }
            if (authenticatedClient[clientSocket])
            {
                onNewConection(clientSocket, ExtraField);
                CreateClientThread(clientSocket);
                
            }

        }
        private void CreateClientThread(Socket clientSocket)
        {
            //Thread ClientThread = new Thread(new ParameterizedThreadStart(ReadThead));
            //Tuple<Thread, Socket> clientInfo = new Tuple<Thread, Socket>(ClientThread, clientSocket);
            ThreadHandler clientInfo = new ThreadHandler();
            clientInfo.ClientSocket = clientSocket;
            clientInfo.FirstByte = new byte[1];
            //clientInfo.ThreadHandle = ClientThread;
            clientSocket.BeginReceive(clientInfo.FirstByte, 0,1,0,ReadThead, clientInfo);
            //ClientThread.Start(clientInfo);
        }
        /// <summary>
        /// Generates a Sec-WebSocket-Accept server key
        /// </summary>
        private String ComputeWebSocketHandshakeSecurityHash09(String secWebSocketKey)
        {
            const String GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            String secWebSocketAccept = String.Empty;

            // 1. Combine the request Sec-WebSocket-Key with magic key.
            String ret = secWebSocketKey + GUID;

            // 2. Compute the SHA1 hash
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] sha1Hash = sha.ComputeHash(Encoding.UTF8.GetBytes(ret));

            // 3. Base64 encode the hash
            secWebSocketAccept = Convert.ToBase64String(sha1Hash);
            DebugMessage("Generated Sec-WebSocket-Accept: " + secWebSocketAccept);
            return secWebSocketAccept;
        }
        /// <summary>
        /// Reads the incoming websocket message
        /// </summary>
        private void Read(string incomingMessage,eWebSocketOpcode opCode,Socket clientSocket)
        {
            onMessage(incomingMessage,opCode,clientSocket);
        }
        private void ReadThead(IAsyncResult Clientobj)
        {
            ThreadHandler TH = (ThreadHandler)Clientobj.AsyncState;
            Tuple<Thread, Socket> Client = new Tuple<Thread, Socket>(TH.ThreadHandle, TH.ClientSocket);
            DebugMessage("Client Detail:" + Client.Item2.RemoteEndPoint.ToString());
            byte[] firstByte = TH.FirstByte,payload = new byte[1];
            byte opCode = (byte)(firstByte[0] & 0x0F);
            byte SocStatus = (byte)(firstByte[0] & 0xF0);
            SocStatus = (byte)(SocStatus >> 4);
            //Client closed forcebly 
            if (firstByte[0] == 0)
            {
                onConnectionRemove(Client.Item2);
                Client.Item2.Close(5);
                return;
            }
            Client.Item2.Receive(payload);
            byte payloadLength = (byte)((payload[0] & 0x40) | (payload[0] & 0x20) | (payload[0] & 0x10)
                | (payload[0] & 0x8) | (payload[0] & 0x4) | (payload[0] & 0x2) | (payload[0] & 0x1));
            bool isMask = (payload[0] & 0x80) == 0x80;
            DebugMessage("SocStatus:" + SocStatus);
            DebugMessage("opCode:" + opCode);
            DebugMessage("Payloadlength:"+payloadLength+" IsMask:"+isMask);
            ulong datalength = 0;
            if ((eWebSocketOpcode)opCode == eWebSocketOpcode.CLOSE)
            {
                onConnectionRemove(Client.Item2);
                SendCloseFrame(Client.Item2);
                Client.Item2.Close(5);
            }
            else
            {
                byte[] data = null;
                switch (payloadLength)
                {
                    case 126:
                        byte[] bytesUShort = new byte[2];
                        Client.Item2.Receive(bytesUShort);
                        if (bytesUShort != null)
                        {
                            datalength = BitConverter.ToUInt16(bytesUShort.Reverse().ToArray(), 0);
                        }
                        data = new byte[datalength + 1];
                        break;
                    case 127:
                        byte[] bytesULong = new byte[8];
                        Client.Item2.Receive(bytesULong);
                        if (bytesULong != null)
                        {
                            datalength = BitConverter.ToUInt16(bytesULong.Reverse().ToArray(), 0);
                        }
                        data = new byte[datalength + 1];
                        break;

                    default:
                        datalength = payloadLength;
                        data = new byte[payloadLength + 1];
                        break;
                }
                byte[] maskKeys = null;
                if (isMask)
                {
                    maskKeys = new byte[4];
                    DebugMessage("Mask bit is set");
                    Client.Item2.Receive(maskKeys);
                    DebugMessage("Masking Keys are:" + Encoding.UTF8.GetString(maskKeys));
                }
                Client.Item2.Receive(data);
                if (isMask)
                {
                    for (int i = 0; i < (int)datalength; i++)
                    {

                        data[i] = (byte)(data[i] ^ maskKeys[i % 4]);
                    }
                }
                Read(Encoding.UTF8.GetString(data), (eWebSocketOpcode)opCode, Client.Item2);
                CreateClientThread(Client.Item2);
            }
        }
        /// <summary>
        /// Sends a given string using websocket protocol
        /// </summary>
        /// <param name="WriteBuffer"></param>
        /// <param name="SendSocket">Socket to be sent</param>
        /// <returns>Boolean of send status</returns>
        public bool Send(string WriteBuffer,Socket SendSocket)
        {
            if (EnsureWebSocketOpen())
            {
                SendSocket.Send(WebSocketEncoder(WriteBuffer));
            }
            return false;
        }
        public byte[] WebSocketEncoder(string rawMessage)
        {
            try
            {
                byte[] binary = Encoding.ASCII.GetBytes(rawMessage);
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
        /// <summary>
        /// Sends a ping frame
        /// </summary>
        private void PingFrame(Socket PingEndPoint)
        {
            if (gpingDuration != 0)
            {
                byte[] sendclient = new byte[2];
                sendclient[0] = 0x89;
                sendclient[1] = 0x00;
                PingEndPoint.Send(sendclient);
            }
        }
        private void SendCloseFrame(Socket CloseEndPoint)
        {
            byte[] sendclient = new byte[2];
            sendclient[0] = 0x88;
            sendclient[1] = 0x00;
            CloseEndPoint.Send(sendclient);
        }
        /// <summary>
        /// Close the web socket server
        /// </summary>
        public void Close()
        {
            this.serverSocket.Close(5);
            this.serverSocket.Dispose();
            this.serverSocket = null;
            if (this.State == eWebSocketServerState.ERROR)
                onClose("Error Occurred");
            else
                onClose("Closed by Owner");
            this.State = eWebSocketServerState.DISCONNECTED;

            
        }
    }
}
