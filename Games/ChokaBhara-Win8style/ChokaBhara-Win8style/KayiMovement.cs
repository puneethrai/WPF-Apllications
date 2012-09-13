using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
namespace ChokaBharaWin8Style
{
    /*
     * TODO:if 4 & 8 dice 1 more chance,If they send other player home 1 more chance,dice options 1,2,3,4,8,1kaye kill 1 kayi only,In home make pawns in row
     * */
    public partial class MainWindow
    {
        bool isMoved;
        uint[,] MyKayi = null;
        uint[] ScoreCard = null;
        Rectangle[,] KayiPlaced = null;
        bool[] WinnerDisplayed = null;
                
        /// <summary> 
        /// Sets <see cref="T:Kayi"/> Position to <see cref="T:RectNo" /> with <see cref="T:KayiNo"/>
        /// </summary> 
        /// <param name="kayi">Kayi of <see cref="T:System.Windows.Shapes.Ellipse"/> to be moved.</param> 
        /// <param name="RectNo">Rectangle of <see cref="T:System.Windows.Shapes.Rectangle"/> to be placed.</param>
        /// <param name="KayiNo">Which Kayi of <see cref="T:System.Windows.Shapes.Ellipse"/> to be moved</param>
        public void SetKayiPosition(Ellipse kayi, Rectangle RectNo, uint KayiNo)
        {
            switch(KayiNo)
            {
                case 0:
                    Canvas.SetLeft(kayi, Canvas.GetLeft(RectNo));
                    Canvas.SetTop(kayi, Canvas.GetTop(RectNo));
                    break;
                case 1:
                    Canvas.SetLeft(kayi, Canvas.GetLeft(RectNo)+40);
                    Canvas.SetTop(kayi, Canvas.GetTop(RectNo));
                    break;
                case 2:
                    Canvas.SetLeft(kayi, Canvas.GetLeft(RectNo));
                    Canvas.SetTop(kayi, Canvas.GetTop(RectNo)+38);
                    break;
                case 3:
                    Canvas.SetLeft(kayi, Canvas.GetLeft(RectNo)+40);
                    Canvas.SetTop(kayi, Canvas.GetTop(RectNo)+38);
                    break;
                    
            }
            KayiPlaced[TurnState, KayiNo] = RectNo;
            isMoved = true;
        }
        [CLSCompliant(false)]
        public bool NoMoreMove(uint MoveNo)
        {
            bool CantMove = true;
            for (uint i = MinKayi; i < MaxKayi; i++)
                if ((MyKayi[TurnState, i] + MoveNo) < 25)
                    CantMove = false;
            return CantMove;
        }
        
         
        public bool OutOfMyWay(Rectangle Placing)
        {
            bool Flags = false;
            for (uint i = MinPlayer; i < MaxPlayer; i++)
            {
                if (i != TurnState)
                {
                    for (uint j = MinKayi; j < MaxKayi; j++)
                    {
                        if (KayiPlaced[i,j] == Placing)
                        {
                            /*
                             * Bug No.3
                             */
                            if (KayiPlaced[i, j] != R13 && KayiPlaced[i, j] != R31 && KayiPlaced[i, j] != R33 && KayiPlaced[i, j] != R35 && KayiPlaced[i, j] != R53)
                            {
                                SetKayiPosition(MoveKayi[i, j], MoveRect[i, 0], j);
                                MyKayi[i, j] = 0;
                                Flags = true;
                                break;
                            }
                        }
                    }
                }
            }
            return Flags;
        }
        public void ReachedHome(Ellipse CheckKayi,uint KayiNo)
        {
            Storyboard s = null;
            if (MyKayi[TurnState, KayiNo] == (MaxMoves - 1))
            {
                CheckKayi.Visibility = System.Windows.Visibility.Hidden;
                ScoreCard[TurnState]++;
                RedPoint.Content = "" + ScoreCard[0];
                GreenPoint.Content = "" + ScoreCard[1];
                BluePoint.Content = "" + ScoreCard[2];
                YellowPoint.Content = "" + ScoreCard[3];

                for (uint i = MinPlayer; i < MaxPlayer; i++)
                {
                    if (ScoreCard[i] == MaxKayi)
                    {
                        if (WinnerList.Visibility != Visibility.Visible)
                            WinnerList.Visibility = Visibility.Visible;
                        switch (i)
                        {
                            case 0:
                                /*
                                 * Bug No. 8
                                 */
                                if (!WinnerDisplayed[0])
                                {
                                    WinnerList.Items.Add("Red Wins");
                                    // Locate Storyboard resource
                                    s = (Storyboard)TryFindResource("PlayerBoxAnimate1");
                                    s.Begin();	// Start animation
                                    WinnerDisplayed[0] = true;
                                }
                                break;
                            case 1: 
                                if (!WinnerDisplayed[1])
                                {
                                    WinnerList.Items.Add("Green Wins");
                                    // Locate Storyboard resource
                                    s = (Storyboard)TryFindResource("PlayerBoxAnimate2");
                                    s.Begin();	// Start animation
                                    WinnerDisplayed[1] = true;
                                }
                                break;
                            case 2: 
                                if (!WinnerDisplayed[2])
                                {
                                    WinnerList.Items.Add("Blue Wins");
                                    // Locate Storyboard resource
                                    s = (Storyboard)TryFindResource("PlayerBoxAnimate3");
                                    s.Begin();	// Start animation
                                    WinnerDisplayed[2] = true;
                                }
                                break;
                            case 3: 
                                if (!WinnerDisplayed[3])
                                {
                                    WinnerList.Items.Add("Yellow Wins");
                                    // Locate Storyboard resource
                                    s = (Storyboard)TryFindResource("PlayerBoxAnimate4");
                                    s.Begin();	// Start animation
                                    WinnerDisplayed[3] = true;
                                }
                                break;

                        }

                    }
                    
                    
                }
            }
            
        }
    }
}
