using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;
using Courses.Models;

namespace Courses.Services
{
    public class AuthService
    {
        private readonly string _connectionString;

        public AuthService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static string HashPassword(string password, out string salt)
        {
            byte[] saltBytes = new byte[16];
            RandomNumberGenerator.Fill(saltBytes);
            salt = Convert.ToBase64String(saltBytes);

            string passwordWithSalt = password + salt;
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(passwordWithSalt));
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            string passwordWithSalt = password + storedSalt;
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(passwordWithSalt));
            string computedHash = Convert.ToBase64String(hashBytes);
            return computedHash == storedHash;
        }

        public User? Authenticate(string username, string password)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand(
                "SELECT UserId, Username, Password, Salt, UserType FROM Users WHERE Username = @Username",
                conn);
            cmd.Parameters.AddWithValue("@Username", username);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int userId = reader.GetInt32(0);
                string storedUsername = reader.GetString(1);
                string storedHash = reader.GetString(2);
                string storedSalt = reader.GetString(3);
                int userType = reader.GetInt32(4);

                if (VerifyPassword(password, storedHash, storedSalt))
                {
                        return new User
                        {
                            UserId = userId,
                            Username = storedUsername,
                            Password = storedHash,
                            UserType = (UserType)userType
                        };
                }
            }

            return null;
        }

        public bool RegisterStudent(string username, string password)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var checkCmd = new SqliteCommand(
                "SELECT COUNT(*) FROM Users WHERE Username = @Username",
                conn);
            checkCmd.Parameters.AddWithValue("@Username", username);
            long count = (long)checkCmd.ExecuteScalar();

            if (count > 0)
            {
                return false;
            }

            string salt;
            string passwordHash = HashPassword(password, out salt);

            var insertCmd = new SqliteCommand(
                "INSERT INTO Users (Username, Password, Salt, UserType) VALUES (@Username, @Password, @Salt, @UserType)",
                conn);
            insertCmd.Parameters.AddWithValue("@Username", username);
            insertCmd.Parameters.AddWithValue("@Password", passwordHash);
            insertCmd.Parameters.AddWithValue("@Salt", salt);
            insertCmd.Parameters.AddWithValue("@UserType", (int)UserType.Student);

            insertCmd.ExecuteNonQuery();
            return true;
        }

        public bool UsernameExists(string username)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = new SqliteCommand(
                "SELECT COUNT(*) FROM Users WHERE Username = @Username",
                conn);
            cmd.Parameters.AddWithValue("@Username", username);

            long count = (long)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}
