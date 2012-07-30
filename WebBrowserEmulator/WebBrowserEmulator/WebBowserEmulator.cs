using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Drawing2D;
namespace WebBrowserEmulator
{
    public class ProgressBarEx : WebBowserEmulator
    {
        
    }
    public partial class WebBowserEmulator : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private const int No_of_Instance = 100;
        private const int No_of_Invokes = 10;
        private int Invokes = 0;
        WebBrowser[][] web = new WebBrowser[No_of_Invokes][];
        public WebBowserEmulator()
        {
            InitializeComponent();
            for(int i = 0 ;i< No_of_Invokes;i++)
            {
                web[i] = new WebBrowser[No_of_Instance]; 
            }
            
            // Create a simple tray menu with only two item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Emulate", OnEmulate);
            trayMenu.MenuItems.Add("Exit", OnExit);
            
            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Web Browser Emulator";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
            
            //InvokeprogressBar.BackColor = Color.Transparent;
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {

           // Visible = false; // Hide form window.
            //ShowInTaskbar = false; // Remove from taskbar.
            //base.OnLoad(e);
        }
        private void OnEmulate(object sender, EventArgs e)
        {
            WebprogressBar.Value = 0;
            if (Invokes == (No_of_Invokes-1))
            {

                MessageBox.Show("Can't Emulate futher");
            }
            else
            {
                InvokeprogressBar.Value = (int)(((float)(Invokes + 1) / (float)No_of_Invokes) * 100);
               
                
                for (int i = 0; i < No_of_Instance; i++)
                {
                    web[Invokes][i] = new WebBrowser();
                    web[Invokes][i].Navigate("http://10.75.15.120/webinstance.html");
                    
                    WebprogressBar.Value = (int)(((float)(i+1) / (float)No_of_Instance) * 100);
                    Thread.Sleep(500);

                }
                Invokes++;
            }
        }
        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }
/*        public void ProgressBarEx()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            LinearGradientBrush brush = null;

            Rectangle rec = new Rectangle(0, 0, this.Width, this.Height);

            if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rec);

            rec.Width = (int)(rec.Width * ((double)Invokes / 100)) - 4;
            rec.Height -= 4;
            brush = new LinearGradientBrush(rec, this.ForeColor, this.BackColor, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, 2, 2, rec.Width, rec.Height);
        }


        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }
 * */
    }


}

/*
namespace MyTrayApp
{
    public class SysTrayApp : Form
    {
        [STAThread]
        public static void Main()
        {
            Application.Run(new SysTrayApp());
        }

        private NotifyIcon  trayIcon;
        private ContextMenu trayMenu;

        public SysTrayApp()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon      = new NotifyIcon();
            trayIcon.Text = "MyTrayApp";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible     = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible       = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}
*/