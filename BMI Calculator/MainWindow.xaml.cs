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
        static string currentName = string.Empty;

        // Database global constants
        static string databaseFile = "UserData.db";
        static string connectionString = $"Data Source={databaseFile};Version=3;";


        public enum WeightType
        {
            WeightLow,
            WeightNormal,
            WeightHigh
        }

        public MainWindow()
        {

            InitializeComponent();

            // Restricting input fields for reasonable values (lazy way)
            tb_name.MaxLength = 20; // I dont think there is a reason to make it longer
            tb_age.MaxLength = 3;
            tb_height.MaxLength = 3;
            tb_weight.MaxLength = 4;

            LoadUsersOnStart();
            LoadDataFromMostRecentUser();
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {

            /// <summary>
            /// The `Button_Click` event is triggered when the "Calculate BMI" button is clicked by the user.
            /// This function first extracts the user's inputs, including name, weight, height, age, and gender from the UI.
            /// It then calculates the BMI using the user's input for weight and height.
            ///
            /// Depending on the BMI range, it performs the following operations:
            /// - Updates the UI with the calculated BMI, changing the font size to 34.
            /// - Adjusts the UI styling and provides user tips specific to the BMI category (low weight, normal weight, or high weight).
            /// - Changes the displayed image based on the BMI category and the user's gender.
            /// 
            /// Finally, it saves the user's data, including the calculated BMI, into the database.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">An instance of RoutedEventArgs containing event data.</param>


            name = tb_name.Text;
          
            weight = double.Parse(tb_weight.Text, CultureInfo.InvariantCulture);
            height = double.Parse(tb_height.Text);
            age = int.Parse(tb_age.Text);
            bool isNameString = string.IsNullOrEmpty(name);
            bool isWeightDouble = double.TryParse(weight.ToString(), out double result_weight);
            bool isHeightDouble = double.TryParse(height.ToString(), out double result_height);
            bool isAgeInt = int.TryParse(age.ToString(), out int result_age);


            if (cb_male.IsChecked == true)
                gender = "Male";
            if (cb_female.IsChecked == true)
                gender = "Female";

            // Execute the calculations only if input data are proper
            if (isNameString == false && isAgeInt && isHeightDouble && isWeightDouble)
            {

                double bmi = calculateBmi(weight, height);

                lbl_result.FontSize = 34;
                lbl_result.Content = bmi;


                if (bmi > 18.5 && bmi < 24.9)
                {

                    ChangeLabelBMIScoreStyle(WeightType.WeightNormal);
                    GiveTipsForBMI(WeightType.WeightNormal);
                    currentWeightType = 1;

                    if (isMale)
                    {
                        SetPersonImage(WeightType.WeightNormal, 0);
                    }
                    else { SetPersonImage(WeightType.WeightNormal, 1); }

                }
                if (bmi < 18.5)
                {
                    ChangeLabelBMIScoreStyle(WeightType.WeightLow);
                    GiveTipsForBMI(WeightType.WeightLow);
                    currentWeightType = 0;

                    if (isMale)
                    {
                        SetPersonImage(WeightType.WeightLow, 0);
                    }
                    else { SetPersonImage(WeightType.WeightLow, 1); }
                }
                if (bmi >= 25)
                {

                    ChangeLabelBMIScoreStyle(WeightType.WeightHigh);
                    GiveTipsForBMI(WeightType.WeightHigh);
                    currentWeightType = 2;

                    if (isMale)
                    {
                        SetPersonImage(WeightType.WeightHigh, 0);
                    }
                    else { SetPersonImage(WeightType.WeightHigh, 1); }
                }

                LoadOrSaveUsersDatabase(name, gender, age, weight, height, bmi);

            }
            else
            {
                MessageBox.Show("Check your information, some of them are incorrect!");
            }
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
                case WeightType.WeightLow:
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
                case WeightType.WeightNormal:
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
                case WeightType.WeightHigh:
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
                case WeightType.WeightLow:
                    bmiCategory = "underweight_tips";
                    break;
                case WeightType.WeightHigh:
                    bmiCategory = "overweight_tips";
                    break;
                case WeightType.WeightNormal:
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
           
            DatabaseInitializer initializer = new DatabaseInitializer(connectionString);
            initializer.Initialize();

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
            if (lb_users.SelectedItem != null)
            {
                currentName = lb_users.SelectedItem.ToString();
                if (currentName != null)
                    LoadUsersOnSelectionChanged(currentName);
            }
        }
        public void LoadUsersOnStart()
        {
          

            UserRepository userRepository = new UserRepository(connectionString);

            lb_users.Items.Clear();
            List<string> lastusers = userRepository.GetUsers();
            foreach (string users in lastusers)
            {
                lb_users.Items.Add(users);
            }
            currentName = mostRecentUser;

        }

        public void LoadDataFromMostRecentUser()
        {
            string name = GSettings.ReadSetting(mostRecentUser);

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
                // TODO: Database needs to store WeighType of saved user,
                // to be passed when read from db to set
                // ChangeLabelBMIScoreStyle(WeightType someweighttype);

            }
        }

        public void LoadUsersOnSelectionChanged(string name)
        {

          

            UserRepository userRepository = new UserRepository(connectionString);
            UserData user = userRepository.GetUserByName(name);

            if (user != null)
            {
                double bmi = Math.Round(user.BMI, 1); ;
                tb_name.Text = user.Name;
                if (user.Gender == "Male")
                    cb_male.IsChecked = true;
                if (user.Gender == "Female")
                    cb_female.IsChecked = true;
                tb_age.Text = user.Age.ToString();
                tb_weight.Text = user.Weight.ToString();
                tb_height.Text = user.Height.ToString();
                lbl_result.Content = bmi;

                {
                    if (bmi > 18.5 && bmi < 24.9)
                    {

                        ChangeLabelBMIScoreStyle(WeightType.WeightNormal);
                        GiveTipsForBMI(WeightType.WeightNormal);

                        if (isMale)
                        {
                            SetPersonImage(WeightType.WeightNormal, 0);
                        }
                        else { SetPersonImage(WeightType.WeightNormal, 1); }

                    }
                    if (bmi < 18.5)
                    {
                        ChangeLabelBMIScoreStyle(WeightType.WeightLow);
                        GiveTipsForBMI(WeightType.WeightLow);

                        if (isMale)
                        {
                            SetPersonImage(WeightType.WeightLow, 0);
                        }
                        else { SetPersonImage(WeightType.WeightLow, 1); }
                    }
                    if (bmi >= 25)
                    {

                        ChangeLabelBMIScoreStyle(WeightType.WeightHigh);
                        GiveTipsForBMI(WeightType.WeightHigh);

                        if (isMale)
                        {
                            SetPersonImage(WeightType.WeightHigh, 0);
                        }
                        else { SetPersonImage(WeightType.WeightHigh, 1); }
                    }
                }
            }

        }

        public void ChangeLabelBMIScoreStyle(WeightType weightType)
        {
           
            lbl_result.FontSize = 34;
            lbl_result.FontWeight = FontWeights.Bold;
            switch (weightType)
            {
                case WeightType.WeightLow:
                    lbl_result.Foreground = Brushes.LightBlue;
                    return;
                case WeightType.WeightNormal:
                    lbl_result.Foreground = Brushes.Green;
                    return;
                case WeightType.WeightHigh:
                    lbl_result.Foreground = Brushes.OrangeRed;
                    return;

            }
        }
    }
}
