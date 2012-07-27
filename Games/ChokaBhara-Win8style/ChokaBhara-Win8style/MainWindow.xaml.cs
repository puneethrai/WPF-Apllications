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

namespace ChokaBhara_Win8style
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*
            StackPanel TitleBarStack = new StackPanel();
            TitleBarStack.Orientation = Orientation.Horizontal;
            Image myImage = new Image();
            BitmapImage myImageSource = new BitmapImage();
            myImageSource.BeginInit();
            myImageSource.UriSource = new Uri("autorun.ico");
            myImageSource.EndInit();
            myImage.Source = myImageSource;

            TextBlock myTextBlock = new TextBlock();
            myTextBlock.Text = "This is my image";

            TitleBarStack.Children.Add(myImage);
            TitleBarStack.Children.Add(myTextBlock);

            TitleGrid.Children.Add(TitleBarStack);*/
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(@"C:\Users\gangathara rai\Documents\GitHub\WPF-Apllications\Games\ChokaBhara-Win8style\ChokaBhara-Win8style\autorun.ico");
            
            logo.EndInit();
            AppIcon.Source = logo;
        }

        #region WindowControRegion

        private void WinControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender.Equals(MinimizeLine))
            {
                Console.WriteLine("Minimize");
                MinimizeLine.Stroke = Brushes.Blue;
            }
            else
            {
                Console.WriteLine("Close");
                CloseLine1.Stroke = Brushes.Blue;
                CloseLine2.Stroke = Brushes.Blue;
            }
        }

        private void WinControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender.Equals(MinimizeLine))
            {
                Console.WriteLine("Minimize");
                MinimizeLine.Stroke = Brushes.Black;
                MinimizeRect.Fill = Brushes.White;
            }
            else
            {
                Console.WriteLine("Close");
                CloseLine1.Stroke = Brushes.Black;
                CloseLine2.Stroke = Brushes.Black;
                CloseRect.Fill = Brushes.White;
             
            }
        }

        private void WinControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(MinimizeLine))
            {
                Console.WriteLine("Minimize");
                MinimizeRect.Fill = Brushes.White;
                MinimizeLine.Stroke = Brushes.Blue;
            }
            else
            {
                Console.WriteLine("Close");
                CloseRect.Fill = Brushes.White;
                CloseLine1.Stroke = Brushes.Red;
                CloseLine2.Stroke = Brushes.Red;
                
                App.Current.Shutdown();
            } 
        }

        private void WinControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(MinimizeLine))
            {
                Console.WriteLine("Minimize");
                MinimizeLine.Stroke = Brushes.White;
                MinimizeRect.Fill = Brushes.Blue;
            }
            else
            {
                Console.WriteLine("Close");
                CloseRect.Fill = Brushes.Red;
                CloseLine1.Stroke = Brushes.White;
                CloseLine2.Stroke = Brushes.White;
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    }
}
