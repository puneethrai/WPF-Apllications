using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
namespace ChokaBhara_Win8style
{
    public partial class MainWindow
    {
        bool isMoved = false;
        uint[,] MyKayi = new uint[4,4];
        uint[] ScoreCard = new uint[4];
        Rectangle[,] KayiPlaced = new Rectangle[4, 4];
        
        /// <summary> 
        /// Sets <see cref="T:Kayi"/> Position to <see cref="T:RectNo" /> with <see cref="T:KayiNo"/>
        /// </summary> 
        /// <param name="Kayi">Kayi of <see cref="T:System.Windows.Shapes.Ellipse"/> to be moved.</param> 
        /// <param name="RectNo">Rectangle of <see cref="T:System.Windows.Shapes.Rectangle"/> to be placed.</param>
        /// <param name="KayiNo">Which Kayi of <see cref="T:System.Windows.Shapes.Ellipse"/> to be moved</param>
        public void SetKayiPosition(Ellipse Kayi, Rectangle RectNo, uint KayiNo)
        {
            switch(KayiNo)
            {
                case 0:
                    Canvas.SetLeft(Kayi, Canvas.GetLeft(RectNo));
                    Canvas.SetTop(Kayi, Canvas.GetTop(RectNo));
                    break;
                case 1:
                    Canvas.SetLeft(Kayi, Canvas.GetLeft(RectNo)+40);
                    Canvas.SetTop(Kayi, Canvas.GetTop(RectNo));
                    break;
                case 2:
                    Canvas.SetLeft(Kayi, Canvas.GetLeft(RectNo));
                    Canvas.SetTop(Kayi, Canvas.GetTop(RectNo)+38);
                    break;
                case 3:
                    Canvas.SetLeft(Kayi, Canvas.GetLeft(RectNo)+40);
                    Canvas.SetTop(Kayi, Canvas.GetTop(RectNo)+38);
                    break;
                    
            }
            KayiPlaced[TurnState, KayiNo] = RectNo;
            isMoved = true;
        }
        public bool NoMoreMove(uint MoveNo)
        {
            bool CantMove = true;
            for (int i = 0; i < 4; i++)
                if ((MyKayi[TurnState, i] + MoveNo) < 25)
                    CantMove = false;
            return CantMove;
        }
        public void OutOfMyWay(Rectangle Placing)
        {
            for (uint i = 0; i < 4; i++)
            {
                if (i != TurnState)
                {
                    for (uint j = 0; j < 4; j++)
                    {
                        if (KayiPlaced[i,j] == Placing)
                        {
                            if (KayiPlaced[i, j] != R13 || KayiPlaced[i, j] != R31 || KayiPlaced[i, j] != R33 || KayiPlaced[i, j] != R35 || KayiPlaced[i, j] != R53)
                            {
                                SetKayiPosition(MoveKayi[i, j], MoveRect[i, 0], j);
                                MyKayi[i, j] = 0;
                                
                            }
                        }
                    }
                }
            }
        }
        public void ReachedHome(Ellipse CheckKayi,uint KayiNo)
        {
            if (MyKayi[TurnState, KayiNo] == 24)
            {
                CheckKayi.Visibility = System.Windows.Visibility.Hidden;
                ScoreCard[TurnState]++;
                RedPoint.Content = ""+ScoreCard[0];
                GreenPoint.Content = "" + ScoreCard[1];
                BluePoint.Content = "" + ScoreCard[2];
                YellowPoint.Content = "" + ScoreCard[3];
            }
            for (int i = 0; i < 4; i++)
            {
                if (ScoreCard[i] == 4)
                {
                    switch (i)
                    {
                        case 0: MessageBox.Show("Red Wins");
                            break;
                        case 1: MessageBox.Show("Green Wins");
                            break;
                        case 2: MessageBox.Show("Blue Wins");
                            break;
                        case 3: MessageBox.Show("Yellow Wins");
                            break;

                    }
                    break;
                }
            }
        }
    }
}
