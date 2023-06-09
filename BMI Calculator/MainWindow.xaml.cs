﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BMI_Calculator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    bool isMale = false;
    bool isFemale = false;
    string name = string.Empty;
    string gender = string.Empty;
    double weight;
    double height;
    int age;
    static string mostRecentUser = "mostRecentUser";
    static int currentWeightType = 0;
    static string currentName = string.Empty;

    // Database global constants
    static string databaseFile = "UserData.db";
    static string connectionString = $"Data Source={databaseFile};Version=3;";
    private readonly BMI_Calculator.Window.PersonImage _personImage;
    private readonly BMI_Calculator.Window.BmiHandler _bmiHandler;

    public MainWindow()
    {
        _personImage = new BMI_Calculator.Window.PersonImage(this);
        _bmiHandler = new BMI_Calculator.Window.BmiHandler(this);
        InitializeComponent();

        // Restricting input fields for reasonable values (lazy way)
        tb_name.MaxLength = 20; // I dont think there is a reason to make it longer
        tb_age.MaxLength = 3;
        tb_height.MaxLength = 3;
        tb_weight.MaxLength = 4;

        PopulateList();
        currentName = mostRecentUser;
        LoadDataFromMostRecentUser();
        _personImage = new (this);
    }

    /// <summary>
    /// The `Button_Click` event is triggered when the user clicks the button.
    /// This function first validates the user's inputs, including name, weight, height, and age from the UI.
    /// If the inputs are valid, it calculates the BMI using the user's weight and height.
    ///
    /// Depending on the BMI range, it performs the following operations:
    /// - Updates the UI with the calculated BMI, changing the font size to 34.
    /// - Adjusts the UI styling and provides user tips specific to the BMI category (low weight, normal weight, or high weight).
    /// - Changes the displayed image based on the BMI category and the user's gender.
    /// 
    /// If the inputs are invalid, it shows a MessageBox with an error message specific to the invalid input.
    ///
    /// Finally, it saves the user's data, including the calculated BMI, into the database.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An instance of RoutedEventArgs containing event data.</param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        // Taking input and checking if correct values are entered

        bool isWeightDouble = double.TryParse(tb_weight.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double result_weight);
        bool isHeightDouble = double.TryParse(tb_height.Text, out _);
        bool isAgeInt = int.TryParse(tb_age.Text, out _);
        bool isNameNotString = string.IsNullOrEmpty(tb_name.Text);

        // setting gender values for db and later use
        if (cb_male.IsChecked == true)
            gender = "Male";
        if (cb_female.IsChecked == true)
            gender = "Female";

        // Execute the calculations only if input data are proper
        if (isNameNotString == false && isAgeInt && isHeightDouble && isWeightDouble)
        {

            name = tb_name.Text;
            weight = double.Parse(tb_weight.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            height = double.Parse(tb_height.Text);
            age = int.Parse(tb_age.Text, NumberStyles.Integer);

            double bmi = BMI_Calculator.Window.BmiHandler.CalculateBmi(weight, height);

            lbl_result.FontSize = 34;
            lbl_result.Content = bmi;

            if (bmi is > 18.5 and < 24.9)
            {

                _bmiHandler.ChangeLabelBMIScoreStyle(WeightType.Normal);
                
                string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.Normal);
                lbl_tipscontent.Content = tip;
                
                currentWeightType = 1;

                BMI_Calculator.Window.PersonImage.SetPersonImage( WeightType.Normal, isMale ? 0 : 1);
            }
            else if (bmi < 18.5)
            {
                _bmiHandler.ChangeLabelBMIScoreStyle(WeightType.Low);
                
                string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.Low);
                lbl_tipscontent.Content = tip;
                
                currentWeightType = 0;

                BMI_Calculator.Window.PersonImage.SetPersonImage(WeightType.Low, isMale ? 0 : 1);
            }
            else if (bmi >= 25)
            {

                _bmiHandler.ChangeLabelBMIScoreStyle(WeightType.High);
                
                string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.High);
                lbl_tipscontent.Content = tip;
                
                currentWeightType = 2;

                BMI_Calculator.Window.PersonImage.SetPersonImage(WeightType.High, isMale ? 0 : 1);
            }

            LoadOrSaveUsersDatabase(name, gender, age, weight, height, bmi);
        }
        else
        {
            if (!isWeightDouble)
                MessageBox.Show("Check your weight, you entered incorrect format of weight!");
            if (!isHeightDouble)
                MessageBox.Show("Check your height, you entered incorrect format of height!");
            if (!isAgeInt)
                MessageBox.Show("Check your age, you entered incorrect format of age!");
            if (isNameNotString)
                MessageBox.Show("Check your name, you should use real name!");
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

    /// <summary>
    /// This function initializes a new database connection using the provided connection string.
    /// It then creates a new user repository to interact with the Users table in the database.
    ///
    /// A new user data object is created with the provided name, gender, age, weight, height, and BMI. 
    /// The current timestamp is also recorded. This new user data is then added to the Users table in the database.
    ///
    /// The function also updates the application settings to record the most recent user.
    ///
    /// Finally, the function clears and repopulates the user list in the UI with the updated list of users from the database.
    /// </summary>
    void LoadOrSaveUsersDatabase(string name, string gender, int age, double weight, double height, double bmi)
    {
        DatabaseInitializer initializer = new DatabaseInitializer(connectionString);
        initializer.Initialize();

        // updating Users table
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
        List<string> lastusers = userRepository.GetUserNames();
        foreach (string users in lastusers)
        {
            lb_users.Items.Add(users);
        }

    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lb_users.SelectedItem != null)
        {
            currentName = lb_users.SelectedItem.ToString();
            if (currentName != null)
                LoadUsersOnSelectionChanged(currentName);
        }
    }
    public void PopulateList()
    {
        UserRepository userRepository = new UserRepository(connectionString);

        lb_users.Items.Clear();

        List<string> lastusers = userRepository.GetUserNames();
        if (lastusers != null)
        {
            foreach (string users in lastusers)
            {
                lb_users.Items.Add(users);
            }
        }
    }

    /// <summary>
    /// The `LoadDataFromMostRecentUser` method retrieves the name of the most recent user from the application settings.
    /// It then calls the `LoadUsersOnSelectionChanged` method with the retrieved name.
    ///
    /// The `LoadUsersOnSelectionChanged` method fetches the user data from the database for the given user name. 
    /// If a user with the given name is found, it populates the UI fields with the user's data, including name, gender, age, weight, height, and BMI.
    /// Depending on the user's BMI, it adjusts the UI styling and provides user tips specific to the BMI category (low weight, normal weight, or high weight).
    /// It also changes the displayed image based on the BMI category and the user's gender.
    /// </summary>
    public void LoadDataFromMostRecentUser()
    {
        string name = GSettings.ReadSetting(mostRecentUser);
        if (name != null)
        {
            LoadUsersOnSelectionChanged(name);
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

            // Culture info is used here because for calculations and validations to work i had to settle for . as decimal separator    
            tb_weight.Text = Math.Round(user.Weight, 1).ToString(CultureInfo.InvariantCulture);
            tb_height.Text = user.Height.ToString();
            lbl_result.Content = bmi;

            {
                if (bmi > 18.5 && bmi < 24.9)
                {

                    _bmiHandler.ChangeLabelBMIScoreStyle(WeightType.Normal);
                    
                    string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.Normal);
                    lbl_tipscontent.Content = tip;

                    BMI_Calculator.Window.PersonImage.SetPersonImage(WeightType.Normal, isMale ? 0 : 1);
                }
                if (bmi < 18.5)
                {
                    _bmiHandler.ChangeLabelBMIScoreStyle(WeightType.Low);
                    
                    string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.Low);
                    lbl_tipscontent.Content = tip;

                    if (isMale)
                    {
                        BMI_Calculator.Window.PersonImage.SetPersonImage(WeightType.Low, 0);
                    }
                    else {
                        BMI_Calculator.Window.PersonImage.SetPersonImage(WeightType.Low, 1); }
                }
                if (bmi >= 25)
                {
                    _bmiHandler.ChangeLabelBMIScoreStyle(WeightType.High);
                    
                    string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.High);
                    lbl_tipscontent.Content = tip;

                    if (isMale)
                    {
                        BMI_Calculator.Window.PersonImage.SetPersonImage(WeightType.High, 0);
                    }
                    else {
                        BMI_Calculator.Window.PersonImage.SetPersonImage(WeightType.High, 1); }
                }
            }
        }

    }

    private void DeleteUser_ButtonClick(object sender, RoutedEventArgs e)
    {
        string name = currentName; // currentName should be always storing currently checked user on the list
        UserRepository userRepository = new UserRepository(connectionString);
        bool success = userRepository.RemoveUserByName(name);

        if (success)
        {
            PopulateList();
            ClearInputFields();
        }
        else
        {
            MessageBox.Show("There was some problem removing this user from database!");
        }

    }

    private void ClearInputFields()
    {
        tb_name.Clear();
        tb_age.Clear();
        tb_weight.Clear();
        tb_height.Clear();

        // clearing checkboxes
        cb_female.IsChecked = false;
        cb_male.IsChecked = false;
    }

    private void Button_ClearFields(object sender, RoutedEventArgs e)
    {
        ClearInputFields();
    }
    
    // Validation functions
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }
    private void NumberValidationTextBoxAllowComma(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9.]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void ValidationTextBoxLettersOnly(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^a-zA-Z]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}