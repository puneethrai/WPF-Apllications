using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Controls;
namespace ChowkaBaraWin8Style
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
        private string ClientVersion = null;
       /// <summary>
       /// Reads the value from configuration file
       /// </summary>
        public void ReadConfig()
        {
            ServerAddress = ConfigurationManager.AppSettings["ServerAddress"];
            ServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["ServerPort"]);
            TimeoutTime = (uint)Convert.ToInt32(ConfigurationManager.AppSettings["TimeoutTime"]);
            MaxPlayer = (uint)Convert.ToInt32(ConfigurationManager.AppSettings["MaxPlayer"]);
            
        }
        /// <summary>
        /// Initialization of all non initialized objects
        /// </summary>
        public void Init()
        {
            MyKayi = new uint[MaxKayi, MaxKayi];
            ScoreCard = new uint[MaxKayi];
            KayiPlaced = new Rectangle[MaxKayi, MaxKayi];
            WinnerDisplayed = new bool[MaxKayi];
            DisplayWindow = MessageBoxLabel;
            ClientVersion = System.Windows.Forms.Application.ProductVersion;
            Display("Can't move to home when dice is 4 or 8 & last kayi", TurnFill[TurnState],5000);
        }
    }
}
