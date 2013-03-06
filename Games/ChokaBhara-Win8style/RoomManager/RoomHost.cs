using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Debug;
namespace RoomManager
{
    class RoomHost
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
            return this.roomSize <= GetPeerCount();
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
            if(!RoomFull() && !locked)
            {
                int currentPeerID = GetUID();
                this.PeerInfo[currentPeerID] = new Tuple<Socket, string>(peerSocket, peerName);
                peerCount++;
                Trace("Added new user IP:" + peerSocket.RemoteEndPoint.ToString() + " with Name:" + peerName, Debug.Debug.elogLevel.INFO);
                RoomState = STATE.ROOMFREE;
                return currentPeerID;
            }
            if (RoomState != STATE.LOCKED)
            {
                Trace("Unable to add new user IP:" + peerSocket.RemoteEndPoint.ToString() + " with Name:" + peerName +" RoomFull", Debug.Debug.elogLevel.ERROR);
                RoomState = STATE.ROOMFULL;
            }
            else
                Trace("Unable to add new user IP:" + peerSocket.RemoteEndPoint.ToString() + " with Name:" + peerName + " RoomLocked", Debug.Debug.elogLevel.ERROR);
            return (int)RoomState;
        }
        /// <summary>
        /// Removes a peer from the room
        /// </summary>
        /// <param name="peerID"></param>
        /// <returns></returns>
        public bool RemovePeer(int peerID)
        {
            foreach (var peerid in this.PeerInfo)
            {
                if (peerid.Key == peerID)
                {
                    Trace("Removed user IP:" + peerid.Value.Item1.RemoteEndPoint.ToString() + " with Name:" + peerid.Value.Item2 + " RoomLocked", Debug.Debug.elogLevel.INFO);
                    this.PeerInfo[peerID] = null;
                    peerCount--;
                    return true;
                }
            }
            Trace("Unable to find user with peer ID:"+peerID,Debug.Debug.elogLevel.ERROR);
            return false;
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
        public STATE GetRoomStatus()
        {
            return RoomState;
        }
        /// <summary>
        /// Destructor 
        /// </summary>
        /// <returns>Dispose success status</returns>
        public bool Dispose()
        {
            foreach (var peerid in this.PeerInfo)
            {
                this.PeerInfo[peerid.Key] = null;
            }
            this.PeerInfo = null;
            Trace("Room " + this.roomNumber +" deleted", Debug.Debug.elogLevel.INFO);
            return true;
        }
        
        #endregion

    }
}
