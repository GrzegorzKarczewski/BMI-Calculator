using System;
using System.Collections.Generic;

namespace BMI_Calculator.Window; 

public class LoadDatabase {
    private readonly MainWindow _mainWindow;

    public LoadDatabase(MainWindow mainWindow) {
        _mainWindow = mainWindow;
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
    public void LoadOrSaveUsersDatabase(string name, string gender, int age, double weight, double height, double bmi)
    {
        DatabaseInitializer initializer = new DatabaseInitializer(MainWindow.ConnectionString);
        initializer.Initialize();

        // updating Users table
        UserRepository userRepository = new UserRepository(MainWindow.ConnectionString);
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

        GSettings.AddUpdateAppSettings(MainWindow.MostRecentUser, name);

        _mainWindow.lb_users.Items.Clear();
        List<string> lastusers = userRepository.GetUserNames();
        foreach (string users in lastusers)
        {
            _mainWindow.lb_users.Items.Add(users);
        }

    }

    public void PopulateList()
    {
        UserRepository userRepository = new UserRepository(MainWindow.ConnectionString);

        _mainWindow.lb_users.Items.Clear();

        List<string> lastusers = userRepository.GetUserNames();
        if (lastusers != null)
        {
            foreach (string users in lastusers)
            {
                _mainWindow.lb_users.Items.Add(users);
            }
        }
    }
}