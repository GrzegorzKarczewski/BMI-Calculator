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
        bool isWeightOK = false;
        bool isWeightLow = false;
        bool isWeightHigh = false;
        float weight;
        float height;

        public MainWindow()
        {

            InitializeComponent();

            // Restricting input fields for reasonable values (lazy way)
            tb_age.MaxLength = 2;
            tb_height.MaxLength = 3;
            tb_weight.MaxLength = 4;
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
                isWeightLow = true;
                
                
            }
            if (bmi < 18.5)
            {
                lbl_result.Foreground = Brushes.LightBlue;
                isWeightOK = true;
            }
            if (bmi >= 25)
            {
                lbl_result.Foreground = Brushes.OrangeRed;
                isWeightHigh = true;
            }

            // TODO: insert function here to change the type of image depending on weight and gender

            setPersonImage();
            if (isMale)
            {
                
             

            }
            else if (isFemale)
            {
                // Put some female figure picture

            }
        }

        private void setPersonImage()
        {
           // populate this
        }

        private void cb_male_Checked(object sender, RoutedEventArgs e)
        {
            // This is checking if male is checked and unsets female to be safe
            if (cb_male.IsChecked == true)
            {
                isMale = true;
                cb_female.IsChecked = false;

                Image maleImage = new Image();
                maleImage.Height = genderImage.Height -5;
                maleImage.Width = genderImage.Width -5 ;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();

                bitmap.UriSource = new Uri(@"D:\coding\BMI Calculator\BMI Calculator\bin\Debug\net6.0-windows\male_fat.png");
                //bitmap.UriSource = new Uri("alpha-mask-3070291_640.png", UriKind.Relative)

                bitmap.EndInit();    
                genderImage.Source = bitmap;                


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
