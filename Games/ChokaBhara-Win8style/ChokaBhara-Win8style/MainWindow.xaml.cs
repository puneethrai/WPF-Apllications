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
// Assembly marked as compliant.
[assembly: CLSCompliant(true)]
namespace ChokaBharaWin8Style
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private uint TurnState;
        private uint DiceNo;
        private Random DiceRand;

        public UInt16 ServerConnectionStatus = 0;
        public enum eServerConnectionStatus { STARTING,ESTABLISHING,ESTABLISHED,CONNECTED,DISCONNECTED,STOPPED,ERROR };
        public UInt16 PlayerStatus = 0;
        public enum ePlayerStatus {HisTurn,OthersTurn };
        Rectangle[,] MoveRect = null;
        Ellipse[,] MoveKayi = null;
        Thread TimerThread = null;
        Brush[] TurnFill = new Brush[4];
        
        
        /// <summary> 
        /// Defines the program entry point. 
        /// </summary> 
        /// <param name="args">An array of <see cref="T:System.String"/> containing command line parameters.</param> 
        
        public MainWindow()
        {
            InitializeComponent();
            DiceRand = new Random();
            MoveRect = new Rectangle[,]
            {
                {R31,R41,R51,R52,R53,R54,R55,R45,R35,R25,R15,R14,R13,R12,R11,R21,R22,R23,R24,R34,R44,R43,R42,R32,R33},
                {R53,R54,R55,R45,R35,R25,R15,R14,R13,R12,R11,R21,R31,R41,R51,R52,R42,R32,R22,R23,R24,R34,R44,R43,R33},
                {R35,R25,R15,R14,R13,R12,R11,R21,R31,R41,R51,R52,R53,R54,R55,R45,R44,R43,R42,R32,R22,R23,R24,R34,R33},
                {R13,R12,R11,R21,R31,R41,R51,R52,R53,R54,R55,R45,R35,R25,R15,R14,R24,R34,R44,R43,R42,R32,R22,R23,R33}
            };
            MoveKayi = new Ellipse[,]
            {
                {CKayi11,CKayi12,CKayi13,CKayi14},
                {CKayi21,CKayi22,CKayi23,CKayi24},
                {CKayi31,CKayi32,CKayi33,CKayi34},
                {CKayi41,CKayi42,CKayi43,CKayi44}
            };
            
            TurnFill[0] = Turn1.Fill;
            TurnFill[1] = turn2.Fill;
            TurnFill[2] = turn3.Fill;
            TurnFill[3] = turn4.Fill;
            TimeOutBarGridHeight = TimeOutBarGrid.Height;
            TimeOutBarGridWidth = TimeOutBarGrid.Width;
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

        /// <summary> 
        /// Entry Point when Main window gets loaded. 
        /// </summary> 
        /// <param name="sender">Sender Object <see cref="T:System.object"/></param> 
        /// <param name="e">Routed Event Args <see cref="T:System.Windows.RoutedEventArgs"/></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Loaded");
            
            ReadConfig();
            GifActions();
            ConnectToServer();
            Init();
               
        }
        
        /*private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Create a collection of points for a polygon
            Turn();
            
        }*/
        public void Turn()
        {
            /*
             * Bug No. 4
             */
            if (DiceNo != 4 && DiceNo != 8)
            {
                TurnState = TurnState == 3 ? 0 : TurnState + 1;
                Point Point1 = new Point(0 + (TurnState * 60), 40);
                Point Point2 = new Point(10 + (TurnState * 60), 50);
                Point Point3 = new Point(20 + (TurnState * 60), 40);
                Point Point4 = new Point(0 + (TurnState * 60), 40);

                if (TurnDisplayTri.Dispatcher.CheckAccess())
                {
                    PointCollection polygonPoints = new PointCollection();
                    polygonPoints.Add(Point1);
                    polygonPoints.Add(Point2);
                    polygonPoints.Add(Point3);
                    polygonPoints.Add(Point4);
                    TurnDisplayTri.Points = polygonPoints;
                    TurnDisplayTri.Fill = TurnFill[TurnState];
                    TurnDisplayRect.Fill = TurnFill[TurnState];
                }
                else
                {
                    TurnDisplayTri.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        PointCollection PolygonPoints = new PointCollection();
                        PolygonPoints.Add(Point1);
                        PolygonPoints.Add(Point2);
                        PolygonPoints.Add(Point3);
                        PolygonPoints.Add(Point4);
                        TurnDisplayTri.Points = PolygonPoints;
                        TurnDisplayTri.Fill = TurnFill[TurnState];

                    }, null);

                    TurnDisplayRect.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        TurnDisplayRect.Fill = TurnFill[TurnState];

                    }, null);
                }
            }
        }
        RadioButton TempRadio1;
        RadioButton TempRadio2;
        RadioButton TempRadio3;
        RadioButton TempRadio4;
        StackPanel TempStackpanel;
        private void DiceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (null == TimerThread)
            {
                /*
                 * Bug No. 2
                 */
                DiceNo = 5;
                while(DiceNo==5||DiceNo==6||DiceNo==7)
                {
                    DiceNo = (uint)DiceRand.Next(1, 9);
                    Console.WriteLine(DiceNo);
                }
                DiceNoLabel.Content = "" + DiceNo;
                /*
                 * Bug No. 1
                 */
                while (true)
                {
                    if (Convert.ToInt32(DiceNoLabel.Content) != (int)DiceNo)
                    {
                        DiceNoLabel.Content = "" + DiceNo;
                    }
                    else
                    {
                        break;
                    }
                }
                if (!NoMoreMove(DiceNo))
                {
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
                    ThreadStart start = new ThreadStart(TimerStart);
                    TimerThread = new Thread(start);
                    TimerThread.Name = "Timer Thread";
                    TimerThread.Start();
                    isMoved = false;
                    TimedOut = false;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("No More Move for this number");
                    Turn();
                }
            }
            
            
        }
        double TimeOutBarGridWidth;
        double TimeOutBarGridHeight;
        bool TimedOut = false;
        
        void TempRadio_Checked(object sender, RoutedEventArgs e)
        {
            uint KayiNo = 0;
            uint ToMove = 0;
            if (!TimedOut)
            {
                if (sender.Equals(TempRadio1))
                {
                    if ((bool)TempRadio1.IsChecked)
                        KayiNo = 0;

                }
                else if (sender.Equals(TempRadio2))
                {
                    if ((bool)TempRadio2.IsChecked)
                        KayiNo = 1;

                }
                else if (sender.Equals(TempRadio3))
                {
                    if ((bool)TempRadio3.IsChecked)
                        KayiNo = 2;

                }
                else if (sender.Equals(TempRadio4))
                {
                    if ((bool)TempRadio4.IsChecked)
                        KayiNo = 3;

                }
                ToMove = DiceNo;
                MyKayi[TurnState,KayiNo] += DiceNo;
                ToMove = MyKayi[TurnState,KayiNo];
                

                if (ToMove < 25)
                {
                    KayiGrid.Children.Remove(TempStackpanel);
                    KayiGrid.Background = Brushes.White;



                    SetKayiPosition(MoveKayi[TurnState, KayiNo], MoveRect[TurnState, ToMove], KayiNo);
                    /*
                     * Bug No. 5
                     */
                    if(!OutOfMyWay(MoveRect[TurnState, ToMove]))
                        Turn();
                    ReachedHome(MoveKayi[TurnState, KayiNo], KayiNo);
                    
                }
                else
                {
                    MyKayi[TurnState,KayiNo] -= DiceNo;
                    System.Windows.Forms.MessageBox.Show("Unable to move");
                }
                
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Timed Out");
                Turn();
            }
            
            /*
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
         */   
        }
        

    }
}
