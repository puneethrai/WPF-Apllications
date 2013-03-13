using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Debug;
namespace RoomManager
{
    public class RoomHost
    {
        #region private
        /// <summary>
        /// Instance room number
        /// </summary>
        private int roomNumber = 0;
        /// <summary>
        /// Instance room size
        /// </summary>
        private int roomSize = 0;
        /// <summary>
        /// Boolean to know room is locked
        /// </summary>
        private bool locked = false;
        private bool canDebug = false;
        private Debug.Debug debugObject = null;
        public enum STATE {ROOMFULL,LOCKED,ROOMEMPTY,ROOMFREE};
        
        private Dictionary<int, Tuple<Socket, string>> PeerInfo = null;
        private int peerCount = 0;
        private int peerID = 0;
        private int peerSize = 0;
        private STATE RoomState = STATE.ROOMEMPTY;
        public const Int16 INVALIDPEER = -1;
        public int GetPeerCount()
        {
            return this.peerCount;
        }
        /// <summary>
        /// Get Unique ID for each peer added
        /// </summary>
        /// <returns></returns>
        private int GetUID()
        {
            return peerID++;
        }
        /// <summary>
        /// Returns boolean whether room is full or not
        /// </summary>
        /// <returns></returns>
        private bool RoomFull()
        {
            return this.roomSize < (GetPeerCount()+1);
        }
        private void Trace(string message,Debug.Debug.elogLevel logLevel)
        {
            if (canDebug)
            {
                debugObject.Print(message, logLevel);
            }
        }
        #endregion
        #region public
        /// <summary>
        /// returns true if room is full else false
        /// </summary>
        public bool isRoomFull 
        {
            get
            {
                return RoomFull();
            }
            set{}
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="roomNo">Room Number assigned</param>
        /// <param name="maxPeerSize">Max peers allowed</param>
        public RoomHost(int roomNo,int maxPeerSize)
        {
            this.roomNumber = roomNo;
            this.roomSize = maxPeerSize;
            this.PeerInfo = new Dictionary<int,Tuple<Socket,string>>(this.peerSize);
            Trace("New Room Created", Debug.Debug.elogLevel.INFO);
        }
        /// <summary>
        /// Constructor with debug enabled
        /// </summary>
        /// <param name="roomNo">Room Number assigned</param>
        /// <param name="maxPeerSize">Max peers allowed</param>
        /// <param name="debugObj">Debug object instance</param>
        public RoomHost(int roomNo,int maxPeerSize,Debug.Debug debugObj)
        {
            this.roomNumber = roomNo;
            this.roomSize = maxPeerSize;
            this.PeerInfo = new Dictionary<int, Tuple<Socket, string>>(this.peerSize);
            this.debugObject = debugObj;
            this.canDebug = true;
            Trace("New Room Created with debug on", Debug.Debug.elogLevel.INFO);
        }

        /// <summary>
        /// Adds Peer to this room
        /// </summary>
        /// <param name="peerSocket">Peer's socket fd</param>
        /// <param name="peerName">Peer's Name</param>
        /// <returns></returns>
        public int AddPeer(Socket peerSocket,string peerName)
        {
            StringBuilder debugMessage = new StringBuilder();
            if (!RoomFull() && !locked)
            {
                int currentPeerID = GetUID();
                this.PeerInfo[currentPeerID] = new Tuple<Socket, string>(peerSocket, peerName);
                peerCount++;
                debugMessage.Append("Added new user ");
                if (peerSocket.RemoteEndPoint != null)
                    debugMessage.Append("IP:" + peerSocket.RemoteEndPoint.ToString());
                debugMessage.Append("with Name:" + peerName);
                Trace(debugMessage.ToString(), Debug.Debug.elogLevel.INFO);
                RoomState = STATE.ROOMFREE;
                if (RoomFull())
                {
                    Trace("No more user allowed ,Room Full", Debug.Debug.elogLevel.INFO);
                    RoomState = STATE.ROOMFULL;
                } 
                return currentPeerID;
            }
            else
            {
                debugMessage.Append("Unable to add new user ");
                if (peerSocket.RemoteEndPoint != null)
                    debugMessage.Append("IP:" + peerSocket.RemoteEndPoint.ToString());
                debugMessage.Append(" with Name:" + peerName);
                
                if (RoomState != STATE.LOCKED)
                {
                    debugMessage.Append(" ROOMFULL");
                    RoomState = STATE.ROOMFULL;
                }
                else
                    debugMessage.Append(" ROOMLocked");
                Trace(debugMessage.ToString(), Debug.Debug.elogLevel.INFO);
            }
            
            debugMessage.Clear();
            return (int)RoomState;
        }
        /// <summary>
        /// Removes a peer from the room
        /// </summary>
        /// <param name="peerID"></param>
        /// <returns></returns>
        public bool RemovePeer(int peerID)
        {
            StringBuilder debugMessage = new StringBuilder();
            bool flag = false;
            if(this.PeerInfo.ContainsKey(peerID))
            {
                debugMessage.Append("Removed user ");
                if (this.PeerInfo[peerID].Item1.RemoteEndPoint != null)
                    debugMessage.Append("IP:" + this.PeerInfo[peerID].Item1.RemoteEndPoint.ToString());
                debugMessage.Append(" with Name:" + this.PeerInfo[peerID].Item2);
                Trace(debugMessage.ToString(), Debug.Debug.elogLevel.INFO);
                this.PeerInfo[peerID] = null;
                this.PeerInfo.Remove(peerID);
                peerCount--;   
            }
            else
                Trace("Unable to find user with peer ID:"+peerID,Debug.Debug.elogLevel.ERROR);
            debugMessage.Clear();
            debugMessage = null;
            return flag;
        }
        /// <summary>
        /// Lock the current room futher peer addition
        /// </summary>
        public void lookRoom()
        {
            locked = true;
            RoomState = STATE.LOCKED;
            Trace("Room Locked", Debug.Debug.elogLevel.INFO);
        }
        /// <summary>
        /// returns room number
        /// </summary>
        /// <returns>Returns current instances room number</returns>
        public int GetRoomNumber()
        {
            return this.roomNumber;
        }
        /// <summary>
        /// Returns current room Status 
        /// </summary>
        /// <returns>Room State</returns>
        public STATE GetRoomStatus()
        {
            return RoomState;
        }
        /// <summary>
        /// Broadcasts to all user
        /// </summary>
        /// <param name="message">WebSocket message or Normal Message</param>
        /// <param name="ExceptSocket">Broadcast Message Sholud not sent to this SocketFD</param>
        /// <param name="PeerID">Broadcast Message Sholud not sent to this PeerID</param>
        public void Broadcast(byte[] message,Socket ExceptSocket = null,int PeerID = INVALIDPEER)
        {
            foreach (var peerID in this.PeerInfo)
            {
                if (ExceptSocket == peerID.Value.Item1 || PeerID == peerID.Key)
                    continue;
                peerID.Value.Item1.Send(message);
            }
        }
        /// <summary>
        /// Returns PeerID by passing SocketFD
        /// </summary>
        /// <param name="verifySocket">Socket FD to verify</param>
        /// <returns>PeerID or INVALIDPEER</returns>
        public int GetPeerID(Socket verifySocket)
        {
            foreach (var peerInfo in this.PeerInfo)
            {
                if (peerInfo.Value.Item1 == verifySocket)
                {
                    return peerInfo.Key;
                }

            }
            return INVALIDPEER;
        }
        /// <summary>
        /// Returns Peers Info
        /// </summary>
        /// <param name="PeerID">Peer ID to query for</param>
        /// <returns>Tuple of Socket FD & name of the peer</returns>
        public Tuple<Socket, string> GetPeerInfo(int PeerID)
        {
            if (this.PeerInfo.ContainsKey(PeerID))
            {
                return this.PeerInfo[PeerID];
            }
            return null;
        }
        public Dictionary<int, Tuple<Socket, string>> GetAllPeer()
        {
            return this.PeerInfo;
        }
        /// <summary>
        /// Destructor 
        /// </summary>
        /// <returns>Dispose success status</returns>
        public void Dispose()
        {
            foreach (var peerid in this.PeerInfo)
            {
                this.PeerInfo[peerid.Key] = null;
            }
            this.PeerInfo.Clear();
            this.PeerInfo = null;
            Trace("Room " + this.roomNumber +" deleted", Debug.Debug.elogLevel.INFO);
            return;
        }
        
        #endregion

    }
}
