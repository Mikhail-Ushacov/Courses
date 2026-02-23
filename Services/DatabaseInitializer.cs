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
        CreateEnrollments(connection);
        CreateTestResults(connection);
        AddTestColumns(connection);
    }

    private void AddTestColumns(SqliteConnection connection)
    {
        var columns = new List<string>();
        using (var cmd = new SqliteCommand("PRAGMA table_info(Tests);", connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                columns.Add(reader.GetString(1));
            }
        }

        if (!columns.Contains("IsFinalTest"))
        {
            using var cmd = new SqliteCommand("ALTER TABLE Tests ADD COLUMN IsFinalTest INTEGER DEFAULT 0;", connection);
            cmd.ExecuteNonQuery();
        }

        if (!columns.Contains("TestMax"))
        {
            using var cmd = new SqliteCommand("ALTER TABLE Tests ADD COLUMN TestMax REAL DEFAULT 0;", connection);
            cmd.ExecuteNonQuery();
        }
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
            Salt TEXT NOT NULL,
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

    private void CreateTestResults(SqliteConnection connection)
    {
        string sql = @"
        CREATE TABLE IF NOT EXISTS TestResults (
            ResultId INTEGER PRIMARY KEY AUTOINCREMENT,
            CourseId INTEGER NOT NULL,
            TestId INTEGER NOT NULL,
            UserId INTEGER NOT NULL,
            TestMark REAL NOT NULL,
            MaxMark REAL NOT NULL,
            CompletedAt TEXT NOT NULL,
            UNIQUE(TestId, UserId),
            FOREIGN KEY (CourseId) REFERENCES Courses(CourseId) ON DELETE CASCADE,
            FOREIGN KEY (TestId) REFERENCES Tests(TestId) ON DELETE CASCADE,
            FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
        );";

        new SqliteCommand(sql, connection).ExecuteNonQuery();
    }
}
