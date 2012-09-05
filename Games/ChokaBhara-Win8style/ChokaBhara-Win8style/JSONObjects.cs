﻿using Newtonsoft.Json;

namespace ChokaBhara_Win8style
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JSONObjects
    {
        [JsonProperty]
        public int RoomID { get; set; }

        [JsonProperty]
        public bool HandShake { get; set; }

        [JsonProperty]
        public int[] KayiNo { get; set; }

        [JsonProperty]
        public int KayiMove { get; set; }

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
