using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BMI_Calculator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    internal bool isMale = false;
    bool isFemale = false;
    string name = string.Empty;
    string gender = string.Empty;
    double weight;
    double height;
    int age;
    internal static string mostRecentUser = "mostRecentUser";
    static int currentWeightType = 0;
    static string currentName = string.Empty;

    // Database global constants
    static string databaseFile = "UserData.db";
    internal static string connectionString = $"Data Source={databaseFile};Version=3;";
    private readonly BMI_Calculator.Window.PersonImage _personImage;
    internal readonly BMI_Calculator.Window.BmiHandler _bmiHandler;
    private readonly BMI_Calculator.Window.LoadData _loadData;
    private readonly BMI_Calculator.Window.LoadDatabase _loadDatabase;

    public MainWindow()
    {
        _loadData = new(this);
        _personImage = new(this);
        _bmiHandler = new(this);
        _loadDatabase = new (this);
        
        InitializeComponent();

        // Restricting input fields for reasonable values (lazy way)
        tb_name.MaxLength = 20; // I dont think there is a reason to make it longer
        tb_age.MaxLength = 3;
        tb_height.MaxLength = 3;
        tb_weight.MaxLength = 4;
        
        _loadDatabase.PopulateList();

        currentName = mostRecentUser;
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

            _loadDatabase.LoadOrSaveUsersDatabase(name, gender, age, weight, height, bmi);
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

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lb_users.SelectedItem != null)
        {
            currentName = lb_users.SelectedItem.ToString();
            if (currentName != null) _loadData.LoadUsersOnSelectionChanged(currentName);
        }
    }

    private void DeleteUser_ButtonClick(object sender, RoutedEventArgs e)
    {
        string name = currentName; // currentName should be always storing currently checked user on the list
        UserRepository userRepository = new UserRepository(connectionString);
        bool success = userRepository.RemoveUserByName(name);

        if (success)
        {
            _loadDatabase.PopulateList();
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