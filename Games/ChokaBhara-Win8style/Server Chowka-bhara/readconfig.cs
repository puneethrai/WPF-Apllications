using System.Configuration;


namespace Server_Chowka_bhara
{
    public partial class ServerUI
    {
        private short serverPort = 8080;
        private short pingDuration = 25;
        private uint MaxRoom = 100;
        private uint MaxPeer = 4;
        private uint MinPeer = 2;
        private uint MaxUser;
        private void ReadConfig()
        {
            if (!short.TryParse(ConfigurationSettings.AppSettings["ServerPort"], out serverPort))
                serverPort = 8080;
            if (!short.TryParse(ConfigurationSettings.AppSettings["PingDuration"], out pingDuration))
                pingDuration = 25;
            if (!uint.TryParse(ConfigurationSettings.AppSettings["MaxRoom"], out MaxRoom))
                MaxRoom = 100;
            if (!uint.TryParse(ConfigurationSettings.AppSettings["MaxPlayer"], out MaxPeer))
                MaxPeer = 4;
            if (!uint.TryParse(ConfigurationSettings.AppSettings["MinPlayer"], out MinPeer))
                MinPeer = 2;
            if (!uint.TryParse(ConfigurationSettings.AppSettings["MaxUser"], out MaxUser))
                MaxUser = 200;    
         
        }
    }
}
