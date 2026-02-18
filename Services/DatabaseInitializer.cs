using Microsoft.Data.Sqlite;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        EnableForeignKeys(connection);

        CreateUsers(connection);
        CreateCourses(connection);
        CreateLectures(connection);
        CreateTests(connection);
        CreateQuestions(connection);
        CreateAnswerOptions(connection);
        CreateEnrollments(connection);
    }

    private void EnableForeignKeys(SqliteConnection connection)
    {
        using var cmd = new SqliteCommand("PRAGMA foreign_keys = ON;", connection);
        cmd.ExecuteNonQuery();
    }

    private void CreateUsers(SqliteConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS Users (
            UserId INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT NOT NULL UNIQUE,
            Password TEXT NOT NULL,
            UserType INTEGER NOT NULL
        );";

        new SqliteCommand(sql, connection).ExecuteNonQuery();
    }

    private void CreateCourses(SqliteConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS Courses (
            CourseId INTEGER PRIMARY KEY AUTOINCREMENT,
            CourseName TEXT NOT NULL,
            TeacherId INTEGER NOT NULL,
            StartDate TEXT,
            EndDate TEXT,
            FOREIGN KEY (TeacherId) REFERENCES Users(UserId) ON DELETE CASCADE
        );";

        new SqliteCommand(sql, connection).ExecuteNonQuery();
    }

    private void CreateLectures(SqliteConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS Lectures (
            LectureId INTEGER PRIMARY KEY AUTOINCREMENT,
            CourseId INTEGER NOT NULL,
            Title TEXT NOT NULL,
            ContentFilePath TEXT NOT NULL,
            AvailableFrom TEXT,
            AvailableUntil TEXT,
            FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE
        );";

        new SqliteCommand(sql, connection).ExecuteNonQuery();
    }

    private void CreateTests(SqliteConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS Tests (
            TestId INTEGER PRIMARY KEY AUTOINCREMENT,
            CourseId INTEGER NOT NULL,
            TestName TEXT NOT NULL,
            ContentFilePath TEXT NOT NULL,
            AvailableFrom TEXT,
            AvailableUntil TEXT,
            FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE
        );";

        new SqliteCommand(sql, connection).ExecuteNonQuery();
    }

    private void CreateQuestions(SqliteConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS Questions (
            QuestionId INTEGER PRIMARY KEY AUTOINCREMENT,
            TestId INTEGER NOT NULL,
            QuestionText TEXT NOT NULL,
            FOREIGN KEY (TestId) REFERENCES Tests(TestId) ON DELETE CASCADE
        );";

        new SqliteCommand(sql, connection).ExecuteNonQuery();
    }

    private void CreateAnswerOptions(SqliteConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS AnswerOptions (
            AnswerId INTEGER PRIMARY KEY AUTOINCREMENT,
            QuestionId INTEGER NOT NULL,
            AnswerText TEXT NOT NULL,
            Points INTEGER NOT NULL,
            FOREIGN KEY (QuestionId) REFERENCES Questions(QuestionId) ON DELETE CASCADE
        );";

        new SqliteCommand(sql, connection).ExecuteNonQuery();
    }

    private void CreateEnrollments(SqliteConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS Enrollments (
            EnrollmentId INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER NOT NULL,
            CourseId INTEGER NOT NULL,
            EnrollmentDate TEXT NOT NULL,
            IsCompleted INTEGER DEFAULT 0,
            FinalGrade REAL DEFAULT 0,
            UNIQUE(UserId, CourseId),
            FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
            FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE
        );";

        new SqliteCommand(sql, connection).ExecuteNonQuery();
    }
}
