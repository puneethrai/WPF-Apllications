using System;
using System.Threading;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChokaBhara_Win8style
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

            while (TimeTicked <= (TimeoutTime*10) && !isMoved) ;

            Timeout.Dispose();
            TimedOut = true;
            if (!isMoved)
            {
                System.Windows.Forms.MessageBox.Show("Timed Out");
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
            TimerThread = null;
        }
    }
}
