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
    internal bool IsMale; // false
    private string _name = string.Empty;
    private string _gender = string.Empty;
    private double _weight;
    private double _height;
    private int _age;
    internal static readonly string MostRecentUser = "mostRecentUser";
    private static int _currentWeightType = 0;
    private static string _currentName = string.Empty;

    // Database global constants
    private static readonly string DatabaseFile = "UserData.db";
    internal static readonly string ConnectionString = $"Data Source={DatabaseFile};Version=3;";
    private readonly BMI_Calculator.Window.PersonImage _personImage;
    private readonly BMI_Calculator.Window.LoadData _loadData;
    private readonly BMI_Calculator.Window.LoadDatabase _loadDatabase;
    internal readonly BMI_Calculator.Window.BmiHandler BmiHandler;

    public MainWindow()
    {
        _loadData = new(this);
        _personImage = new(this);
        BmiHandler = new(this);
        _loadDatabase = new(this);

        InitializeComponent();

        // Restricting input fields for reasonable values (lazy way)
        tb_name.MaxLength = 20; // I dont think there is a reason to make it longer
        tb_age.MaxLength = 3;
        tb_height.MaxLength = 3;
        tb_weight.MaxLength = 4;

        _loadData.LoadDataFromMostRecentUser(); // Loading most recent user data
        _loadDatabase.PopulateList(); // Loading all users from database

        _currentName = MostRecentUser;
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
            _gender = "Male";
        if (cb_female.IsChecked == true)
            _gender = "Female";

        // Execute the calculations only if input data are proper
        if (isNameNotString == false && isAgeInt && isHeightDouble && isWeightDouble)
        {
            _name = tb_name.Text;
            _weight = double.Parse(tb_weight.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            _height = double.Parse(tb_height.Text);
            _age = int.Parse(tb_age.Text, NumberStyles.Integer);

            double bmi = BMI_Calculator.Window.BmiHandler.CalculateBmi(_weight, _height);

            lbl_result.FontSize = 34;
            lbl_result.Content = bmi;

            switch (bmi)
            {
                case > 18.5 and < 24.9:
                    {
                        BmiHandler.ChangeLabelBMIScoreStyle(WeightType.Normal);

                        string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.Normal);
                        lbl_tipscontent.Content = tip;

                        _currentWeightType = 1;

                        _personImage.SetPersonImage(WeightType.Normal, IsMale ? 0 : 1);
                        break;
                    }
                case < 18.5:
                    {
                        BmiHandler.ChangeLabelBMIScoreStyle(WeightType.Low);

                        string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.Low);
                        lbl_tipscontent.Content = tip;

                        _currentWeightType = 0;

                        _personImage.SetPersonImage(WeightType.Low, IsMale ? 0 : 1);
                        break;
                    }
                case >= 25:
                    {
                        BmiHandler.ChangeLabelBMIScoreStyle(WeightType.High);

                        string tip = BMI_Calculator.Window.BmiHandler.GiveTipsForBmi(WeightType.High);
                        lbl_tipscontent.Content = tip;

                        _currentWeightType = 2;

                        _personImage.SetPersonImage(WeightType.High, IsMale ? 0 : 1);
                        break;
                    }
            }
            _loadDatabase.LoadOrSaveUsersDatabase(_name, _gender, _age, _weight, _height, bmi);
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
        if (cb_male.IsChecked != true) return;
        IsMale = true;
        cb_female.IsChecked = false;
    }

    private void cb_female_Checked(object sender, RoutedEventArgs e)
    {
        // This is checking if female is checked and unsets male to be safe
        if (cb_female.IsChecked != true) return;
        IsMale = false;
        cb_male.IsChecked = false;
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lb_users.SelectedItem == null) return;
        _currentName = lb_users.SelectedItem.ToString();
        if (_currentName != null) _loadData.LoadUsersOnSelectionChanged(_currentName);
    }

    private void DeleteUser_ButtonClick(object sender, RoutedEventArgs e)
    {
        string name = _currentName; // currentName should be always storing currently checked user on the list
        UserRepository userRepository = new UserRepository(ConnectionString);
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

    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void DragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }
}