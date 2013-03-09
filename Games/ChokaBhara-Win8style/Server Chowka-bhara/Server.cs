using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Debug;
using RoomManager;
using WebSocketServer;
namespace Server_Chowka_bhara
{
    public partial class Server : Form
    {
        private Socket serverSocket;
        private int CurrentConectedUsers = 0;
        public const int BUFFER_SIZE = 1000000;
        public const int bufferSize = 255;
        public Socket AcceptSocket = null;
        public static Thread workerThread;
        public byte[] buffer = new byte[BUFFER_SIZE];
        private byte[] dataBuffer;                                  // buffer to hold the data we are reading
        private StringBuilder dataString;                           // holds the currently accumulated data
        public static CheckedListBox.ObjectCollection item;
        public int RoomNo = 0;
        public bool NewRoom = true;
        public int RoomIndex = 0;
        public Dictionary<int, int> RoomKey = new Dictionary<int, int>();
        public Dictionary<Socket, int> SocketKey = new Dictionary<Socket, int>();
        public Dictionary<Socket, bool> PingKey = new Dictionary<Socket, bool>();
        public bool RoomFlag = false;
        public Server()
        {
            InitializeComponent();
            Form Active = Form.ActiveForm;
            
            // Create the thread object. This does not start the thread.
            workerThread = new Thread(DoWork);
            // Start the worker thread.
            workerThread.Start();
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            serverSocket.Listen(128);
            dataBuffer = new byte[BUFFER_SIZE];
            dataString = new StringBuilder();
            ListenForClients();
            ServerMessage.AppendText("Server Listing on 8080");
            Console.WriteLine("main thread: Starting worker thread...");
            item = Room_1.Items;
            Active.FormClosed += AppExit;


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
        /// Takes care of the initial handshaking between the the client and the server
        /// </summary>
        public bool ShakeHands(Socket conn, TextBox ServerMessage)
        {
            bool IsOwner = false;
            using (var stream = new NetworkStream(conn))
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            {

                // send handshake to the client

                string handshakemessage = null;
                writer.Write(handshakemessage);


            }

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

        private void RefreshBtn_Click(object sender, EventArgs e)
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
                if (Flag)
                {
                    Room_1.Items.Add("Room " + (i + 1));
                    Flag = false;
                }
            }
        }
    }
}
