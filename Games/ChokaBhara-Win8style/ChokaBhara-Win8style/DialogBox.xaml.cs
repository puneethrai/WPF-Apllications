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
using System.Windows.Shapes;

namespace ChowkaBaraWin8Style
{
    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogBox : Window
    {
        public DialogBox(string Display,string WindowName = "DialogBox")
        {
            InitializeComponent();
            this.WindowTitle.Content = WindowName;
            this.Display.Content = Display;

        }

        private EventHandler on_Confirm;
        private void ConfirmClick()
        {
            if (on_Confirm == null)
                return;
            on_Confirm(this, new EventArgs());
        }
        public event EventHandler onConfirm
        {
            add { on_Confirm += value; }
            remove { on_Cancel += value; }
        }

        private EventHandler on_Cancel;
        private void CancelClick()
        {
            if (on_Cancel == null)
                return;
            on_Cancel(this, new EventArgs());
        }
        public event EventHandler onCancel
        {
            add { on_Cancel += value; }
            remove { on_Cancel += value; }
        }
        private void BtnClick(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(OK))
                ConfirmClick();
            else
                CancelClick();
            this.Close();
        }

        private void WindowTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
