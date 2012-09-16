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
            
            while (TimeTicked <= (TimeoutTime*10) && !isMoved && !AppExited) ;

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
