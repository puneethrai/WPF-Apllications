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
        #region private members
        private string webSocketOrigin ;                            // location for the protocol handshake
        private string webSocketLocation;                           // location for the protocol handshake
        private Socket serverSocket;
        private int CurrentConectedUsers=0;
        public const int BUFFER_SIZE = 1000000;
        public const int bufferSize = 255;
        public Socket AcceptSocket = null;
        private enum WrapperBytes : byte { Start = 0, End = 255 };  // data passed between client and server are wrapped in start and end bytes according to the protocol (0x00, 0xFF)
                                                  
        public byte[] buffer = new byte[BUFFER_SIZE];
        private byte[] dataBuffer;                                  // buffer to hold the data we are reading
        private StringBuilder dataString;                           // holds the currently accumulated data
        public static Thread workerThread;
        public Thread[] clientThread;
        public Socket[][] ConnectedSocketClient = new Socket[100][];
        
        public int RoomNo=0;
        public bool NewRoom = true;
        public int RoomIndex = 0;
        public Dictionary<int, int> RoomKey = new Dictionary<int, int>();
        public Dictionary<Socket, int> SocketKey = new Dictionary<Socket, int>();
        public Dictionary<Socket, bool> PingKey = new Dictionary<Socket, bool>();
        
        public static CheckedListBox.ObjectCollection item;
        public bool RoomFlag = false;
        
        #endregion
        
        public WebSocketApp()
        {
            InitializeComponent();
            Application.ApplicationExit += AppExit;
            // Create the thread object. This does not start the thread.
            workerThread = new Thread(DoWork);
            // Start the worker thread.
            workerThread.Start();

            for (int x = 0; x < ConnectedSocketClient.Length; x++)
            {
                ConnectedSocketClient[x] = new Socket[100];
            }
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8080));
                serverSocket.Listen(128);
                dataBuffer = new byte[BUFFER_SIZE];
                dataString = new StringBuilder();
                webSocketOrigin = "http://10.75.15.10";
                webSocketLocation = "ws://10.75.15.10:8080/chat";
                ListenForClients();
                ServerMessage.AppendText("Server Listing on 8080");
                Console.WriteLine("main thread: Starting worker thread...");
                item = Room_1.Items;
                
            
        }
        // look for connecting clients
        private void ListenForClients()
        {
            serverSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);
            
        }
        

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
                    AcceptSocket = client;
                    CreateThread(client);
                   
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
        /// Takes care of the initial handshaking between the the client and the server
        /// </summary>
        public bool ShakeHands(Socket conn, TextBox ServerMessage)
        {
            bool IsOwner = false;
            using (var stream = new NetworkStream(conn))
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            {

                //read handshake from client (no need to actually read it, we know its there):
                //LogLine("Reading client handshake:", ServerLogLevel.Verbose);

                string r = null;
                string clienthandshake = null;
                while (r != "")
                {
                    r = reader.ReadLine();
                    Console.WriteLine(r);
                    //LogLine(r, ServerLogLevel.Verbose);
                    clienthandshake += r + "\n";
                }
                Console.WriteLine(clienthandshake);
                string[] ar = new string[20];
                string[] ar1 = new string[2];
                string[] owner = new string[2];
                bool isSocketIO = false;
                ar = clienthandshake.Split('\n');
                for (int i = 0; i < ar.Length; i++)
                {
                    if (String.Compare(ar[i], 0, "GET", 0, "GET".Length) == 0)
                    {
                        
                        string[] ToCompare = ar[i].Split(' ');
                        foreach (string CompareString in ToCompare)
                        {
                            if (String.Compare(CompareString, 0, "/socket.io/1/", 0, "/socket.io/1/".Length) == 0)
                                isSocketIO = true;
                        }
                        if (!isSocketIO)
                        {
                            owner = ar[i].Split('?');
                            if (owner.Length > 1)
                            {
                                ChowkaWebSocket.Add(conn, false);
                                if (String.Compare(owner[1], 0, "owner=true", 0, "owner=true".Length) == 0)
                                    IsOwner = true;
                                else
                                {
                                    owner = owner[1].Split('&');
                                    owner = owner[1].Split(' ');
                                    Console.WriteLine("Room ID:" + Convert.ToDecimal(owner[0]));
                                    //  foreach (KeyValuePair<int, int> pair in RoomKey)
                                    foreach (var pair in RoomKey)
                                    {

                                        if (pair.Value == Convert.ToDecimal(owner[0]))
                                        {
                                            SocketKey.Add(conn, pair.Key);
                                            RoomFlag = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ChowkaClientState[conn] = (ushort)eClientConnectionStatus.STARTING;
                                ChowkaWebSocket.Add(conn, true);
                                IsOwner = true;
                            }
                        }
                        else
                        {
                            ChowkaWebSocket.Add(conn, true);
                        }
                    }
                    if (!isSocketIO||ChowkaWebSocket[conn])
                    {
                        if (String.Compare(ar[i], 0, "Sec-WebSocket-Key:", 0, "Sec-WebSocket-Key:".Length) == 0)
                        {

                            ar1 = ar[i].Split(':');
                            ar1[1] = ar1[1].Trim();
                        }
                    }


                }
                string handshakemessage = null;
                if (!isSocketIO||ChowkaWebSocket[conn])
                {
                    // send handshake to the client

                    handshakemessage = "HTTP/1.1 101 Switching Protocols\r\n" +
                        "Upgrade: websocket\r\n" +
                        "Connection: Upgrade\r\n" +
                        "Sec-WebSocket-Accept: " + ComputeWebSocketHandshakeSecurityHash09(ar1[1]) + "\r\n\r\n";
                    writer.Write(handshakemessage);
                    ChowkaClientState[conn] = eClientConnectionStatus.ESTABLISHING;
                }
                else
                {
                    handshakemessage = "HTTP/1.1 200 OK\r\n" +
                        "Content-Type:text/plain\r\n";
                    writer.Write(handshakemessage);
                    while (r != "")
                    {
                        r = reader.ReadLine();
                        Console.WriteLine(r);
                        //LogLine(r, ServerLogLevel.Verbose);
                        clienthandshake += r + "\n";
                    }
                    handshakemessage = "HTTP/1.1 101 Switching Protocols\r\n" +
                        "Upgrade: websocket\r\n" +
                        "Connection: Upgrade\r\n";
                    writer.Write(handshakemessage);
                }
                writer.Write(handshakemessage);
            }
            /*
            if (ServerMessage.InvokeRequired)
            {
                ServerMessage.Invoke(new MethodInvoker(delegate { ServerMessage.Clear(); 
                    ServerMessage.AppendText("Server Handshake done to "+conn.RemoteEndPoint); }));
            }
             */
            return IsOwner;
         
        }

        private void Room_1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int RoomID = Room_1.SelectedIndex;
            bool Room_1Flag = false;
            if (-1 != RoomID)
            {
                if (Room_1.GetItemChecked(RoomID))
                {

                    List<string> _items = new List<string>();
                    string Roomname = Room_1.Items[RoomID].ToString();
                    string[] rooms = Roomname.Split(' ');
                    RoomID = (Convert.ToInt32(rooms[1])) - 1;
                    _items.Add("Room ID:" + RoomKey[RoomID].ToString());
                    foreach (var pair in SocketKey)
                    {
                        if (pair.Value == RoomID)
                            if (null != pair.Key)
                            {
                                Room_1Flag = true;
                                _items.Add(pair.Key.RemoteEndPoint.ToString());

                            }
                    }
                    if (Room_1Flag)
                    {
                        Rooms.DataSource = _items;
                    }
                    else
                    {
                        Room_1.Items.RemoveAt(Room_1.SelectedIndex);
                    }
                }
                else
                {
                    Rooms.DataSource = null;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool Flag = false;
            Room_1.Items.Clear();
            for (int i = 0; i < RoomIndex; i++)
            {
                foreach (var pair in SocketKey)
                {
                    if (pair.Value == i)
                        if (null != pair.Key)
                        {
                            Flag = true;
                        }
                }
                if(Flag)
                {
                    Room_1.Items.Add("Room " + (i + 1));
                    Flag = false;
                }
            }
        }

    }
}
