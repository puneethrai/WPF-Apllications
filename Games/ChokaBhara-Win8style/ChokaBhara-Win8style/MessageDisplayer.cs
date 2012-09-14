using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ChokaBharaWin8Style
{
    class MessageDisplayer : MainWindow
    {
        public Label DisplayWindow;
        /// <summary>
        /// Displays Message on to appropriate window else pops out message box
        /// </summary>
        /// <param name="Message">Message to be displayed</param>
        public void Display(string Message)
        {
            if (DisplayWindow != null)
            {
                if (!DisplayWindow.Dispatcher.CheckAccess())
                {
                    DisplayWindow.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        DisplayWindow.Content = "";
                        DisplayWindow.Content = Message;
                    }, null);
                }
                else
                {
                    DisplayWindow.Content = "";
                    DisplayWindow.Content = Message;
                }
                
            }
            else
            {
                MessageBox.Show(Message);
            }
        }
        /// <summary>
        /// Displays Message on to appropriate window else pops out message box
        /// </summary>
        /// <param name="Message">Message to be displayed</param>
        /// <param name="Color">Display Color</param>
        public void Display(string Message,Brush Color)
        {
            if (DisplayWindow != null)
            {
                if (!DisplayWindow.Dispatcher.CheckAccess())
                {
                    DisplayWindow.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        DisplayWindow.Content = "";
                        DisplayWindow.Content = Message;
                        DisplayWindow.Foreground = Color;
                    }, null);
                }
                else
                {
                    DisplayWindow.Content = "";
                    DisplayWindow.Content = Message;
                    DisplayWindow.Foreground = Color;
                }
                
            }
            else
            {
                MessageBox.Show(Message);
            }
        }
        /// <summary>
        /// Displays Message on to appropriate window for given Duration else pops out message box
        /// </summary>
        /// <param name="Message">Message to be displayed</param>
        /// <param name="Duration">Duration in msec to be displayed</param>
        public void Display(string Message,long Duration)
        {
            AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
            if (DisplayWindow != null)
            {
                if (!DisplayWindow.Dispatcher.CheckAccess())
                {
                    DisplayWindow.Dispatcher.BeginInvoke((Action)delegate()
                        {
                            DisplayWindow.Opacity = 1;
                            DisplayWindow.Content = "";
                            DisplayWindow.Content = Message;
                        }, null);
                }
                else
                {
                    DisplayWindow.Opacity = 1;
                    DisplayWindow.Content = "";
                    DisplayWindow.Content = Message;
                }
                Console.WriteLine(Message);
                Console.WriteLine(DisplayWindow+":"+Duration);
                Timer Timeout = new Timer((object state) =>
                {
                    try
                    {
                        DisplayWindow.Dispatcher.BeginInvoke((ThreadStart)(() =>
                        {

                            DisplayWindow.Content = "";
                            _autoResetEvent.Set();
                        }), null);
       
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    

                }, null, 0, Duration);
                Thread th = new Thread(() =>
                {
                    _autoResetEvent.WaitOne();
                    Timeout.Dispose();
                });
                th.Name = "Displayer Sync thread";
                th.Start();
            }
            else
            {
                MessageBox.Show(Message);
            }
        }
        public MessageDisplayer()
        {
        }
    }
}
