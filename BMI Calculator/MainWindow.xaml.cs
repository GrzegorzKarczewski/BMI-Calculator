using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BMI_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
     
        bool isMale = false;
        bool isFemale = false;
        float weight;
        float height;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            weight = float.Parse(tb_weight.Text);
            height = float.Parse(tb_height.Text);
            double bmi =  Math.Round(calculateBmi(weight, height), 1);

            lbl_result.FontSize = 34;
            lbl_result.Content = bmi;
            if (bmi > 18.5 && bmi < 24.9)
            {
                lbl_result.Foreground = Brushes.Green;
                
            }
            if (bmi < 18.5)
            {
                lbl_result.Foreground = Brushes.LightBlue;
            }
            if (bmi >= 25)
            {
                lbl_result.Foreground = Brushes.OrangeRed;
            }



            if (isMale)
            {
                // Put some male figure picture
             

            }
            else if (isFemale)
            {
                // Put some female figure picture

            }
        }

        private void cb_male_Checked(object sender, RoutedEventArgs e)
        {
            // This is checking if male is checked and unsets female to be safe
            if (cb_male.IsChecked == true)
            {
                isMale = true;
                cb_female.IsChecked = false ;
            }
        }

        private void cb_female_Checked(object sender, RoutedEventArgs e)
        {
            // This is checking if female is checked and unsets male to be safe
            if (cb_female.IsChecked == true)
            {
                isFemale= true;
                cb_male.IsChecked = false ;
            }
        }

        double calculateBmi(double fweight, double fheight)
        {
            // local variable for returning value
            double bmi = 0;

            bmi = (fweight / fheight) / fheight * 10000;  // bmi formula 


            return bmi;
        }
    }
}
