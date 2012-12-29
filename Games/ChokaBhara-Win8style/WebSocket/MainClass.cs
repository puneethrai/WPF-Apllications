using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Security.Cryptography;
namespace WebSocketServer
{
    public class WebSocket
    {
        private Socket serverSocket = null;
        Int16 gpingDuration = 5;//in seconds
        public Dictionary<Socket, bool> authenticatedClient = new Dictionary<Socket, bool>();
        /// <summary>
        /// WebSocket Server constructor
        /// </summary>
        /// <param name="port">Port number on which server should listen</param>
        /// <param name="pingDuration">ping duration in sec (0 means no ping)</param>
        public WebSocket(Int16 port,Int16 pingDuration)
        {
            gpingDuration = pingDuration;
            Init(port);
        }
        /// <summary>
        /// WebSocket Server constructor
        /// </summary>
        public WebSocket()
        {
            Init(8080);
        }
        /// <summary>
        /// Initializes the websocket server
        /// </summary>
        /// <param name="port"></param>
        private void Init(Int16 port)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            serverSocket.Listen(128);
            ListenForClients();
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
                string[] ar = new string[20];
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
                        "Sec-WebSocket-Accept: " + ComputeWebSocketHandshakeSecurityHash09(ar1[1]) + "\r\n\r\n";
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
        private void Read(string incomingMessage,)
        {
        }

        /// <summary>
        /// Sends a given string using websocket protocol
        /// </summary>
        /// <param name="WriteBuffer"></param>
        /// <returns></returns>
        public bool Send(string WriteBuffer)
        {

            return true;
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
    }
}
