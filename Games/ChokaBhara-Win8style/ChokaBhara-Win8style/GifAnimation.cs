using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Interop;

namespace ChokaBharaWin8Style
{
    public partial class MainWindow 
    {
        private void GifActions()
        {
            ServerConnect.LoadedBehavior = MediaState.Manual;
            ServerConnect.Play();
            ServerConnect.MediaEnded += new RoutedEventHandler(GIF_MediaEnded);
            WaitingGIF.LoadedBehavior = MediaState.Manual;
            WaitingGIF.Play();
            WaitingGIF.MediaEnded += new RoutedEventHandler(GIF_MediaEnded);
        }
        private void GIF_MediaEnded(object sender, RoutedEventArgs REA)
        {
            if (sender.Equals(ServerConnect))
            {
                if (ServerConnectionStatus != (UInt16)eServerConnectionStatus.CONNECTED)
                {
                    ServerConnect.Visibility = Visibility.Visible;
                    ServerConnect.LoadedBehavior = MediaState.Manual;
                    ServerConnect.Position = TimeSpan.FromSeconds(5);
                    ServerConnect.Play();

                }
                else
                {
                    ServerConnect.Stop();
                    ServerConnect.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                if (PlayerStatus == (UInt16)ePlayerStatus.OthersTurn)
                {
                    WaitingGIF.Visibility = Visibility.Visible;
                    WaitingGIF.LoadedBehavior = MediaState.Manual;
                    WaitingGIF.Position = TimeSpan.FromSeconds(5);
                    WaitingGIF.Play();

                }
                else
                {
                    WaitingGIF.Stop();
                    WaitingGIF.Visibility = Visibility.Hidden;

                }
            }
        }
    }
}
