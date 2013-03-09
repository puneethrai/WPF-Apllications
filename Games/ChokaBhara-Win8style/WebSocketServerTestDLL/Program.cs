using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketServer;
using Debug;
using RoomManager;
using System.Net.Sockets;
//describe:
//test result:
namespace WebSocketServerTestDLL
{
    class Program
    {
        static Log testLog = new Log("Main Test Suit", Log.elogLevel.ALL, Log.eproductLevel.DEVELOPMENT);
        static void Main(string[] args)
        {
            //describe:Create new instance with default param
            //test result:passed need to test ping frame
            Console.WriteLine("Starting webserver");
            WebSocket ws = new WebSocket();
            Console.WriteLine("Websocketserver started");
            //describe:Create new instance with specified param
            //test result:passed need to test ping frame
            WebSocket fe = new WebSocket(8080, 10);
            //Describe:Testing debug class
            //test result:passed 
            Log l = new Log("Test Suit1", Log.elogLevel.ALL, Log.eproductLevel.DEVELOPMENT);
            TestScript TS = new TestScript(l);
            l.Print("Hey wats up buddy", Log.elogLevel.INFO);
            //Describe:Testing debug class for another instance with log level changed dynamically 
            //test result:passed 
            Log l1 = new Log("Test Suit2", Log.elogLevel.ALL, Log.eproductLevel.DEVELOPMENT);
            l1.Print("Hey wats up buddy", Log.elogLevel.INFO);
            l1.ChangeLogLevel(Log.elogLevel.ERROR);
            l1.Print("This should not be printed", Log.elogLevel.INFO);
            //Describe:Testing debug class for file operation
            //test result:passed 
            Log l2 = new Log("Test Suit2", Log.elogLevel.ALL, Log.eproductLevel.PRODUCTION);
            l2.Print("Hey wats up buddy", Log.elogLevel.INFO);
            

            //Decribe:testing event handlers
            //test result:passed not all state checked yet
            testLog.Print("Server State:"+ws.GetServerStatus(),Log.elogLevel.INFO);
            ws.onClosed += new EventHandler<WebSocketClose>(ws_onClosed);
            ws.onDebugMessage += new EventHandler<DebugMessages>(ws_onDebugMessage);
            ws.onError += new EventHandler<ErrorEventArgs>(ws_onError);
            ws.onMessageReceived += new EventHandler<MessageReceivedEventArgs>(ws_onMessageReceived);
            ws.onOpen += new EventHandler(ws_onOpen);
            ws.BeginInitialize();
            //Decribe:testing error handling
            //test result:passed not all state checked yet
            testLog.Print("Server State:" + ws.GetServerStatus(), Log.elogLevel.INFO);
            testLog.Print("Server State:" + fe.GetServerStatus(), Log.elogLevel.INFO);
            fe.onClosed += new EventHandler<WebSocketClose>(ws_onClosed);
            fe.onDebugMessage += new EventHandler<DebugMessages>(ws_onDebugMessage);
            fe.onError += new EventHandler<ErrorEventArgs>(ws_onError);
            fe.onMessageReceived += new EventHandler<MessageReceivedEventArgs>(ws_onMessageReceived);
            fe.onOpen += new EventHandler(ws_onOpen);
            fe.BeginInitialize();
            testLog.Print("Server State:" + fe.GetServerStatus(), Log.elogLevel.INFO);
            //Decribe:testing Room Management
            //test result:passed not all state checked yet
            Room room = new Room(5, 4, 2, testLog);
            int roomno =  room.CreateRoom();
            roomno = room.CreateRoom();
            roomno = room.CreateRoom();
            Socket S = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            room.AddUser(roomno,S);
            room.AddUser(roomno, S);
            room.AddUser(roomno, S);
            room.LockRoom(roomno);
            room.AddUser(roomno, S);
            room.AddUser(roomno, S);
            
            TS.Test(room.GetPeerID(roomno, S), 0, "Testing PeerID");
            TS.Test(room.GetPeerInfo(roomno, room.GetPeerID(roomno, S)).Item2, "Dummy", "Testing PeerInfo Name");
            TS.Test(room.GetPeerInfo(roomno, room.GetPeerID(roomno, S)).Item1, S, "Testing PeerInfo Socket");
            List<int> H = null;
            room.GetAvailableRoomNumber(ref H);
            room.BroadcastTo(roomno).Broadcast(Encoding.ASCII.GetBytes("Hey buddy"), null, room.GetPeerID(roomno, S));
            TS.Test(H.Count, 2, "Testing Available Room");
            foreach (var i in H)
            {
                testLog.Print("Available rooms:" + i, Log.elogLevel.INFO);
            }
            TS.FinalReport();
            TS = null;
            Console.ReadLine();
            
        }

        static void ws_onOpen(object sender, EventArgs e)
        {
            testLog.Print("Server open Event triggered ", Log.elogLevel.INFO);
        }

        static void ws_onMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            testLog.Print("Message Event triggered ,contains message: " + e.Message, Log.elogLevel.INFO);
        }

        static void ws_onError(object sender, ErrorEventArgs e)
        {
            testLog.Print("Error Event triggered ,contains message: " + e.Message, Log.elogLevel.INFO);
        }

        static void ws_onDebugMessage(object sender, DebugMessages e)
        {
            testLog.Print("Debug Event triggered ,contains message: "+e.Message, Log.elogLevel.INFO);
        }

        static void ws_onClosed(object sender, WebSocketClose e)
        {
            testLog.Print("Close Event triggered", Log.elogLevel.INFO);
        }
    }
}
