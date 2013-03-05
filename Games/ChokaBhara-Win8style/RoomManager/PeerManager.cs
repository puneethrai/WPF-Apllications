using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace RoomManager
{
    class PeerManager
    {
        #region private
        private Dictionary<int, Tuple<Socket, string>> PeerInfo = null;
        private int peerCount = 0;
        private int peerID = 0;
        private int peerSize = 0;
        private int GetUID()
        {
            return peerID++;
        }
        #endregion
        #region public
        public PeerManager(int PeerSize)
        {
            this.peerSize = PeerSize;
            this.PeerInfo = new Dictionary<int,Tuple<Socket,string>>(this.peerSize);
        }
        
        public int GetPeerCount()
        {
            return this.peerCount;
        }
        public int AddPeer(Socket peerSocket, string peerName)
        {
            int currentPeerID = GetUID();
            this.PeerInfo[currentPeerID] = new Tuple<Socket, string>(peerSocket, peerName);
            peerCount++;
            return currentPeerID;
        }
        public bool RemovePeer(int peerID)
        {
            foreach (var peerid in this.PeerInfo)
            {
                if (peerid.Key == peerID)
                {
                    this.PeerInfo[peerID] = null;
                    peerCount--;
                    return true;
                }
            }
            return false;
        }
        public bool Dispose()
        {
            
            foreach (var peerid in this.PeerInfo)
            {
                this.PeerInfo[peerid.Key] = null;
            }
            this.PeerInfo = null;
            return false;
        }
        #endregion
    }
}
