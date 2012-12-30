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

        private Socket serverSocket = null;
        private Int16 serverPort = defaultPort;
        private Int16 gpingDuration = defaultPingDuration;//in seconds
        private eWebSocketServerState State = eWebSocketServerState.DISCONNECTED;
        #region PrivateEventHandlers
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
        private void onClose(string Reason)
        {
            if (m_Closed == null)
                return;
            m_Closed(this, new WebSocketClose(Reason));

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
                    serverSocket.Bind(new IPEndPoint(IPAddress.Any, this.serverPort));
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
                Console.WriteLine("Exception Occured:" + exception.Message);
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
            using (var stream = new NetworkStream(clientSocket))
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            {
                string r = null;
                string clienthandshake = null;
                string[] ar1 = new string[2];
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
        }
        /// <summary>
        /// Generates a Sec-WebSocket-Accept server key
        /// </summary>
        public static String ComputeWebSocketHandshakeSecurityHash09(String secWebSocketKey)
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
            Console.WriteLine("Generated Sec-WebSocket-Accept: " + secWebSocketAccept);
            return secWebSocketAccept;
        }
        /// <summary>
        /// Reads the incoming websocket message
        /// </summary>
        private void Read(string incomingMessage)
        {

        }
        private void ReadThead()
        {
            
        }
        /// <summary>
        /// Sends a given string using websocket protocol
        /// </summary>
        /// <param name="WriteBuffer"></param>
        /// <returns>Boolean of send status</returns>
        public bool Send(string WriteBuffer)
        {
            if (EnsureWebSocketOpen())
            {
            }
            return false;
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
