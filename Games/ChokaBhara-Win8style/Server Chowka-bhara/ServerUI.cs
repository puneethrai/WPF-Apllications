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

namespace Server_Chowka_bhara
{
    public partial class ServerUI : Form
    {
        private Room RoomManager = null;
        private WebSocket WS = null;
        private Log log = null;
        public static Thread workerThread; 
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
            RoomManager = new Room(100, 4, 2,log);
            WS = new WebSocket(8080, 25);
            WS.onOpen += new EventHandler(WS_onOpen);
            WS.onNewUser += new EventHandler<NewUser>(WS_onNewUser);
            WS.onError += new EventHandler<WebSocketServer.ErrorEventArgs>(WS_onError);
            WS.onDebugMessage += new EventHandler<DebugMessages>(WS_onDebugMessage);
            WS.onClosed += new EventHandler<WebSocketClose>(WS_onClosed);
            WS.onMessageReceived += new EventHandler<MessageReceivedEventArgs>(WS_onMessageReceived);
            WS.onUserLeft += new EventHandler<UserLeft>(WS_onUserLeft);
            WS.BeginInitialize();
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }

        void WS_onUserLeft(object sender, UserLeft e)
        {
            if (RoomManager.RemoveUser(e.UserSocket))
                DebugMessage("User Removed:" + e.UserSocket.RemoteEndPoint.ToString(), Debug.Debug.elogLevel.INFO);
            else
                DebugMessage("Unable to remove User:" + e.UserSocket.RemoteEndPoint.ToString(), Debug.Debug.elogLevel.INFO);
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
        ///  Event triggered callback function when application about to exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            WS.Close();
            RequestStop();
        }
        /// <summary>
        ///  Event triggered callback function when new message is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WS_onMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            List<int> h = null;
            DebugMessage(Environment.NewLine + "Server :" + "Message: " + e.Message + " Opcode:" + e.Opcode.ToString() + "Socket:" + e.clientSocket.RemoteEndPoint
                +" Room Count:" + RoomManager.GetAvailableRoomNumber(ref h).Count,Debug.Debug.elogLevel.INFO);
        }
        /// <summary>
        ///  Event triggered callback function when WebServer closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WS_onClosed(object sender, WebSocketClose e)
        {
            DebugMessage("Server Closed Reason:"+e.Reason,Debug.Debug.elogLevel.ALL);
        }
        /// <summary>
        ///  Event triggered callback function for Debug messages received
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
        /// Event triggered callback function for New User 
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
                RoomNo = RoomManager.CreateRoom();
            else
                RoomNo = AvailableRoom[0];
            int peerID = RoomManager.AddUser(RoomNo, e.newUserSocket,value);
            AvailableRoom.Clear();
            WS.Send("New User Room NO:" + RoomNo + " PeerID:" + peerID, e.newUserSocket);
            AvailableRoom = null;
        }
        /// <summary>
        ///  Event triggered callback function for WebSocket Server opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WS_onOpen(object sender, EventArgs e)
        {
            DebugMessage("Server Open", Debug.Debug.elogLevel.INFO);
        }
        private void Room_1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
        }
    }
}
