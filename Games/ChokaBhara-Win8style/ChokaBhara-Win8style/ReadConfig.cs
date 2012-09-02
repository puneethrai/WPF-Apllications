using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace ChokaBhara_Win8style
{
    
    public partial class MainWindow
    {
        public string ServerAddress = null;
        public int ServerPort = 0;
        public void ReadConfig()
        {
            ServerAddress = ConfigurationManager.AppSettings["ServerAddress"];
            ServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["ServerPort"]);
        }

    }
}
