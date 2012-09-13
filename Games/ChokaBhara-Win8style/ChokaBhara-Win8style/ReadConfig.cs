using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Shapes;
namespace ChokaBharaWin8Style
{
    
    public partial class MainWindow
    {
        private string ServerAddress = null;
        private int ServerPort = 0;
        private uint TimeoutTime;
        private const uint MinPlayer = 0;
        private uint MaxPlayer = 4;
        private const uint MinKayi = 0;
        private const uint MaxKayi = 4;
        private const uint MaxMoves = 25;
        
        public void ReadConfig()
        {
            ServerAddress = ConfigurationManager.AppSettings["ServerAddress"];
            ServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["ServerPort"]);
            TimeoutTime = (uint)Convert.ToInt32(ConfigurationManager.AppSettings["TimeoutTime"]);
            MaxPlayer = (uint)Convert.ToInt32(ConfigurationManager.AppSettings["MaxPlayer"]);
            
        }
        public void Init()
        {
            MyKayi = new uint[MaxKayi, MaxKayi];
            ScoreCard = new uint[MaxKayi];
            KayiPlaced = new Rectangle[MaxKayi, MaxKayi];
            WinnerDisplayed = new bool[MaxKayi];
            
        }
    }
}
