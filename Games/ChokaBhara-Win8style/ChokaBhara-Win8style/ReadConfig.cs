using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace ChokaBharaWin8Style
{
    
    public partial class MainWindow
    {
        private string ServerAddress = null;
        private int ServerPort = 0;
        private uint TimeoutTime;  
        public void ReadConfig()
        {
            ServerAddress = ConfigurationManager.AppSettings["ServerAddress"];
            ServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["ServerPort"]);
            
        }
        public void Init()
        {
            TimeoutTime = (uint) Convert.ToInt32(ConfigurationManager.AppSettings["TimeoutTime"]);
        }
    }
}
