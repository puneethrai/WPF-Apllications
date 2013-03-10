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
        public ServerUI()
        {
            InitializeComponent();
            log = new Log("Server UI", Debug.Debug.elogLevel.ALL, Debug.Debug.eproductLevel.DEVELOPMENT);
            RoomManager = new Room(100, 4, 2,log);
            WS = new WebSocket(8080, 25);
            WS.onOpen += new EventHandler(WS_onOpen);
            WS.onNewUser += new EventHandler<NewUser>(WS_onNewUser);
            WS.onError += new EventHandler<WebSocketServer.ErrorEventArgs>(WS_onError);
            WS.onDebugMessage += new EventHandler<DebugMessages>(WS_onDebugMessage);
            WS.onClosed += new EventHandler<WebSocketClose>(WS_onClosed);
            WS.onMessageReceived += new EventHandler<MessageReceivedEventArgs>(WS_onMessageReceived);
            WS.BeginInitialize();
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }
        
        void Application_ApplicationExit(object sender, EventArgs e)
        {
            WS.Close();
        }

        void WS_onMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            List<int> h = null;
            Console.WriteLine("Message: "+e.Message+" Room Count:"+RoomManager.GetAvailableRoomNumber(ref h).Count);
        }

        void WS_onClosed(object sender, WebSocketClose e)
        {
            //log("Server Closed Reaseon:"+e.Reason,Debug.Debug.elogLevel.ALL);
        }

        void WS_onDebugMessage(object sender, DebugMessages e)
        {
            Console.WriteLine(e.Message);
        }

        void WS_onError(object sender, WebSocketServer.ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        void WS_onNewUser(object sender, NewUser e)
        {
            Console.WriteLine("New User :" + e.ExtraField);
            string Key = "" ,value = "";
            if (e.ExtraField != "")
            {
                Key = e.ExtraField.Split('=')[0];
                if(Key == "username")
                    value = e.ExtraField.Split('=')[1];
            }
            Console.WriteLine("Key :" + Key+" Value:"+value);
            List<int> AvailableRoom = null;
            int RoomNo = 0;
            RoomManager.GetAvailableRoomNumber(ref AvailableRoom);
            if (AvailableRoom.Count == 0)
                RoomNo = RoomManager.CreateRoom();
            else
                RoomNo = AvailableRoom[0];
            RoomManager.AddUser(RoomNo, e.newUserSocket,value);
            AvailableRoom.Clear();
            WS.Send("New User", e.newUserSocket);
            AvailableRoom = null;
        }

        void WS_onOpen(object sender, EventArgs e)
        {
            log.Print("Server Open", Debug.Debug.elogLevel.INFO);
        }
        private void Room_1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
        }
    }
}
