﻿using System;
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
        private int count = 0;
        private const int TWENTY = 20;
        private const int DELAY = 1;
        
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            
 
        }
        /// <summary>
        /// Apllication gets loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }
        /// <summary>
        /// Initializing Timer & State change component
        /// </summary>
        private void Init()
        {
            //  DispatcherTimer setup
            TwentyTwentyTimer = new DispatcherTimer();
            
            TwentyTwentyTimer.Interval = new TimeSpan(0, 0, TWENTY+DELAY);
            TwentyTwentyTimer.Start();
            // Updating the Label which displays the countdown 20 second
            TwentyTimer = new DispatcherTimer();
            
            TwentyTimer.Interval = new TimeSpan(0, 0, DELAY);
            
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            MyNotifyIcon.Icon = new System.Drawing.Icon(@"..\..\autorun.ico");

            #region EventHandlers

            TwentyTwentyTimer.Tick += new EventHandler(TwentyTwentyInvoke);
            TwentyTimer.Tick += new EventHandler(TwentyInvoke);
            MyNotifyIcon.MouseDoubleClick +=
                new System.Windows.Forms.MouseEventHandler(MyNotifyIcon_MouseDoubleClick);
            this.StateChanged += Window_StateChanged;
            System.Windows.Forms.Application.ApplicationExit += 
                new EventHandler(Application_ApplicationExit);

            #endregion
        }
        /// <summary>
        /// Triggered when application is about to exit
        /// </summary>
        void Application_ApplicationExit(object sender, EventArgs e)
        {
            MyNotifyIcon.Dispose();
        }


        /// <summary>
        /// Invoked after twenty minute & twenty second
        /// </summary>
        private void TwentyTwentyInvoke(object sender, EventArgs e)
        {
           
            TwentyTimer.Start();
            WindowState = WindowState.Maximized;
            this.Topmost = true;

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }
        /// <summary>
        /// Invoked every second for twenty second
        /// </summary>
        private void TwentyInvoke(object sendobj, EventArgs eventarg)
        {
            count++;
            if (20 < count)
            {
                count = 0;
                TwentyTimer.Stop();
                WindowState = WindowState.Minimized;
            }
            TimeLabel.Content = TWENTY - count;
        }
        void MyNotifyIcon_MouseDoubleClick(object sender,
            System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                MyNotifyIcon.BalloonTipTitle = "Minimize Sucessful";
                MyNotifyIcon.BalloonTipText = "Minimized the app ";
                MyNotifyIcon.ShowBalloonTip(400);
                MyNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                MyNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
            
        }
    }
}
