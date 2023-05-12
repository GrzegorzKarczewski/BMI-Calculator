﻿using System;
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
using System.Windows.Documents.DocumentStructures;
using System.Reflection.PortableExecutable;
using System.Globalization;

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
        string gender = string.Empty;
        double weight;
        double height;
        int age;
        string mostRecentUser = "mostRecentUser";
        static int currentWeightType = 0;


       public enum WeightType
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

            LoadUsersOnStart();
            LoadDataFromMostRecentUser();
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
            weight = double.Parse(tb_weight.Text, CultureInfo.InvariantCulture);
            height = double.Parse(tb_height.Text);
            age = int.Parse(tb_age.Text);
            if (cb_male.IsChecked == true)
                gender = "Male";
            if (cb_female.IsChecked == true)
                gender = "Female";



            double bmi = calculateBmi(weight, height);

            lbl_result.FontSize = 34;
            lbl_result.Content = bmi;


            if (bmi > 18.5 && bmi < 24.9)
            {

                ChangeLabelBMIScore(WeightType.weightNormal);
                GiveTipsForBMI(WeightType.weightNormal);
                currentWeightType = 1;

                if (isMale)
                {
                    SetPersonImage(WeightType.weightNormal, 0);
                }
                else { SetPersonImage(WeightType.weightNormal, 1); }

            }
            if (bmi < 18.5)
            {
                ChangeLabelBMIScore(WeightType.weightLow);
                GiveTipsForBMI(WeightType.weightLow);
                currentWeightType = 0;

                if (isMale)
                {
                    SetPersonImage(WeightType.weightLow, 0);
                }
                else { SetPersonImage(WeightType.weightLow, 1); }
            }
            if (bmi >= 25)
            {

                ChangeLabelBMIScore(WeightType.weightHigh);
                GiveTipsForBMI(WeightType.weightHigh);
                currentWeightType = 2;

                if (isMale)
                {
                    SetPersonImage(WeightType.weightHigh, 0);
                }
                else { SetPersonImage(WeightType.weightHigh, 1); }
            }

            LoadOrSaveUsersDatabase(name, gender, age, weight, height, bmi);

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
                gender = "Male";

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
                gender = "Female";
            }

        }

        static double calculateBmi(double dweight, double dheight)
        {

            double bmi = 0;

            bmi = (dweight / dheight) / dheight * 10000;  // bmi formula 


            return Math.Round(bmi,1);
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


        void LoadOrSaveUsersDatabase(string name, string gender, int age, double weight, double height, double bmi)
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
                Name = name,
                Gender = gender,
                Age = age,
                Weight = weight,
                Height = height,
                BMI = bmi,
                Timestamp = DateTime.Now

            };
            userRepository.AddUser(newUser);

            GSettings.AddUpdateAppSettings(mostRecentUser, name);
            
            lb_users.Items.Clear();
            List<string> lastusers = userRepository.GetUsers();
            foreach (string users in lastusers)
            {
                lb_users.Items.Add(users);
            }

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: If clicked on the list, load name, height, age and gender of user
            //       leave weight for user to calculate again

            string name = lb_users.SelectedItem.ToString();
            if (name != null)
                LoadUsersOnSelectionChanged(name);

        }
        public void LoadUsersOnStart()
        {
            string databaseFile = "UserData.db";
            string connectionString = $"Data Source={databaseFile};Version=3;";

            DatabaseInitializer initializer = new DatabaseInitializer(connectionString);
            initializer.Initialize();

            UserRepository userRepository = new UserRepository(connectionString);

            lb_users.Items.Clear();
            List<string> lastusers = userRepository.GetUsers();
            foreach (string users in lastusers)
            {
                lb_users.Items.Add(users);
            }

        }

        public void LoadDataFromMostRecentUser()
        {
            string name = GSettings.ReadSetting(mostRecentUser);

            string databaseFile = "UserData.db";
            string connectionString = $"Data Source={databaseFile};Version=3;";

            DatabaseInitializer initializer = new DatabaseInitializer(connectionString);
            initializer.Initialize();

            UserRepository userRepository = new UserRepository(connectionString);
            UserData user = userRepository.GetUserByName(name);

            if (user != null)
            {
                tb_name.Text = user.Name;
                if (user.Gender == "Male")
                    cb_male.IsChecked = true;
                if (user.Gender == "Female")
                    cb_female.IsChecked = true;
                tb_age.Text = user.Age.ToString();
                tb_weight.Text = user.Weight.ToString();
                tb_height.Text = user.Height.ToString();
                lbl_result.Content = Math.Round(user.BMI, 1);

            }
        }

        public void LoadUsersOnSelectionChanged(string name)
        {


            string databaseFile = "UserData.db";
            string connectionString = $"Data Source={databaseFile};Version=3;";

            DatabaseInitializer initializer = new DatabaseInitializer(connectionString);
            initializer.Initialize();

            UserRepository userRepository = new UserRepository(connectionString);
            UserData user = userRepository.GetUserByName(name);

            if (user != null)
            {
                tb_name.Text = user.Name;
                if (user.Gender == "Male")
                    cb_male.IsChecked = true;
                if (user.Gender == "Female")
                    cb_female.IsChecked = true;
                tb_age.Text = user.Age.ToString();
                tb_weight.Text = user.Weight.ToString();
                tb_height.Text = user.Height.ToString();
                lbl_result.Content = Math.Round(user.BMI,1);

                {
                    if (user.BMI > 18.5 && user.BMI < 24.9)
                    {

                        ChangeLabelBMIScore(WeightType.weightNormal);
                        GiveTipsForBMI(WeightType.weightNormal);

                        if (isMale)
                        {
                            SetPersonImage(WeightType.weightNormal, 0);
                        }
                        else { SetPersonImage(WeightType.weightNormal, 1); }

                    }
                    if (user.BMI < 18.5)
                    {
                        ChangeLabelBMIScore(WeightType.weightLow);
                        GiveTipsForBMI(WeightType.weightLow);

                        if (isMale)
                        {
                            SetPersonImage(WeightType.weightLow, 0);
                        }
                        else { SetPersonImage(WeightType.weightLow, 1); }
                    }
                    if (user.BMI >= 25)
                    {

                        ChangeLabelBMIScore(WeightType.weightHigh);
                        GiveTipsForBMI(WeightType.weightHigh);

                        if (isMale)
                        {
                            SetPersonImage(WeightType.weightHigh, 0);
                        }
                        else { SetPersonImage(WeightType.weightHigh, 1); }
                    }
                }
            }

        }

        public void ChangeLabelBMIScore(WeightType weightType)
        {
           
            lbl_result.FontSize = 34;
            lbl_result.FontWeight = FontWeights.Bold;
            switch (weightType)
            {
                case WeightType.weightLow:
                    lbl_result.Foreground = Brushes.LightBlue;
                    return;
                case WeightType.weightNormal:
                    lbl_result.Foreground = Brushes.Green;
                    return;
                case WeightType.weightHigh:
                    lbl_result.Foreground = Brushes.OrangeRed;
                    return;

            }
        }
    }
}
