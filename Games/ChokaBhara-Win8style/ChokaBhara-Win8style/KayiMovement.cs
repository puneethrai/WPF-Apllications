using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
namespace ChokaBhara_Win8style
{
    public partial class MainWindow
    {
        public void SetKayiPosition(Ellipse Kayi, Rectangle RectNo, int KayiNo)
        {
            switch(KayiNo)
            {
                case 1:
                    Canvas.SetLeft(Kayi, Canvas.GetLeft(RectNo));
                    Canvas.SetTop(Kayi, Canvas.GetTop(RectNo));
                    break;
                case 2:
                    Canvas.SetLeft(Kayi, Canvas.GetLeft(RectNo)+40);
                    Canvas.SetTop(Kayi, Canvas.GetTop(RectNo));
                    break;
                case 3:
                    Canvas.SetLeft(Kayi, Canvas.GetLeft(RectNo));
                    Canvas.SetTop(Kayi, Canvas.GetTop(RectNo)+38);
                    break;
                case 4:
                    Canvas.SetLeft(Kayi, Canvas.GetLeft(RectNo)+40);
                    Canvas.SetTop(Kayi, Canvas.GetTop(RectNo)+38);
                    break;
            }
        }
    }
}
