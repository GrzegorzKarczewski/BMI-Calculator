using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace BMI_Calculator
{
    public class UserRepository
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

                var cmd = new SQLiteCommand("INSERT INTO Users (Name, Age, Weight, Height, BMI) VALUES (@Name,@Age, @Weight, @Height, @BMI)", connection);

                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Age", user.Age);
                cmd.Parameters.AddWithValue("@Weight", user.Weight);
                cmd.Parameters.AddWithValue("@Height", user.Height);
                cmd.Parameters.AddWithValue("@BMI", user.BMI);

                cmd.ExecuteNonQuery();
            }
        }

        public UserData GetUserByName(string name)
        {
            UserData user = null;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var cmd = new SQLiteCommand("SELECT * FROM Users WHERE Name = @Name", connection);
                cmd.Parameters.AddWithValue("@Name", name);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserData
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            Weight = reader.GetFloat(reader.GetOrdinal("Weight")),
                            Height = reader.GetFloat(reader.GetOrdinal("Height")),
                            BMI = reader.GetFloat(reader.GetOrdinal("BMI"))
                        };
                    }
                }
            }

            return user;
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
                    if (reader.Read())
                    {
                        users.Add(reader.GetString(reader.GetOrdinal("Name")));
                    }

                }
            }
            return users;
        }
    }
}
