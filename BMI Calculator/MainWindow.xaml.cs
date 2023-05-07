using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
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


        enum WeightType
        {
            weightLow,
            weightNormal,
            weightHigh
        }

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
                isWeightOK = true;
                if (isMale)
                {
                    SetPersonImage(WeightType.weightNormal, 0);
                }
                else { SetPersonImage(WeightType.weightNormal, 1); }

            }
            if (bmi < 18.5)
            {
                lbl_result.Foreground = Brushes.LightBlue;
                isWeightLow = true;
                if (isMale)
                {
                    SetPersonImage(WeightType.weightLow, 0);
                }
                else { SetPersonImage(WeightType.weightLow, 1); }
            }
            if (bmi >= 25)
            {
                lbl_result.Foreground = Brushes.OrangeRed;
                isWeightHigh = true;
                if (isMale)
                {
                    SetPersonImage(WeightType.weightHigh, 0);
                }
                else { SetPersonImage(WeightType.weightHigh, 1); }
            }
          
        }

        private void SetPersonImage(WeightType weightType, int gender)
        {
           // weightype shold be always of enum WeightType
           // gender is 0 for man, 1 for woman
           switch (weightType)
            {
                case WeightType.weightLow:
                    if (gender == 0)
                    {
                        // put image of too skinny man
                        Image maleImage = new();
                        maleImage.Height = genderImage.Height - 5;
                        maleImage.Width = genderImage.Width - 5;

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();

                        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "male_skinny.png");
                        bitmap.UriSource = new Uri(imagePath);

                        bitmap.EndInit();
                        genderImage.Source = bitmap;
                        return;
                    }
                    else
                    {
                        // put image of skinny lady
                        Image femaleImage = new ();
                        femaleImage.Height = genderImage.Height - 5;
                        femaleImage.Width = genderImage.Width - 5;

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();

                        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "female_skinny.png");
                        bitmap.UriSource = new Uri(imagePath);

                        bitmap.EndInit();
                        genderImage.Source = bitmap;


                    }
                    
                    return;
                case WeightType.weightNormal:
                    if (gender == 0)
                    {
                        // put image of too skinny man
                        Image maleImage = new();
                        maleImage.Height = genderImage.Height - 5;
                        maleImage.Width = genderImage.Width - 5;

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();

                        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "male_regular.png");
                        bitmap.UriSource = new Uri(imagePath);

                        bitmap.EndInit();
                        genderImage.Source = bitmap;
                        return;
                    }
                    else
                    {
                        // put image of skinny lady
                        Image femaleImage = new ();
                        femaleImage.Height = genderImage.Height - 5;
                        femaleImage.Width = genderImage.Width - 5;

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();

                        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "female_regular.png");
                        bitmap.UriSource = new Uri(imagePath);

                        bitmap.EndInit();
                        genderImage.Source = bitmap;


                    }
                    return;
                case WeightType.weightHigh:
                    if (gender == 0)
                    {
                        // put image of too skinny man
                        Image maleImage = new ();
                        maleImage.Height = genderImage.Height - 5;
                        maleImage.Width = genderImage.Width - 5;

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();

                        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "male_fat.png");
                        bitmap.UriSource = new Uri(imagePath);

                        bitmap.EndInit();
                        genderImage.Source = bitmap;
                        return;
                    }
                    else
                    {
                        // put image of skinny lady
                        Image femaleImage = new ();
                        femaleImage.Height = genderImage.Height - 5;
                        femaleImage.Width = genderImage.Width - 5;

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();

                        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "female_fat.png");
                        bitmap.UriSource = new Uri(imagePath);

                        bitmap.EndInit();
                        genderImage.Source = bitmap;


                    }
                    return;
                   
            }

        }

        private void cb_male_Checked(object sender, RoutedEventArgs e)
        {
            // This is checking if male is checked and unsets female to be safe
            if (cb_male.IsChecked == true)
            {
                isMale = true;
                
                cb_female.IsChecked = false;
                isFemale = false;
                /*
                Image maleImage = new Image();
                maleImage.Height = genderImage.Height -5;
                maleImage.Width = genderImage.Width -5 ;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();

                bitmap.UriSource = new Uri(@"D:\coding\BMI Calculator\BMI Calculator\bin\Debug\net6.0-windows\male_fat.png");
                
                // TODO: for the final version of the program, images needs to be stored in the same catalogue as .exe file
                //bitmap.UriSource = new Uri("alpha-mask-3070291_640.png", UriKind.Relative)

                bitmap.EndInit();    
                genderImage.Source = bitmap;                
                */

            } 
        }

        private void cb_female_Checked(object sender, RoutedEventArgs e)
        {
            // This is checking if female is checked and unsets male to be safe
            if (cb_female.IsChecked == true)
            {
                isFemale= true;
                isMale = false;
                cb_male.IsChecked = false ;
/*
                Image maleImage = new Image();
                maleImage.Height = genderImage.Height - 5;
                maleImage.Width = genderImage.Width - 5;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();

                bitmap.UriSource = new Uri(@"D:\coding\BMI Calculator\BMI Calculator\bin\Debug\net6.0-windows\female_regular.png");

                // TODO: for the final version of the program, images needs to be stored in the same catalogue as .exe file

                bitmap.EndInit();
                genderImage.Source = bitmap;
                */
            }
        }

       static double calculateBmi(double dweight, double dheight)
        {
            // local variable for returning value
            double bmi = 0;

            bmi = (dweight / dheight) / dheight * 10000;  // bmi formula 


            return bmi;
        }
    }
}
