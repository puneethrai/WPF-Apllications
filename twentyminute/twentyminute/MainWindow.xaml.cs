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
using System.Windows.Threading;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace twentyminute
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region private
        private NotifyIcon MyNotifyIcon;
        private DispatcherTimer TwentyTwentyTimer;
        private DispatcherTimer TwentyTimer;
        private DispatcherTimer MinuteTimer;
        private int count = 0;
        private int MinuteCount = 0;
        private const int TWENTY = 20;
        private const int DELAY = 1;
        private SolidColorBrush mySolidColorBrush;
        private bool StopBtnPressed = false;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.ResizeMode=ResizeMode.CanMinimize;
            
 
        }
        /// <summary>
        /// Apllication gets loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
            this.SizeToContent = System.Windows.SizeToContent.Manual;
        }
        /// <summary>
        /// Initializing Timer & State change component
        /// </summary>
        private void Init()
        {
            
            mySolidColorBrush = new SolidColorBrush();
            //  DispatcherTimer setup
            TwentyTwentyTimer = new DispatcherTimer();
            
            TwentyTwentyTimer.Interval = new TimeSpan(0, TWENTY, DELAY);
            TwentyTwentyTimer.Start();
            // Updating the Label which displays the countdown 20 second
            TwentyTimer = new DispatcherTimer();
            
            TwentyTimer.Interval = new TimeSpan(0, 0, DELAY);

            MinuteTimer = new DispatcherTimer();
            MinuteTimer.Interval = new TimeSpan(0, DELAY, 0);
            MinuteTimer.Start();

            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            MyNotifyIcon.Icon = new System.Drawing.Icon(@"autorun.ico");
            System.Windows.Forms.ContextMenu MenuItems = new System.Windows.Forms.ContextMenu();
            MenuItems.MenuItems.Add(0,
                new System.Windows.Forms.MenuItem("Exit", new EventHandler((object sender, EventArgs e) =>
                    {
                        System.Windows.Forms.Application.Exit();
                    })));
            MyNotifyIcon.ContextMenu = MenuItems;
            MyNotifyIcon.Text = "Twenty Twenty eye relaxation app" ;

            #region EventHandlers

            TwentyTwentyTimer.Tick += new EventHandler(TwentyTwentyInvoke);
            TwentyTimer.Tick += new EventHandler(TwentyInvoke);
            MyNotifyIcon.MouseDoubleClick +=
                new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);
            this.StateChanged += Window_StateChanged;
            System.Windows.Forms.Application.ApplicationExit += 
                new EventHandler(Application_ApplicationExit);
            MinuteTimer.Tick += new EventHandler((object sender, EventArgs e) =>
            {
                MinuteCount++;
                DisplayLabel.Content = "Continue Working For Another "+ (TWENTY - MinuteCount)+" min";
                MinuteLabel.Content = "Minutes Left:" + (TWENTY - MinuteCount);
            });

            #endregion
        }
        /// <summary>
        /// Triggered when application is about to exit
        /// </summary>
        void Application_ApplicationExit(object sender, EventArgs e)
        {
            MyNotifyIcon.Visible = false;
            MyNotifyIcon.Dispose();
            
            System.Windows.Application.Current.Shutdown();
        }


        /// <summary>
        /// Invoked after twenty minute & twenty second
        /// </summary>
        private void TwentyTwentyInvoke(object sender, EventArgs e)
        {

            Console.Beep(440, 1000);
            TwentyTimer.Start();
            MinuteTimer.Stop();
            TwentyTwentyTimer.Stop();
            WindowState = WindowState.Maximized;
            this.Topmost = true;
            this.ResizeMode=ResizeMode.NoResize;
            MinimizeBtn.IsEnabled = false;
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }
        /// <summary>
        /// Invoked every second for twenty second
        /// </summary>
        private void TwentyInvoke(object sendobj, EventArgs eventarg)
        {
            if(this.WindowState != WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            count++;
            if (20 < count)
            {
                count = 0;
                MinuteCount = 0;
                TwentyTimer.Stop();
                MinuteTimer.Start();
                TwentyTwentyTimer.Start();
                this.WindowState = WindowState.Normal;
                this.WindowState = WindowState.Minimized;
                this.ResizeMode=ResizeMode.CanMinimize;
                MinimizeBtn.IsEnabled = true;
                DisplayLabel.Content = "Continue Working For Another 20 min";
                TimeLabel.Content = "Seconds Left:0";
               
                Console.Beep(440, 1000);
            }
            
            mySolidColorBrush.Color = System.Windows.Media.Color.FromRgb((byte)(((count + 1 / 15) * 255) - 1),
                (byte)(((count + 1 / 10) * 128) - 1), (byte)(((count + 1 / 5) * 64) - 1));
            TimeLabel.Foreground = mySolidColorBrush;
            DisplayLabel.Content = "See a 20ft distant object for "+(TWENTY - count) +"s";
            TimeLabel.Content = "Seconds Left:"+(TWENTY - count);
        }
        /// <summary>
        /// Triggered when app is double clicked in system tray icon
        /// </summary>
        void MyNotifyIcon_MouseDoubleClick(object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }
        /// <summary>
        /// Triggered when Window State has been changed & enables or disables system tray icon based on window state
        /// </summary>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                MyNotifyIcon.BalloonTipTitle = "App still running in background";
                MyNotifyIcon.BalloonTipText = 
                    "Right click & press \"Exit\" to exit the app";
                MyNotifyIcon.ShowBalloonTip(400);
                MyNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                MyNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
            
        }
        /// <summary>
        /// Triggered Stop button is clicked
        /// </summary>
        private void StopBtn_Clicked( object sender, RoutedEventArgs e )
        {
            if(!StopBtnPressed)
            {
                MinimizeBtn.IsEnabled = true;
                TwentyTwentyTimer.Stop();
                TwentyTimer.Stop();
                MinuteTimer.Stop();
                StopBtnPressed = true;
                DisplayLabel.Content = "Timers Stoped";
                this.Background = System.Windows.Media.Brushes.Red;
                ResizeMode = ResizeMode.CanMinimize;
            }
        }
        /// <summary>
        /// Triggered Start button is clicked
        /// </summary>
        private void StartBtn_Clicked( object sender, RoutedEventArgs e )
        {
            if(StopBtnPressed)
            {
                TwentyTwentyTimer.Start();
                MinuteTimer.Start();
                count = 0;
                MinuteCount = 0;
                StopBtnPressed = false;
                DisplayLabel.Content = "Timers Started";
                this.Background = System.Windows.Media.Brushes.White;
            }
        }
        /// <summary>
        /// Triggered when user presses Help button(F1)
        /// </summary>
        private void HelpExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"help.chm");
           
        }
        /// <summary>
        /// Used for dragging the window
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }
        /// <summary>
        /// CLoses the application when close button is pressed
        /// </summary>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        /// <summary>
        /// Minimizes the application when minimize button is pressed
        /// </summary>
        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
    }
}
