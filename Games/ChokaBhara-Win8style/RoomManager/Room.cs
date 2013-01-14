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
        private Dictionary<int,Tuple<Socket,string>> RoomInfo;
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
            this.RoomInfo[RoomID] = null;
            return RoomID;
        }
        public int JoinUser(Socket userSocket, string userName)
        {
        }
    }
}
