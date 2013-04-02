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
using WebSocketServer;
using Debug;
using RoomManager;
using Authentication;

namespace Server_Chowka_bhara
{
    public partial class ServerUI : Form
    {
        private Room RoomManager = null;
        private WebSocket WS = null;
        private Log log = null;
        public static Thread workerThread;

        private uint TotalUsers = 0;
        private Authentication.Authentication Auth = null;
        /// <summary>
        /// Constructor
        /// </summary>
        public ServerUI()
        {
            InitializeComponent();
            // Create the thread object. This does not start the thread.
            workerThread = new Thread(DoWork);
            // Start the worker thread.
            workerThread.Start();
            log = new Log("Server UI", Debug.Debug.elogLevel.ALL, Debug.Debug.eproductLevel.DEVELOPMENT);
            RoomManager = new Room(MaxRoom, MaxPeer, MinPeer,log);
            WS = new WebSocket(serverPort, pingDuration);
            WS.onOpen += new EventHandler(WS_onOpen);
            WS.onNewUser += new EventHandler<NewUser>(WS_onNewUser);
            WS.onError += new EventHandler<WebSocketServer.ErrorEventArgs>(WS_onError);
            WS.onDebugMessage += new EventHandler<DebugMessages>(WS_onDebugMessage);
            WS.onClosed += new EventHandler<WebSocketClose>(WS_onClosed);
            WS.onMessageReceived += new EventHandler<MessageReceivedEventArgs>(WS_onMessageReceived);
            WS.onUserLeft += new EventHandler<UserLeft>(WS_onUserLeft);
            WS.BeginInitialize();
            Auth = new Authentication.Authentication(MaxUser);
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }

        void WS_onUserLeft(object sender, UserLeft e)
        {
            if (RoomManager.RemoveUser(e.UserSocket))
                DebugMessage("User Removed:" + e.UserSocket.RemoteEndPoint.ToString(), Debug.Debug.elogLevel.INFO);
            else
                DebugMessage("Unable to remove User:" + e.UserSocket.RemoteEndPoint.ToString(), Debug.Debug.elogLevel.INFO);
            this.TotalUsers--;
            this.Status.Text = "No of connection:" + this.TotalUsers;
            Auth.DeRegisterUser(e.UserSocket);
            DisplayRoomNo();
        }
        /// <summary>
        /// Logs the message 
        /// </summary>
        /// <param name="message">Message to logged</param>
        /// <param name="level">Level of the message</param>
        public void DebugMessage(string message, Debug.Debug.elogLevel level)
        {
            if (log != null)
                log.Print(message, level);
        }
        /// <summary>
        ///  Event triggered call-back function when application about to exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            WS.Close();
            RequestStop();
        }
        /// <summary>
        ///  Event triggered call-back function when new message is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WS_onMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            List<int> h = null;
            Tuple<int,int,string> k = null;
            DebugMessage(Environment.NewLine + "Server :" + "Message: " + e.Message + " Opcode:" + e.Opcode.ToString() + "Socket:" + e.clientSocket.RemoteEndPoint
                +" Room Count:" + RoomManager.GetAvailableRoomNumber(ref h).Count,Debug.Debug.elogLevel.INFO);
            try
            {
                JSONObjects Message = JSONObjects.Deserialize(e.Message);
                if (Message.ClientMessage == "ACK" && !Message.HandShake)
                {
                    if (RoomManager.ValidPeer(Message.RoomID, e.clientSocket))
                        Message.ServerMessage = "ACK";
                    else
                        Message.ServerMessage = "FIN";
                    WS.Send(Message.ToJsonString(), e.clientSocket);
                    if (Message.ServerMessage == "FIN")
                    {
                        e.clientSocket.Close(5);
                        RoomManager.RemoveUser(e.clientSocket);
                    }
                }
                if (RoomManager.RoomFull(Message.RoomID) && !RoomManager.RoomLocked(Message.RoomID))
                {
                    Message.ServerMessage = "START";
                    RoomManager.BroadcastTo(Message.RoomID).Broadcast(WS.WebSocketEncoder(Message.ToJsonString()));
                    RoomManager.LockRoom(Message.RoomID);
                }
                RoomManager.BroadcastTo(RoomManager.GetPeerInfo(e.clientSocket, ref k).Item1).Broadcast(WS.WebSocketEncoder(e.Message), e.clientSocket);
            }
            catch (Exception ex)
            {
                DebugMessage("Unformatted/Invalid JSON object received:" + e.Message+" Error Message:"+ex, Debug.Debug.elogLevel.ERROR);
            }
        }
        /// <summary>
        ///  Event triggered call-back function when WebServer closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WS_onClosed(object sender, WebSocketClose e)
        {
            DebugMessage("Server Closed Reason:"+e.Reason,Debug.Debug.elogLevel.ALL);
        }
        /// <summary>
        ///  Event triggered call-back function for Debug messages received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WS_onDebugMessage(object sender, DebugMessages e)
        {
            DebugMessage("Debug Event Message:"+e.Message,Debug.Debug.elogLevel.INFO);
            
        }

        private void WS_onError(object sender, WebSocketServer.ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Event triggered call-back function for New User 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">NewUser type contains socket & Extra field</param>
        private void WS_onNewUser(object sender, NewUser e)
        {
            log.Print("New User :" + e.ExtraField,Debug.Debug.elogLevel.INFO);
            string Key = "" ,value = "";
            if (e.ExtraField != "")
            {
                Key = e.ExtraField.Split('=')[0];
                if(Key == "username")
                    value = e.ExtraField.Split('=')[1];
            }
            DebugMessage("Key :" + Key+" Value:"+value,Debug.Debug.elogLevel.INFO);
            List<int> AvailableRoom = null;
            int RoomNo = 0;
            RoomManager.GetAvailableRoomNumber(ref AvailableRoom);
            if (AvailableRoom.Count == 0)
            {
                RoomNo = RoomManager.CreateRoom();
                DisplayRoomNo();
            }
            else
                RoomNo = AvailableRoom[0];
            byte peerID = (byte)RoomManager.AddUser(RoomNo, e.newUserSocket,value);
            AvailableRoom.Clear();
            //WS.Send("New User Room NO:" + RoomNo + " PeerID:" + peerID, e.newUserSocket);
            JSONObjects JS = new JSONObjects();
            JS.HandShake = false; JS.RoomID = RoomNo; JS.WhoIAm = peerID;
            WS.Send(JS.ToJsonString(), e.newUserSocket);
            JS = null;
            AvailableRoom = null;
            this.TotalUsers++;
            this.Status.Text = "No of connection:" + this.TotalUsers;
        }
        /// <summary>
        ///  Event triggered call-back function for WebSocket Server opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WS_onOpen(object sender, EventArgs e)
        {
            DebugMessage("Server Open", Debug.Debug.elogLevel.INFO);
            ServerMessage.Text = "Server listening at port:"+serverPort;
        }
        private void Room_1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int CheckedIndex = Room_1.SelectedIndex;
            if (CheckedIndex > -1 && Room_1.GetItemChecked(CheckedIndex))
            {
                List<string> _items = new List<string>();
                string Roomname = Room_1.Items[CheckedIndex].ToString().Split(' ')[1];
                _items.Add("Room ID:" + Roomname);
                int RoomID = (Convert.ToInt32(Roomname));
                Dictionary<int, Tuple<Socket, string>> PeerInfo = RoomManager.GetAllPeer(RoomID);
                foreach (var peerInfo in PeerInfo)
                {
                    _items.Add("Peer ID:" + peerInfo.Key +" Name:"+peerInfo.Value.Item2+Environment.NewLine +" From:"+peerInfo.Value.Item1);
                }
                Rooms.DataSource = _items;
                
            }
        }
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            Rooms.DataSource = null;
            Rooms.Items.Clear();
            DisplayRoomNo();
        }
        private void DisplayRoomNo()
        {
            
            List<int> RoomNoList = null;
            RoomManager.GetAllRoomNo(ref RoomNoList);
            if (this.Room_1.InvokeRequired)
                this.Invoke((MethodInvoker)delegate
                {
                    DisplayRoomNo();
                });
            else
            {
                Room_1.Items.Clear();
                foreach (int roomNo in RoomNoList)
                    Room_1.Items.Add("Room " + (roomNo));
            }
            RoomNoList = null;
        }
    }
}
