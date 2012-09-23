using System;
using System.Threading;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChowkaBaraWin8Style
{
    public partial class MainWindow
    {
        void TimerStart()
        {
            int TimeTicked = 0;
            Timer Timeout = new Timer((object state) =>
            {
                TimeTicked++;

                TimeOutBar.Dispatcher.BeginInvoke((ThreadStart)delegate()
                {

                    TimeOutBar.Height = TimeOutBarGridHeight;
                    TimeOutBar.Width = (TimeTicked * (int)TimeOutBarGridWidth) / (TimeoutTime * 10);


                }, null);

            }, null, 0, 100);

            while (TimeTicked <= (TimeoutTime * 10) && !isMoved && !AppExited)
            {
                Thread.Sleep(100);
            }

            Timeout.Dispose();
            TimedOut = true;
            if (!AppExited)
            {
                if (!isMoved)
                {
                    Display("Timed Out", TurnFill[TurnState]);
                    KayiGrid.Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
                        KayiGrid.Children.Remove(TempStackpanel);
                        KayiGrid.Background = Brushes.White;
                    }, null);
                    if (ServerConnectionStatus == (ushort)eServerConnectionStatus.CONNECTED && PlayerStatus == ePlayerStatus.HisTurn)
                    {
                        SendObject.ClientMessage = "KAYIMOVE";
                        SendObject.TurnState = (byte)TurnState;
                        SendObject.KayiMove = 0;
                        SendObject.KayiNo = 0;
                        ws.Send(SendObject.ToJsonString());
                    }
                    Turn();

                }
                TimeOutBar.Dispatcher.BeginInvoke((ThreadStart)delegate()
                {

                    TimeOutBar.Height = 0;
                    TimeOutBar.Width = 0;
                }, null);
            }
            TimerThread = null;
        }
    }
}
