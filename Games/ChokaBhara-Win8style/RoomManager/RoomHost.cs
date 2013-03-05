using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
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
        /// Peer Manger instance object
        /// </summary>
        private PeerManager peerList = null;
        private bool locked = false;
        private enum ERROR {ROOMFULL};
        /// <summary>
        /// Returns boolean whether room is full or not
        /// </summary>
        /// <returns></returns>
        private bool RoomFull()
        {
            return this.roomSize <= peerList.GetPeerCount();
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
            set;
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
            this.peerList = new PeerManager(maxPeerSize);
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
                int peerID = peerList.AddPeer(peerSocket,peerName);
                return peerID;
            }
            return (int)ERROR.ROOMFULL;
        }
        /// <summary>
        /// Removes a peer from the room
        /// </summary>
        /// <param name="peerID"></param>
        /// <returns></returns>
        public bool RemovePeer(int peerID)
        {
            return peerList.RemovePeer(peerID);
        }
        public void lookRoom()
        {
            locked = true;
        }
        /// <summary>
        /// returns room number
        /// </summary>
        /// <returns></returns>
        public int GetRoomNumber()
        {
            return this.roomNumber;
        }
        /// <summary>
        /// Destructor 
        /// </summary>
        /// <returns></returns>
        public bool Dispose()
        {
            this.peerList.Dispose();
            this.peerList = null;
            return false;
        }
        public 
        #endregion

    }
}
