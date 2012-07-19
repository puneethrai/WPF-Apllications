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
using DLL;
namespace UsesDLL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region private
        private enum CalcOption {ADD,SUB,MUL,DIV,INVALID};
        private string message { get; set; }
        private Calculator Calc;
        private Brush XTBBorder;
        private Brush YTBBorder;
        private Brush CalcOptionBorderBrush;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            Calc = new Calculator();
            XTBBorder = XTB.BorderBrush;
            YTBBorder = YTB.BorderBrush;
            CalcOptionBorderBrush = CalcOptionBorder.BorderBrush;
        }

        private void CalcButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                int InputParam = CheckInputParam();
                switch (InputParam)
                {

                    case (int)CalcOption.ADD:
                        ResultTB.Text = "" +
                            Calc.Sum
                            (Convert.ToInt64(XTB.Text), Convert.ToInt64(XTB.Text));
                        break;
                    case (int)CalcOption.SUB:
                        ResultTB.Text = "" +
                            Calc.Difference
                            (Convert.ToInt64(XTB.Text), Convert.ToInt64(XTB.Text));
                        break;
                    case (int)CalcOption.MUL:
                        ResultTB.Text = "" +
                            Calc.Multiply
                            (Convert.ToInt64(XTB.Text), Convert.ToInt64(XTB.Text));
                        break;
                    case (int)CalcOption.DIV:
                        ResultTB.Text = "" +
                            Calc.Divide
                            (Convert.ToInt64(XTB.Text), Convert.ToInt64(XTB.Text));
                        break;
                    case (int)CalcOption.INVALID:
                        ResultTB.Text = message;
                        break;
                }
                if (InputParam != (int)CalcOption.INVALID)
                {
                    Restore();
                }
            }
            catch (DivideByZeroException error)
                {
                    ResultTB.Text = error.Message;
                }
            catch(Exception error)
            {
                ResultTB.Text = "Unknow Exception occured:"+error.Message;
            }

            
        }
        private void Restore()
        {
            XTB.BorderBrush = XTBBorder;
            YTB.BorderBrush = YTBBorder;
            CalcOptionBorder.BorderBrush = CalcOptionBorderBrush;
        }
        private int CheckInputParam()
        {
            int ReturnVal = -1;
            if ((bool)AddRB.IsChecked)
                ReturnVal = (int)CalcOption.ADD;
            else if ((bool) SubRB.IsChecked)
                ReturnVal = (int)CalcOption.SUB;
            else if ((bool)MulRB.IsChecked)
                ReturnVal = (int)CalcOption.MUL;
            else if ((bool)DivRB.IsChecked)
                ReturnVal = (int)CalcOption.DIV;
            if (ReturnVal >= (int)CalcOption.ADD)
            {
                CalcOptionBorder.BorderBrush = CalcOptionBorderBrush;
                if (XTB.Text == "" || YTB.Text == "")
                {
                    ReturnVal = (int)CalcOption.INVALID;
                    var temp =
                        (XTB.Text == "") ?
                        XTB.BorderBrush = Brushes.Red
                        : YTB.BorderBrush = Brushes.Red;
                    message = (XTB.Text == "") ?
                        "Please Provide X Value" 
                        : "Please Provide Y Value";
                    
                }
            }
            else
            {
                CalcOptionBorder.BorderBrush = Brushes.Red;
                ReturnVal = (int)CalcOption.INVALID;
                message = "Please select one of the above calculation option";
            }
            return ReturnVal;
        }
    }
}
