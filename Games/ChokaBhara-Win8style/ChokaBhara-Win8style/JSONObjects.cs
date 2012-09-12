using Newtonsoft.Json;

namespace ChokaBharaWin8Style
{
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
        public int WhoIAm { get; set; }

        [JsonProperty]
        public string ClientVersion { get; set; }

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
