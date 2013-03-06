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
        /// <summary>
        /// Gets a unique ID for a room
        /// </summary>
        /// <returns>Unique ID</returns>
        private int GetUID(int randomNumber = INVALIDROOM)
        {
            if (!RoomInfo.ContainsKey(randomNumber) || randomNumber != INVALIDROOM)
            {
                return randomNumber;
            }
            Random getUID = new Random();
            return GetUID(getUID.Next(this.MaxRoom));
        }
        /// <summary>
        /// Creates a new room 
        /// </summary>
        /// <returns>Room ID</returns>
        public int CreateRoom()
        {
            int RoomID = GetUID();
            this.RoomInfo[RoomID] = new RoomHost(RoomID,this.MaxRoomSize);
            return RoomID;
        }
        /// <summary>
        /// Add user to a room
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="userSocket"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int AddUser(int RoomID,Socket userSocket, string userName = "Dummy")
        {
            foreach (var Room in RoomInfo)
            {
                if (Room.Key == RoomID)
                    this.RoomInfo[RoomID].AddPeer(userSocket, userName);
                return 0;
            }
            return INVALIDROOM;
        }
        public List<int> GetAvailableRoomNumber()
        {
            List<int> AvailableRoomNumber = new List<int>();
            foreach (var Room in RoomInfo)
            {
                if (this.RoomInfo[Room.Key].GetRoomStatus() == RoomManager.RoomHost.STATE.ROOMFREE || this.RoomInfo[Room.Key].GetRoomStatus() == RoomManager.RoomHost.STATE.ROOMFREE)
                {
                    AvailableRoomNumber.Add(Room.Key);
                }
            }
            return AvailableRoomNumber;
        }
    }
}
