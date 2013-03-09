using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Debug
{
    public class TestScript
    {
        #region private
        private int totalTest;
        private int successTest;
        private int failureTest;
        private Debug log = null;
        private bool isDisposed = false;
        #endregion
        #region public
        public TestScript(Debug obj)
        {
            totalTest = 0;
            successTest = 0;
            failureTest = 0;
            log = obj;
        }
        public void Test(dynamic input, dynamic expected, string customMessage)
        {
            
            if (customMessage == null)
                customMessage = "";
            
            if (input.GetType() == expected.GetType() && input == expected)
            {
                log.Print("Test :" + (totalTest++) + " " + customMessage + " Success", Debug.elogLevel.ALL);
                successTest++;
            }
           
            else
            {
                log.Print("Test :" + (totalTest++) + " " + customMessage + " Failure" + Environment.NewLine + "Reason: Expected " + expected +
                    " Got:" + input, Debug.elogLevel.ALL);
                failureTest++;
            }
        }
        public string Result()
        {
            return "Total test: " + totalTest + " Successfull: " + successTest + " Failure:" + failureTest;
        }
        public void FinalReport()
        {
            log.Print(Result(), Debug.elogLevel.ALL);
            Dispose();
            isDisposed = true;
        }
        public void Dispose()
        {
            log = null;
        }
        #endregion
    }
}
