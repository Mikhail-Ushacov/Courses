using Microsoft.Data.Sqlite;
using System;

namespace Courses.Services
{
    public class ContentInitializer
    {
        private readonly string _connectionString;
        public ContentInitializer(string conn) => _connectionString = conn;

        public void SeedInitialData()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM Users", connection);
            if ((long)checkCmd.ExecuteScalar() > 0) return;

            using var transaction = connection.BeginTransaction();
            try
            {
                // Insert Teacher (UserType 1)
                var cmd1 = new SqliteCommand("INSERT INTO Users (Username, Password, UserType) VALUES ('AdminTeacher', '123', 1)", connection, transaction);
                cmd1.ExecuteNonQuery();

                // Insert Course
                var cmd2 = new SqliteCommand("INSERT INTO Courses (CourseName, TeacherId) VALUES ('C# Programming', 1)", connection, transaction);
                cmd2.ExecuteNonQuery();

                transaction.Commit();
            }
            catch { transaction.Rollback(); }
        }
    }
}