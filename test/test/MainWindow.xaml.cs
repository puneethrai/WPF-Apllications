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
using System.Windows.Forms;
using System.Drawing;
using System.Web.Script.Serialization;


namespace test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon MyNotifyIcon;
        

        public MainWindow()
        {
            InitializeComponent();
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            MyNotifyIcon.Icon = new System.Drawing.Icon(
                    @"..\..\autorun.ico");
        MyNotifyIcon.MouseDoubleClick += 
        new System.Windows.Forms.MouseEventHandler
            (MyNotifyIcon_MouseDoubleClick);
            this.StateChanged += Window_StateChanged;
            var json = "[{\"id\":\"588\",\"value\":false},{\"id\":\"486\",\"value\":false}]";
            var jss = new JavaScriptSerializer();
            var dic = jss.Deserialize<dynamic>(json);
            Console.WriteLine(dic[0]["id"]);
            System.Windows.MessageBox.Show(dic[0]["id"]);

            json = jss.Serialize(dic);
            System.Windows.MessageBox.Show(json);
            
            
        }
        void MyNotifyIcon_MouseDoubleClick(object sender, 
            System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("Called");
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
