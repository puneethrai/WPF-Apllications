﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
namespace WebSocket
{
    #region JSON
    [JsonObject(MemberSerialization.OptIn)]
    public class JSONObjects
    {
        [JsonProperty]
        public int RoomID { get; set; }

        [JsonProperty]
        public bool HandShake { get; set; }

        [JsonProperty]
        public int KayiNo { get; set; }

        [JsonProperty]
        public int KayiMove { get; set; }

        [JsonProperty]
        public byte WhoIAm { get; set; }

        [JsonProperty]
        public string ClientVersion { get; set; }

        [JsonProperty]
        public string ServerMessage { get; set; }

        [JsonProperty]
        public string ClientMessage { get; set; }

        public byte TurnState { get; set; }

        public JSONObjects()
        {
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static JSONObjects Deserialize(string jsonString)
        {
            return JsonConvert.DeserializeObject<JSONObjects>(jsonString);
        }
    }
    #endregion
    partial class WebSocketApp
    {
        public enum eClientConnectionStatus { STARTING, ESTABLISHING, ESTABLISHED, CONNECTED, DISCONNECTED, STOPPED, ERROR };
        public Dictionary<Socket, bool> ChowkaWebSocket = new Dictionary<Socket, bool>();
        public Dictionary<Socket, eClientConnectionStatus> ChowkaClientState = new Dictionary<Socket, eClientConnectionStatus>();
        //RoomID,State
        public Dictionary<int, eChowkaRoomState> ChowkaRoomState = new Dictionary<int, eChowkaRoomState>();
        public enum eChowkaRoomState { CREATED,STARTING,STARTED,DISTROYING,DISTROYED };
        //RoomID,No. of users
        public Dictionary<int, byte> ChowkaRoom = new Dictionary<int, byte>();
        public const byte MaxPlayer = 4;

        public void CreateJoinRoom(Socket CreateJoinSocket,JSONObjects HandShake)
        {
            bool isNewRoom = true;
            int RoomID = 0;
            foreach (var room in ChowkaRoom)
            {
                if (room.Value < MaxPlayer)
                {
                    if (ChowkaRoomState[room.Key] == eChowkaRoomState.CREATED)
                    {
                        isNewRoom = false;
                        RoomID = room.Key;
                        ChowkaRoom[room.Key]++;
                        HandShake.WhoIAm = ChowkaRoom[room.Key];
                        if (ChowkaRoom[room.Key] == MaxPlayer)
                            ChowkaRoomState[room.Key] = eChowkaRoomState.STARTING;
                        foreach (var roomindex in RoomKey)
                        {
                            if (roomindex.Value == RoomID)
                            {
                                SocketKey.Add(CreateJoinSocket, roomindex.Key);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            if (isNewRoom)
            {
                RoomID = GenerateRoomNo();
                RoomKey.Add(RoomIndex, RoomID);
                SocketKey.Add(CreateJoinSocket, RoomIndex);
                ChowkaRoom.Add(RoomID, 1);
                ChowkaRoomState.Add(RoomID,eChowkaRoomState.CREATED);
                
                RoomIndex++;
                HandShake.WhoIAm = 1;
            }
            HandShake.RoomID = RoomID;

                        
        }
        public void HandleChowkaWebSocket(Socket WebSocket,string ReceivedData)
        {
            Console.WriteLine("SoketState:" + ChowkaClientState[WebSocket]);
            if (ChowkaClientState[WebSocket] == eClientConnectionStatus.ESTABLISHED)
            {
                JSONObjects HandShake = Newtonsoft.Json.JsonConvert.DeserializeObject<JSONObjects>(ReceivedData);
                if (HandShake.HandShake)
                {
                    CreateJoinRoom(WebSocket, HandShake);
                    WebSocket.Send(Send(HandShake.ToJsonString()));
                    Console.WriteLine("Sent:" + HandShake.ToJsonString());
                }
                else if (HandShake.ClientMessage == "ACK")
                {
                    ChowkaClientState[WebSocket] = eClientConnectionStatus.CONNECTED;
                    HandShake.ServerMessage = "ACK";
                    WebSocket.Send(Send(HandShake.ToJsonString()));
                    Console.WriteLine("Sent ACK:" + HandShake.ToJsonString());
                    
                }
            }
            if (ChowkaClientState[WebSocket] == eClientConnectionStatus.CONNECTED)
            {
                JSONObjects ConnectedState = Newtonsoft.Json.JsonConvert.DeserializeObject<JSONObjects>(ReceivedData);
                if (ChowkaRoomState[ConnectedState.RoomID] == eChowkaRoomState.STARTING)
                {
                    if (ConnectedState.ClientMessage != "STARTED")
                    {
                        ConnectedState.ServerMessage = "START";
                        WebSocket.Send(Send(ConnectedState.ToJsonString()));
                        SendToAllExceptOne("START", WebSocket, ConnectedState.RoomID);
                    }
                    else
                    {
                        ChowkaRoomState[ConnectedState.RoomID] = eChowkaRoomState.STARTED;
                    }
                }
                else if (ChowkaRoomState[ConnectedState.RoomID] == eChowkaRoomState.STARTED)
                {
                    SendToAllExceptOne(ReceivedData, WebSocket, ConnectedState.RoomID);
                }
            }
        }

    }
}
