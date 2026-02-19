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
                string salt;
                string hash = AuthService.HashPassword("123", out salt);

                var cmd1 = new SqliteCommand(
                    "INSERT INTO Users (Username, Password, Salt, UserType) VALUES (@Username, @Password, @Salt, @UserType)",
                    connection, transaction);
                cmd1.Parameters.AddWithValue("@Username", "AdminTeacher");
                cmd1.Parameters.AddWithValue("@Password", hash);
                cmd1.Parameters.AddWithValue("@Salt", salt);
                cmd1.Parameters.AddWithValue("@UserType", (int)UserType.Teacher);
                cmd1.ExecuteNonQuery();

                var cmd2 = new SqliteCommand("INSERT INTO Courses (CourseName, TeacherId) VALUES ('C# Programming', 1)", connection, transaction);
                cmd2.ExecuteNonQuery();

                transaction.Commit();
            }
            catch { transaction.Rollback(); }
        }
    }
}
