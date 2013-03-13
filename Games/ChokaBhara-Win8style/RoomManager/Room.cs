using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Debug;

namespace RoomManager
{
    public class Room
    {
        /// <summary>
        /// Contains List of socket FD & Name of perticular room
        /// </summary>
        private Dictionary<int,RoomHost> RoomInfo;
        private bool canLog = false;
        private Debug.Debug _log = null;
        /// <summary>
        /// Max Room allowed,Max Room Size and Min Room Size
        /// </summary>
        private int MaxRoom = 0,MaxRoomSize = 0,MinRoomSize;
        public const Int16 INVALIDROOM = -1;
        public Room(int maxRoom,int maxRoomSize,int minRoomSize)
        {
            this.MaxRoom = maxRoom;
            this.MaxRoomSize = maxRoomSize;
            this.MinRoomSize = minRoomSize;
            this.RoomInfo = new Dictionary<int, RoomHost>(maxRoom);
        }
        public Room(int maxRoom, int maxRoomSize, int minRoomSize,Debug.Debug log)
        {
            this.MaxRoom = maxRoom;
            this.MaxRoomSize = maxRoomSize;
            this.MinRoomSize = minRoomSize;
            this.RoomInfo = new Dictionary<int, RoomHost>(maxRoom);
            _log = log;
            this.canLog = true;
        }
        /// <summary>
        /// Gets a unique ID for a room
        /// </summary>
        /// <returns>Unique ID</returns>
        private int GetUID()
        {
            Random getUID = new Random();
            int newRoomNo = 0;
            while (true)
            {
                newRoomNo = getUID.Next(this.MaxRoom);
                if (!RoomInfo.ContainsKey(newRoomNo))
                {
                    break;
                }
            }
            return newRoomNo;
        }
        /// <summary>
        /// Creates a new room 
        /// </summary>
        /// <returns>Room ID</returns>
        public int CreateRoom()
        {
            int RoomID = GetUID();
            if(canLog)
                this.RoomInfo[RoomID] = new RoomHost(RoomID, this.MaxRoomSize,_log);
            else
                this.RoomInfo[RoomID] = new RoomHost(RoomID,this.MaxRoomSize);
            return RoomID;
        }
        /// <summary>
        /// Add user to a room
        /// </summary>
        /// <param name="RoomID">Room ID to Of user</param>
        /// <param name="userSocket">Socket FD</param>
        /// <param name="userName">User Name</param>
        /// <returns></returns>
        public int AddUser(int RoomID,Socket userSocket, string userName = "Dummy")
        {
            if(RoomInfo.ContainsKey(RoomID))
            {
                return this.RoomInfo[RoomID].AddPeer(userSocket, userName);    
            }
            return INVALIDROOM;
        }
        /// <summary>
        /// Removes Peer from a Room using PeerID
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="PeerID"></param>
        /// <returns></returns>
        public bool RemoveUser(int RoomID, int PeerID)
        {
            if (RoomInfo.ContainsKey(RoomID))
            {
                    this.RoomInfo[RoomID].RemovePeer(PeerID);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Removes Peer from a Room using only SOcket FD
        /// </summary>
        ///<param name="userSocket">UserSocket to search</param>
        /// <returns></returns>
        public bool RemoveUser(Socket userSocket)
        {
            int tempPeerID = 0;
            foreach(var roomID in RoomInfo)
            {
                tempPeerID = RoomInfo[roomID.Key].GetPeerID(userSocket);
                if (tempPeerID != RoomHost.INVALIDPEER)
                {
                    RoomInfo[roomID.Key].RemovePeer(tempPeerID);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Let Available Room ID
        /// <param name="AvailableRoomNumber">Input List</param>
        /// </summary>
        /// <returns>List of int containg available rooms</returns>
        public List<int> GetAvailableRoomNumber(ref List<int> AvailableRoomNumber)
        {
            if(AvailableRoomNumber == null)
                AvailableRoomNumber = new List<int>();
            foreach (var Room in RoomInfo)
            {
                if (this.RoomInfo[Room.Key].GetRoomStatus() == RoomHost.STATE.ROOMFREE || this.RoomInfo[Room.Key].GetRoomStatus() == RoomHost.STATE.ROOMEMPTY)
                {
                    AvailableRoomNumber.Add(Room.Key);
                }
            }
            
            return AvailableRoomNumber;
        }
        /// <summary>
        /// Lock a room
        /// </summary>
        /// <param name="RoomID">Room number to lock</param>
        /// <returns>Success status</returns>
        public int LockRoom(int RoomID)
        {
            if (RoomInfo.ContainsKey(RoomID))
            {
                RoomInfo[RoomID].lookRoom();
                return 0;
            }
            return INVALIDROOM;
        }
        /// <summary>
        /// Returns PeerID by passing SocketFD
        /// </summary>
        /// <param name="verifySocket">Socket FD to verify</param>
        /// <param name="RoomID">RoomID to search for</param>
        /// <returns>PeerID or INVALIDPEER</returns>
        public int GetPeerID(int RoomID,Socket verifySocket)
        {
            if (RoomInfo.ContainsKey(RoomID))
            {
                return RoomInfo[RoomID].GetPeerID(verifySocket);
            }
            return INVALIDROOM;
        }
        /// <summary>
        /// Removes Peer from a Room using only SOcket FD
        /// </summary>
        ///<param name="userSocket">UserSocket to search</param>
        ///<param name="completeInfo">Null refrence Tuple </param>
        /// <returns>Tuple with  RoomID,PeerID,PeerName</returns>
        public Tuple<int,int,string> GetPeerInfo(Socket userSocket,ref Tuple<int,int,string> completeInfo)
        {
            int tempPeerID = 0;
            foreach (var roomID in RoomInfo)
            {
                tempPeerID = RoomInfo[roomID.Key].GetPeerID(userSocket);
                if (tempPeerID != RoomHost.INVALIDPEER)
                {
                    if (completeInfo != null)
                    {
                        completeInfo = null;
                    }
                    completeInfo = new Tuple<int, int, string>(roomID.Key, tempPeerID, this.RoomInfo[roomID.Key].GetPeerInfo(tempPeerID).Item2);
                    return completeInfo;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns Peer Info
        /// </summary>
        /// <param name="RoomID">Room ID of peer</param>
        /// <param name="PeerID">Peer ID to query for info</param>
        /// <returns>Tuple containg Socket FD & name of the Peer</returns>
        public Tuple<Socket,string> GetPeerInfo(int RoomID, int PeerID)
        {
            if (this.RoomInfo.ContainsKey(RoomID))
            {
                return this.RoomInfo[RoomID].GetPeerInfo(PeerID);
            }
            return null;

        }
        public void GetAllRoomNo(ref List<int> ListRoomNo)
        {
            if (ListRoomNo != null)
                ListRoomNo = null;
            ListRoomNo = new List<int>(MaxRoom);
            foreach (var roomID in this.RoomInfo)
                ListRoomNo.Add(roomID.Key);
        }
        public Dictionary<int,Tuple<Socket,string>> GetAllPeer(int RoomID)
        {
            if (this.RoomInfo.ContainsKey(RoomID))
            {
                return this.RoomInfo[RoomID].GetAllPeer();
            }
            return null;
        }
        /// <summary>
        /// Get Room to broadcast to other
        /// </summary>
        /// <param name="RoomID"></param>
        /// <returns></returns>
        public RoomHost BroadcastTo(int RoomID)
        {
            if (this.RoomInfo.ContainsKey(RoomID))
            {
                return this.RoomInfo[RoomID];
            }
            return null;
        }
        /// <summary>
        /// Destructor
        /// </summary>
        public void Dispose()
        {
            foreach (var room in RoomInfo)
            {
                RoomInfo[room.Key].Dispose();
            }
            RoomInfo.Clear();
            RoomInfo = null;
        }
    }
}
