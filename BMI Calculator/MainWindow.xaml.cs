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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isMale)
            {
                // put function calculating bmi for male
            }
            else if (isFemale)
            {
                // function calculating bmi for male
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
    }
}
