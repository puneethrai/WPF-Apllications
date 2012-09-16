using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ChowkaBaraWin8Style
{
    public partial class MainWindow 
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
                        DisplayWindow.Content = Message;
                        DisplayAnimation(false, 0);

                    }, null);
                }
                else
                {
                    DisplayWindow.Content = Message;
                    DisplayAnimation(false, 0);
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
        /// <param name="BrushColor">Display Color</param>
        public void Display(string Message,Brush BrushColor)
        {
            if (DisplayWindow != null)
            {
                if (!DisplayWindow.Dispatcher.CheckAccess())
                {
                    DisplayWindow.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        DisplayWindow.Content = Message;
                        DisplayWindow.Foreground = BrushColor;
                        DisplayAnimation(false, 0);

                    }, null);
                }
                else
                {
                    DisplayWindow.Content = Message;
                    DisplayWindow.Foreground = BrushColor;
                    DisplayAnimation(false, 0);
                }
                
            }
            else
            {
                MessageBox.Show(Message);
            }
        }
        /// <summary>
        /// Displays Message on to appropriate window for given Duration and Color else pops out message box
        /// </summary>
        /// <param name="Message">Message to be displayed</param>
        /// <param name="BrushColor">Display Color</param>
        /// <param name="Duration">Duration in msec to be displayed</param>
        public void Display(string Message, Brush BrushColor,int Duration)
        {
            if (DisplayWindow != null)
            {
                if (!DisplayWindow.Dispatcher.CheckAccess())
                {
                    DisplayWindow.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        DisplayWindow.Content = Message;
                        DisplayWindow.Foreground = BrushColor;
                        DisplayAnimation(false, Duration);

                    }, null);
                }
                else
                {
                    DisplayWindow.Content = Message;
                    DisplayWindow.Foreground = BrushColor;
                    DisplayAnimation(false, Duration);
                }
                DisplayAnimation(true, Duration);
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
        public void Display(string Message,int Duration)
        {
            if (DisplayWindow != null)
            {
                if (!DisplayWindow.Dispatcher.CheckAccess())
                {
                    DisplayWindow.Dispatcher.BeginInvoke((Action)delegate()
                        {
                            DisplayWindow.Content = Message;
                            DisplayAnimation(false, Duration);
                            
                        }, null);
                }
                else
                {
                    DisplayWindow.Content = Message;
                    DisplayAnimation(false, Duration);
                }
                DisplayAnimation(true, Duration);
            }
            else
            {
                MessageBox.Show(Message);
            }
        }
        /// <summary>
        /// Fade IN & Fade OUT animation for Display message box
        /// </summary>
        /// <param name="FadeOut">Boolean to either Fade OUT or Fade IN</param>
        /// <param name="Duration">Duration in msec after which fad IN or fade OUT should take place</param>
        public void DisplayAnimation(bool FadeOut,int Duration)
        {
            Storyboard s = null;
            Thread Animation = new Thread(() =>
            {
                if (!FadeOut)
                    Duration = 100;
                Thread.Sleep(Duration);
                DisplayWindow.Dispatcher.BeginInvoke((Action)delegate()
                {
                    if (FadeOut)
                    {
                        s = (Storyboard)TryFindResource("MessageBoxFadeOut");
                        s.Begin();	// Start animation
                        DisplayWindow.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    else
                    {
                        s = (Storyboard)TryFindResource("MessageBoxFadeIn");
                        s.Begin();	// Start animation
                    }
                }, null);

            });
            Animation.Name = "Animation Thread";
            Animation.Start();
        }
        
       
    }
}
