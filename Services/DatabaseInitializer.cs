using Microsoft.Data.Sqlite;
using System.IO;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Initialize()
    {
        var dbPath = GetDatabasePath();
        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        CreateCoursesTable(connection);
        CreateEnrollmentsTable(connection);
        CreateLecturesTable(connection);
        CreateTestsTable(connection);
        CreateUsersTable(connection);
    }

    private string GetDatabasePath()
    {
        var parts = _connectionString.Split(';');
        foreach (var part in parts)
        {
            if (part.TrimStart().StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
            {
                return part.Substring("Data Source=".Length).Trim();
            }
        }
        return "courses.db";
    }

    private void CreateCoursesTable(SqliteConnection connection)
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS Courses (
                CourseId INTEGER PRIMARY KEY AUTOINCREMENT,
                CourseName TEXT NOT NULL,
                TeacherId INTEGER NOT NULL
            );";

        using var command = new SqliteCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    private void CreateEnrollmentsTable(SqliteConnection connection)
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS Enrollments (
                EnrollmentId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                CourseId INTEGER NOT NULL,
                EnrollmentDate TEXT NOT NULL
            );";

        using var command = new SqliteCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    private void CreateLecturesTable(SqliteConnection connection)
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS Lectures (
                LectureId INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                ContentFilePath TEXT NOT NULL
            );";

        using var command = new SqliteCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    private void CreateTestsTable(SqliteConnection connection)
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS Tests (
                TestId INTEGER PRIMARY KEY AUTOINCREMENT,
                TestName TEXT NOT NULL,
                Title TEXT NOT NULL,
                ContentFilePath TEXT NOT NULL,
                AvailableFrom TEXT,
                AvailableUntil TEXT
            );";

        using var command = new SqliteCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    private void CreateUsersTable(SqliteConnection connection)
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS Users (
                UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL,
                Password TEXT NOT NULL,
                UserType INTEGER NOT NULL
            );";

        using var command = new SqliteCommand(sql, connection);
        command.ExecuteNonQuery();
    }
}
