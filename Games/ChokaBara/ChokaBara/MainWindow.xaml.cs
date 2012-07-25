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

namespace ChokaBara
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region private
        private Random DiceNo = null;
        private int KayiMove;

        #endregion
        public MainWindow()
        {
            InitializeComponent();
            
            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        private void DiceBtn_Click(object sender, RoutedEventArgs e)
        {
                
                DiceValue.Content = "" + DiceNo.Next(1, 8);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }
        private void Init()
        {
            #region Allocation
            DiceNo = new Random();
            #endregion
        }
    }
}
