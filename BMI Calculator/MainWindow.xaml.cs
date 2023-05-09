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
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Data.SQLite;

namespace BMI_Calculator
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool isMale = false;
        bool isFemale = false;
        string name = string.Empty;
        float weight;
        float height;
        int age;


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

            /// <summary>
            /// Calculates the BMI using the user's input for weight and height, then updates the UI with the result,
            /// provides a tip based on the BMI, and changes the displayed image accordingly.
            /// This function is triggered when the user clicks the "Calculate BMI" button.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An instance of RoutedEventArgs containing event data.</param>

            name = tb_name.Text;
            weight = float.Parse(tb_weight.Text);
            height = float.Parse(tb_height.Text);
            age = int.Parse(tb_age.Text);

            
            double bmi = Math.Round(calculateBmi(weight, height), 1);

            lbl_result.FontSize = 34;
            lbl_result.Content = bmi;


            if (bmi > 18.5 && bmi < 24.9)
            {
                lbl_result.Foreground = Brushes.Green;
               

                GiveTipsForBMI(WeightType.weightNormal);

                if (isMale)
                {
                    SetPersonImage(WeightType.weightNormal, 0);
                }
                else { SetPersonImage(WeightType.weightNormal, 1); }

            }
            if (bmi < 18.5)
            {
                lbl_result.Foreground = Brushes.LightBlue;
             

                GiveTipsForBMI(WeightType.weightLow);

                if (isMale)
                {
                    SetPersonImage(WeightType.weightLow, 0);
                }
                else { SetPersonImage(WeightType.weightLow, 1); }
            }
            if (bmi >= 25)
            {
                lbl_result.Foreground = Brushes.OrangeRed;

                GiveTipsForBMI(WeightType.weightHigh);

                if (isMale)
                {
                    SetPersonImage(WeightType.weightHigh, 0);
                }
                else { SetPersonImage(WeightType.weightHigh, 1); }
            }

            LoadOrSaveUsersDatabase(name, age, weight, height, bmi);

            // TODO: Creating ui input for name or uniqe login, 

           

        }

     

        private void SetPersonImage(WeightType weightType, int gender)
        {

            /// <summary>
            /// Updates the displayed image based on the user's weight type and gender.
            /// The function takes the WeightType enum value and an integer representing the gender (0 for male, 1 for female),
            /// and changes the image source accordingly using the images stored in the "Images" folder.
            /// </summary>
            /// <param name="weightType">A WeightType enum value representing the user's weight type.</param>
            /// <param name="gender">An integer representing the user's gender (0 for male, 1 for female).</param>


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
                        Image femaleImage = new();
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
                        Image femaleImage = new();
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
                        Image maleImage = new();
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
                        Image femaleImage = new();
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

            }
        }

        private void cb_female_Checked(object sender, RoutedEventArgs e)
        {
            // This is checking if female is checked and unsets male to be safe
            if (cb_female.IsChecked == true)
            {
                isFemale = true;
                isMale = false;
                cb_male.IsChecked = false;         
            }
        }

        static double calculateBmi(double dweight, double dheight)
        {

            double bmi = 0;

            bmi = (dweight / dheight) / dheight * 10000;  // bmi formula 


            return bmi;
        }



        void GiveTipsForBMI(WeightType weightType)
        {
            /// <summary>
            /// Provides a random tip based on the user's weight type (underweight, healthy, or overweight).
            /// The tips are read from a JSON file named "tips.json", which should contain three categories
            /// of tips: underweight_tips, healthy_weight_tips, and overweight_tips.
            /// If an error occurs while reading the file or the file is not found, a default tip is displayed.
            /// </summary>
            /// <param name="weightType">A WeightType enum value representing the user's weight type.</param>


            string json = String.Empty;
            try
            {
                json = File.ReadAllText("tips.json");
            }
            catch (IOException e)
            {
                //MessageBox.Show(e.ToString());
            }
            finally
            {
                if (json == null)
                {
                    json = "Try to eat Healthy!";
                }
            }

            BMITips tips = JsonConvert.DeserializeObject<BMITips>(json);
            string bmiCategory = String.Empty;

            switch (weightType)
            {
                case WeightType.weightLow:
                    bmiCategory = "underweight_tips";
                    break;
                case WeightType.weightHigh:
                    bmiCategory = "overweight_tips";
                    break;
                case WeightType.weightNormal:
                    bmiCategory = "healthy_weight_tips";
                    break;

            }
            List<string> selectedTipsList = (List<string>)typeof(BMITips).GetProperty(bmiCategory).GetValue(tips);

            Random random = new Random();
            int randomIndex = random.Next(0, selectedTipsList.Count);
            string randomTip = selectedTipsList[randomIndex];
            lbl_tipscontent.Content = randomTip;
        }


        void LoadOrSaveUsersDatabase(string name, int age, double weight, double height, double bmi)
        {
            string databaseFile = "UserData.db";
            string connectionString = $"Data Source={databaseFile};Version=3;";

            DatabaseInitializer initializer = new DatabaseInitializer(connectionString);
            initializer.Initialize();

            // Testing adding new user
            // Add a new user to the Users table
            UserRepository userRepository = new UserRepository(connectionString);
            UserData newUser = new UserData
            {
                Name = "John Doe",
                Age = age,
                Weight = weight,
                Height = height,
                BMI = bmi
            };
            userRepository.AddUser(newUser);



            // Get user data by name
            string userName = "John Doe";
            UserData user = userRepository.GetUserByName(userName);

            if (user != null)
            {
                MessageBox.Show($"User: {user.Name} " +
                $"Age: {user.Age} " +
                $"Weight: {user.Weight} " +
                $"Height: {user.Height}  " +
               $"BMI: {user.BMI}");
            }
            else
            {
                MessageBox.Show($"User '{userName}' not found.");
            }
        }
    }
}
