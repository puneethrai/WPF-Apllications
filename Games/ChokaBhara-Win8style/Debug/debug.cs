using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Debug
{
    public abstract class Debug
    {
        public enum eproductLevel { PRODUCTION = 0, DEVELOPMENT = 1 };
        public enum elogLevel { ERROR = 0, WARNING = 1, INFO = 2, ALL = 3, NONE = 4 };
        public abstract void Print(string debugMessage, elogLevel logLevel);
        public abstract void ChangeLogLevel(elogLevel toChange);
        public abstract void Close();
    }
    public class Log:Debug
    {
        private Int16 glogLevel = 0;
        private Int16 gproductLevel = 0;
        private string gloggerName = null;
        private FileStream gfileDiscriptor = null;
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="loggerName">Name of the logger</param>
        /// <param name="logLevel">Log level to be used</param>
        /// <param name="productLevel">Production level of log i.e., weather to use file(PRODUCTION) or console(DEVELOPMENT) </param>
        public Log(string loggerName, elogLevel logLevel,eproductLevel productLevel)
        {
            this.glogLevel = (Int16)logLevel;
            this.gproductLevel = (Int16)productLevel;
            this.gloggerName = loggerName;

        }
        public override void Print(string debugMessage,elogLevel logLevel)
        {
            
            if (this.glogLevel == (Int16)logLevel || this.glogLevel == (Int16)elogLevel.ALL)
            {
                DateTime currentTime = DateTime.Now;
                string printableTime = currentTime.ToString();
                string toLog = printableTime+":"+this.gloggerName+":"+debugMessage+"\n";
                if (this.gproductLevel == (Int16)eproductLevel.PRODUCTION)
                {
                    if (gfileDiscriptor == null)
                    {
                        gfileDiscriptor = File.Open("log.txt", FileMode.Append);
                        string initialLog = "\n-----------------"+printableTime+":"+this.gloggerName+"-----------------------\n";
                        gfileDiscriptor.Write(Encoding.ASCII.GetBytes(initialLog), 0, initialLog.Length);
                        gfileDiscriptor.Flush();
                        
                    }
                    if (gfileDiscriptor.CanWrite)
                    {
                        gfileDiscriptor.Write(Encoding.ASCII.GetBytes(toLog), 0, toLog.Length);
                        gfileDiscriptor.Flush();
                    }
                    
                }
                else if (this.gproductLevel == (Int16)eproductLevel.DEVELOPMENT)
                {
                    Console.WriteLine(toLog);
                }
            }
        }
        public override void ChangeLogLevel(elogLevel toChange)
        {
            this.glogLevel = (Int16)toChange;
        }
        public override void Close()
        {
            this.Print("Logger Existing", elogLevel.INFO);

            if (this.gproductLevel == (Int16)eproductLevel.PRODUCTION)
            {
                gfileDiscriptor.Close();
                gfileDiscriptor = null;
            }
        }
    }
}
