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

namespace ChokaBhara_Win8style
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int TurnState;
        private int DiceNo;
        private Random DiceRand;
        public MainWindow()
        {
            InitializeComponent();
            DiceRand = new Random();
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
            //this.pictureBoxLoading.Image = System.Drawing.Image.FromFile(@"C:\Users\gangathara rai\Documents\GitHub\WPF-Apllications\Games\ChokaBhara-Win8style\ChokaBhara-Win8style\bin\Debug\animated_loader.gif");
            //System.Drawing.Bitmap animatedImage = new System.Drawing.Bitmap("SampleAnimation.gif");
            //System.Drawing.ImageAnimator.Animate(animatedImage, new EventHandler(this.OnFrameChanged));
            int i=0;
            ServerConnect.LoadedBehavior = MediaState.Manual;
            ServerConnect.Play();
            Uri Source = ServerConnect.Source;
            ServerConnect.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>((object ob, ExceptionRoutedEventArgs r) =>
            {
                Console.WriteLine("Error");
            });
            ServerConnect.MediaEnded += new RoutedEventHandler((object ob, RoutedEventArgs r) =>
            {
                //ServerConnect.Source = Source;
                ServerConnect.LoadedBehavior = MediaState.Manual;
                ServerConnect.Position = TimeSpan.FromSeconds(0);
                
               
                ServerConnect.Play();
                Console.WriteLine("Playing Again {0}",i);
                i++;
            });
            
        }
        
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Create a collection of points for a polygon
            Turn();
        }
        void Turn()
        {
            switch (TurnState)
            {
                case 0:
                    System.Windows.Point Point1 = new System.Windows.Point(60, 40);
                    System.Windows.Point Point2 = new System.Windows.Point(70, 50);
                    System.Windows.Point Point3 = new System.Windows.Point(80, 40);
                    System.Windows.Point Point4 = new System.Windows.Point(60, 40);
                    PointCollection polygonPoints = new PointCollection();
                    polygonPoints.Add(Point1);
                    polygonPoints.Add(Point2);
                    polygonPoints.Add(Point3);
                    polygonPoints.Add(Point4);

                    TurnDisplayTri.Points = polygonPoints;
                    TurnDisplayRect.Fill = turn2.Fill;
                    TurnDisplayTri.Fill = turn2.Fill;
                    TurnState++;
                    break;
                case 1:
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
                    TurnDisplayRect.Fill = turn3.Fill;
                    TurnDisplayTri.Fill = turn3.Fill;
                    TurnState++;
                    break;
                case 2:
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
                    TurnDisplayRect.Fill = turn4.Fill;
                    TurnDisplayTri.Fill = turn4.Fill;
                    TurnState++;
                    break;
                case 3:
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
                    TurnDisplayRect.Fill = Turn1.Fill;
                    TurnDisplayTri.Fill = Turn1.Fill;
                    TurnState = 0;
                    break;
            }
        }
        RadioButton TempRadio1;
        RadioButton TempRadio2;
        RadioButton TempRadio3;
        RadioButton TempRadio4;
        StackPanel TempStackpanel;
        private void DiceBtn_Click(object sender, RoutedEventArgs e)
        {
            DiceNo = DiceRand.Next(1, 8);
            DiceNoLabel.Content = "" + DiceNo;
            KayiGrid.Background = Brushes.Magenta;
            TempStackpanel = new StackPanel();
            KayiGrid.Children.Add(TempStackpanel);
            TempRadio1 = new RadioButton();
            TempRadio1.Name = "TempRadio1";
            TempRadio1.Content = "Kayi 1";
            TempRadio2 = new RadioButton();
            TempRadio2.Name = "TempRadio2";
            TempRadio2.Content = "Kayi 2";
            TempRadio3 = new RadioButton();
            TempRadio3.Name = "TempRadio3";
            TempRadio3.Content = "Kayi 3";
            TempRadio4 = new RadioButton();
            TempRadio4.Name = "TempRadio4";
            TempRadio4.Content = "Kayi 4";
            TempRadio1.Checked += new RoutedEventHandler(TempRadio_Checked);
            TempRadio2.Checked += new RoutedEventHandler(TempRadio_Checked);
            TempRadio3.Checked += new RoutedEventHandler(TempRadio_Checked);
            TempRadio4.Checked += new RoutedEventHandler(TempRadio_Checked);
            TempStackpanel.Children.Add(TempRadio1);
            TempStackpanel.Children.Add(TempRadio2);
            TempStackpanel.Children.Add(TempRadio3);
            TempStackpanel.Children.Add(TempRadio4);
            
        }
        Rectangle[,] MoveRect;
        void TempRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(TempRadio1))
            {
                if((bool)TempRadio1.IsChecked)
                MessageBox.Show("Checked 1");
            }
            else if (sender.Equals(TempRadio2))
            {
                if ((bool)TempRadio2.IsChecked)
                    MessageBox.Show("Checked 2");
            }
            else if (sender.Equals(TempRadio3))
            {
                if ((bool)TempRadio3.IsChecked)
                    MessageBox.Show("Checked 3");
            }
            else if (sender.Equals(TempRadio4))
            {
                if ((bool)TempRadio4.IsChecked)
                    MessageBox.Show("Checked 4");
            }
            
            KayiGrid.Children.Remove(TempStackpanel);
            KayiGrid.Background = Brushes.White;
           
            
            MoveRect = new Rectangle[,]
            {
                {R13,R12,R11,R21,R31,R41,R51,R52,R53,R54,R55,R45,R35,R25,R15,R14,R24,R34,R44,R43,R42,R32,R22,R23,R33},
                {R35,R25,R15,R14,R13,R12,R11,R21,R31,R41,R51,R52,R53,R54,R55,R45,R44,R43,R42,R32,R22,R23,R24,R34,R33},
                {R53,R54,R55,R45,R35,R25,R15,R14,R13,R12,R11,R21,R31,R41,R51,R52,R42,R32,R22,R23,R24,R34,R44,R43,R33},
                {R31,R41,R51,R52,R53,R54,R55,R45,R35,R25,R15,R14,R13,R12,R11,R21,R22,R23,R24,R34,R44,R43,R42,R32,R33}
            };

            // Create an Ellipse
            Ellipse blueRectangle = new Ellipse();
            blueRectangle.Height = 100;
            blueRectangle.Width = 200;

            // Create a blue and a black Brush
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Blue;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;

            // Set Ellipse's width and color
            blueRectangle.StrokeThickness = 4;
            blueRectangle.Stroke = blackBrush;
            // Fill rectangle with blue color
            blueRectangle.Fill = blueBrush;
            blueRectangle.HorizontalAlignment = MoveRect[0, 0].HorizontalAlignment;
            blueRectangle.VerticalAlignment = MoveRect[0, 0].VerticalAlignment;
             blueRectangle.Margin = MoveRect[0, 0].Margin;
            // Add Ellipse to the Grid.
            RectCanvas.Children.Add(blueRectangle);
            ThreadStart start = new ThreadStart(Start);
            Thread one = new Thread(start);
            one.Start();
            if (first)
            {

                first = false;
                
                temp1 = MoveRect[0, 0].Fill;
                temp2 = MoveRect[1, 0].Fill;
                temp3 = MoveRect[2, 0].Fill;
                temp4 = MoveRect[3, 0].Fill;
            }
        }
        bool first = true;
        Brush temp1;
        Brush temp2;
        Brush temp3;
        Brush temp4;
        Ellipse blueRectangle;
        void Start()
        {

            
            
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 25; j++)
                {

                    MoveRect[i, j].Dispatcher.BeginInvoke((ThreadStart)delegate()
                    {
                        //blueRectangle.HorizontalAlignment = MoveRect[i, j].HorizontalAlignment;
                        //blueRectangle.VerticalAlignment = MoveRect[i, j].VerticalAlignment;
                       // blueRectangle.Margin = MoveRect[i, j].Margin;
                        // Add Ellipse to the Grid.
                        //RectCanvas.Children.Add(blueRectangle);
                            switch (i)
                            {
                                case 0: MoveRect[i, DiceNo].Fill = temp1;
                                    break;
                                case 1: MoveRect[i, DiceNo].Fill = temp2;
                                    break;
                                case 2: MoveRect[i, DiceNo].Fill = temp3;
                                    break;
                                case 3: MoveRect[i, DiceNo].Fill = temp4;
                                    break;
                            }
                            
                           // RectCanvas.Children.Remove(blueRectangle);
                        });
                    for (long k = 0; k < 99999999; k++) ;
                }
                Dispatcher.BeginInvoke((ThreadStart)delegate()
                { Turn(); });
            }
            
        }


    }
}
