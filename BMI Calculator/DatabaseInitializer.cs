using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMI_Calculator
{
    using System.Data.SQLite;

    public class DatabaseInitializer
    {
        private string _connectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Age INTEGER,
                    Weight REAL,
                    Height REAL,
                    BMI REAL
                );";

                using (var cmd = new SQLiteCommand(createUsersTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            } // Note for myself 
            // At this point,(thanks to using "using") the connection.Dispose() method is automatically called, closing the connection and releasing resources.
        }
    }

}
