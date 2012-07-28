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

            TitleGrid.Children.Add(TitleBarStack);
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(@"..\..\autorun.ico",UriKind.Relative);
            logo.DecodePixelHeight = 19;
            logo.DecodePixelWidth = 19;
            logo.EndInit();
            AppIcon.Source = logo;*/
        }

        #region WindowControRegion

        private void TitleGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void WinControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender.Equals(MinimizeLine)||sender.Equals(MinimizeRect))
            {
                Console.WriteLine("Minimize");
                MinimizeLine.Stroke = Brushes.Blue;
            }
            else
            {
                Console.WriteLine("Close");
                CloseLine1.Stroke = Brushes.Red;
                CloseLine2.Stroke = Brushes.Red;
            }
        }

        private void WinControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender.Equals(MinimizeLine) || sender.Equals(MinimizeRect))
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
            if (sender.Equals(MinimizeLine) || sender.Equals(MinimizeRect))
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
            if (sender.Equals(MinimizeLine) || sender.Equals(MinimizeRect))
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
            Console.WriteLine("Loaded");
            //this.pictureBoxLoading.Image = System.Drawing.Image.FromFile(@"C:\Users\gangathara rai\Documents\GitHub\WPF-Apllications\Games\ChokaBhara-Win8style\ChokaBhara-Win8style\animated_loader.gif");
            // Create a collection of points for a polygon
             System.Windows.Point Point1 = new System.Windows.Point(60,40);
             System.Windows.Point Point2 = new System.Windows.Point(70,50);
             System.Windows.Point Point3 = new System.Windows.Point(80,40);
             System.Windows.Point Point4 = new System.Windows.Point(60,40);
             PointCollection polygonPoints = new PointCollection();
             polygonPoints.Add(Point1);
             polygonPoints.Add(Point2);
             polygonPoints.Add(Point3);
             polygonPoints.Add(Point4);

             TurnDisplayTri.Points = polygonPoints;
            
             for (long i = 0; i < 999999999; i++) ;
             Point1 = new System.Windows.Point(120, 40);
             Point2 = new System.Windows.Point(130, 50);
             Point3 = new System.Windows.Point(140, 40);
             Point4 = new System.Windows.Point(120, 40);
             polygonPoints = new PointCollection();
             polygonPoints.Add(Point1);
             polygonPoints.Add(Point2);
             polygonPoints.Add(Point3);
             polygonPoints.Add(Point4);
             TurnDisplayTri.Points = polygonPoints;
             for (long i = 0; i < 999999999; i++) ;
             Point1 = new System.Windows.Point(180, 40);
             Point2 = new System.Windows.Point(190, 50);
             Point3 = new System.Windows.Point(200, 40);
             Point4 = new System.Windows.Point(180, 40);
             polygonPoints = new PointCollection();
             polygonPoints.Add(Point1);
             polygonPoints.Add(Point2);
             polygonPoints.Add(Point3);
             polygonPoints.Add(Point4);
             TurnDisplayTri.Points = polygonPoints;
             for (long i = 0; i < 999999999; i++) ;
             Point1 = new System.Windows.Point(0, 40);
             Point2 = new System.Windows.Point(10, 50);
             Point3 = new System.Windows.Point(20, 40);
             Point4 = new System.Windows.Point(0, 40);
             polygonPoints = new PointCollection();
             polygonPoints.Add(Point1);
             polygonPoints.Add(Point2);
             polygonPoints.Add(Point3);
             polygonPoints.Add(Point4);
             TurnDisplayTri.Points = polygonPoints;
             Console.WriteLine("DOne");
             System.Windows.Media.Brush brush = Background;
             if (m_grid.Background != System.Windows.Media.Brushes.Transparent && m_grid.Background != null)
                 brush = m_grid.Background;

             System.Windows.Media.SolidColorBrush scb = (System.Windows.Media.SolidColorBrush)brush;
             System.Drawing.Color color = System.Drawing.Color.White;
             if (scb != null)
                 color = System.Drawing.Color.FromArgb(scb.Color.A, scb.Color.R, scb.Color.G, scb.Color.B);

             System.Drawing.Brush solidBrush = new System.Drawing.SolidBrush(color);

             //
             AnimatedImageControl animatedImageControl =
                 new AnimatedImageControl(this, ChokaBhara_Win8style.Properties.Resources.animated_loader, solidBrush);
             m_grid.Children.Add(animatedImageControl);
             m_grid.HorizontalAlignment = HorizontalAlignment.Left;
             m_grid.VerticalAlignment = VerticalAlignment.Top;
             animatedImageControl.Margin = new Thickness(30, 10, 30, 10);
        }


    }
}
