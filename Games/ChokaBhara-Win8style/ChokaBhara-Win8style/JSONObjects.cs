using Newtonsoft.Json;

namespace ChowkaBaraWin8Style
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JSONObjects
    {
        [JsonProperty]
        public int RoomID { get; set; }

        [JsonProperty]
        public bool HandShake { get; set; }

        [JsonProperty]
        public uint KayiNo { get; set; }

        [JsonProperty]
        public uint KayiMove { get; set; }

        [JsonProperty]
        public byte WhoIAm { get; set; }

        [JsonProperty]
        public string ClientVersion { get; set; }

        [JsonProperty]
        public string ServerMessage { get; set; }

        [JsonProperty]
        public string ClientMessage { get; set; }

        [JsonProperty]
        public byte TurnState { get; set; }

        [JsonProperty]
        public bool ChatMessage { get; set; }

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
}
