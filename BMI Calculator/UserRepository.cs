using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Windows.Media.Animation;

namespace BMI_Calculator
{
    public class UserRepository
    /*
    The `UserRepository` class is responsible for the data access operations related to the `Users` table in a SQLite database.
  
    The class constructor accepts a connection string parameter, which is used to establish a connection with the database.

    There are three main methods in this class:

    1. `AddUser(UserData user)`: This method takes a `UserData` object as a parameter and inserts its properties into the `Users` table. 
    The `UserData` object contains details such as Name, Gender, Age, Weight, Height, BMI, and Timestamp. 
    The Timestamp is converted to a string format before being inserted into the table.

    2. `GetUserByName(string name)`: This method accepts a `name` parameter and returns a `UserData` object. 
    The method queries the `Users` table in the database for a row where the `Name` column matches the given `name` parameter. 
    If such a row is found, the method populates a new `UserData` object with the row's data and returns it. 
    If no such row is found, the method returns null.

    3. `GetUsers()`: This method queries the `Users` table for distinct user names and returns a list of these names. 
    If no users are found, it returns an empty list.

    All database connections are properly disposed of using `using` blocks to ensure that resources are freed after use.
    */




    {
        private string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(UserData user)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var cmd = new SQLiteCommand("INSERT INTO Users (Name, Gender, Age, Weight, Height, BMI, Timestamp) VALUES (@Name, @Gender, @Age, @Weight, @Height, @BMI, @Timestamp)", connection);

                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Gender", user.Gender);
                cmd.Parameters.AddWithValue("@Age", user.Age);
                cmd.Parameters.AddWithValue("@Weight", user.Weight);
                cmd.Parameters.AddWithValue("@Height", user.Height);
                cmd.Parameters.AddWithValue("@BMI", user.BMI);

                cmd.Parameters.AddWithValue("@Timestamp", user.Timestamp.ToString("s"));

                cmd.ExecuteNonQuery();
            }
        }

        public UserData GetUserByName(string name)
        {
            UserData user = null;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Users WHERE Name = @Name ORDER BY timestamp DESC LIMIT 1", connection);
                cmd.Parameters.AddWithValue("@Name", name);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserData
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Gender = reader.GetString(reader.GetOrdinal("Gender")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            Weight = reader.GetFloat(reader.GetOrdinal("Weight")),
                            Height = reader.GetFloat(reader.GetOrdinal("Height")),
                            BMI = reader.GetFloat(reader.GetOrdinal("BMI")),
                            Timestamp = DateTime.Parse(reader.GetString(7))
                        };
                    }
                }
            }

            return user;
        }
        public bool RemoveUserByName(string name)
        {
            int check = 0;
            using (var connection = new SQLiteConnection(_connectionString))
            {
              
                connection.Open();
                var cmd = new SQLiteCommand("DELETE FROM Users WHERE Name = @Name", connection);
                cmd.Parameters.AddWithValue("@Name", name);
                check = cmd.ExecuteNonQuery();
            }
            if (check > 0) 
            { 
                return true; 
            } else 
            { 
                return false; 
            }
           
       
        }

        public List<string> GetUsers()
        {

            List<string> users = new List<string>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var cmd = new SQLiteCommand("SELECT DISTINCT Name from Users", connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(reader.GetString(0));
                    }

                }
            }
            return users;
        }
    }
}
