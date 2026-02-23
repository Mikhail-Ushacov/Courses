using System.Globalization;
using Microsoft.Data.Sqlite;
using System.IO;
using Courses.Models;

public class DatabaseService
{
    private static readonly string DbPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Courses",
        "courses.db");

    private string connectionString = $"Data Source={DbPath};";

    public string ConnectionString => connectionString;

    public DatabaseService()
    {
        var directory = Path.GetDirectoryName(DbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public List<Course> GetCourses()
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            var command = new SqliteCommand("SELECT * FROM Courses", conn);
            var reader = command.ExecuteReader();

            List<Course> courses = new List<Course>();
            while (reader.Read())
            {
                courses.Add(new Course 
                { 
                    CourseId = reader.GetInt32(0), 
                    CourseName = reader.GetString(1) 
                });
            }
            return courses;
        }
    }

    public List<Course> GetEnrolledCourses(int studentId)
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            var command = new SqliteCommand(
                "SELECT c.CourseId, c.CourseName, e.FinalGrade FROM Courses c INNER JOIN Enrollments e ON c.CourseId = e.CourseId WHERE e.UserId = @UserId", conn);
            command.Parameters.AddWithValue("@UserId", studentId);
            var reader = command.ExecuteReader();

            List<Course> courses = new List<Course>();
            while (reader.Read())
            {
                courses.Add(new Course 
                { 
                    CourseId = reader.GetInt32(0), 
                    CourseName = reader.GetString(1),
                    FinalGrade = reader.IsDBNull(2) ? null : reader.GetDouble(2)
                });
            }
            return courses;
        }
    }

    public List<Course> GetCoursesByTeacher(int teacherId)
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            var command = new SqliteCommand("SELECT CourseId, CourseName FROM Courses WHERE TeacherId = @TeacherId", conn);
            command.Parameters.AddWithValue("@TeacherId", teacherId);
            var reader = command.ExecuteReader();

            List<Course> courses = new List<Course>();
            while (reader.Read())
            {
                courses.Add(new Course 
                { 
                    CourseId = reader.GetInt32(0), 
                    CourseName = reader.GetString(1) 
                });
            }
            return courses;
        }
    }

    public Course? GetCourseById(int courseId)
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            var command = new SqliteCommand("SELECT CourseId, CourseName FROM Courses WHERE CourseId = @CourseId", conn);
            command.Parameters.AddWithValue("@CourseId", courseId);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Course 
                { 
                    CourseId = reader.GetInt32(0), 
                    CourseName = reader.GetString(1) 
                };
            }
            return null;
        }
    }

    public Lecture? GetLectureById(int lectureId)
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            var command = new SqliteCommand("SELECT LectureId, Title, ContentFilePath FROM Lectures WHERE LectureId = @LectureId", conn);
            command.Parameters.AddWithValue("@LectureId", lectureId);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Lecture 
                { 
                    LectureId = reader.GetInt32(0), 
                    Title = reader.GetString(1), 
                    ContentFilePath = reader.GetString(2) 
                };
            }
            return null;
        }
    }

    public Test? GetTestById(int testId)
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            var command = new SqliteCommand("SELECT TestId, CourseId, TestName, ContentFilePath, AvailableFrom, AvailableUntil, IsFinalTest, TestMax FROM Tests WHERE TestId = @TestId", conn);
            command.Parameters.AddWithValue("@TestId", testId);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Test 
                { 
                    TestId = reader.GetInt32(0),
                    CourseId = reader.GetInt32(1),
                    TestName = reader.GetString(2),
                    ContentFilePath = reader.GetString(3),
                    AvailableFrom = reader.IsDBNull(4) ? null : DateTimeOffset.Parse(reader.GetString(4), CultureInfo.InvariantCulture),
                    AvailableUntil = reader.IsDBNull(5) ? null : DateTimeOffset.Parse(reader.GetString(5), CultureInfo.InvariantCulture),
                    IsFinalTest = !reader.IsDBNull(6) && reader.GetInt32(6) == 1,
                    TestMax = reader.IsDBNull(7) ? 0 : reader.GetDouble(7)
                };
            }
            return null;
        }
    }

    public List<Question> GetQuestionsByTestId(int testId)
{
    var questions = new List<Question>();

    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand("SELECT QuestionId, QuestionText FROM Questions WHERE TestId = @TestId", conn);
    cmd.Parameters.AddWithValue("@TestId", testId);

    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        var q = new Question
        {
            QuestionId = reader.GetInt32(0),
            QuestionText = reader.GetString(1),
            AnswerOptions = GetAnswersByQuestionId(reader.GetInt32(0))
        };

        questions.Add(q);
    }

    return questions;
}

private List<AnswerOption> GetAnswersByQuestionId(int questionId)
{
    var answers = new List<AnswerOption>();

    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand("SELECT AnswerId, AnswerText, Points FROM AnswerOptions WHERE QuestionId = @QuestionId", conn);
    cmd.Parameters.AddWithValue("@QuestionId", questionId);

    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        answers.Add(new AnswerOption
        {
            AnswerId = reader.GetInt32(0),
            AnswerText = reader.GetString(1),
            Points = reader.GetInt32(2)
        });
    }

    return answers;
}

public void SaveFinalGrade(int userId, int courseId, double grade)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand(@"
        UPDATE Enrollments 
        SET FinalGrade = @Grade, IsCompleted = 1
        WHERE UserId = @UserId AND CourseId = @CourseId", conn);

    cmd.Parameters.AddWithValue("@Grade", grade);
    cmd.Parameters.AddWithValue("@UserId", userId);
    cmd.Parameters.AddWithValue("@CourseId", courseId);

    cmd.ExecuteNonQuery();
}

public bool AllCourseTestsCompleted(int userId, int courseId)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand(@"
        SELECT COUNT(*) FROM Tests 
        WHERE CourseId = @CourseId AND IsFinalTest = 0
        AND TestId NOT IN (
            SELECT TestId FROM TestResults 
            WHERE UserId = @UserId
        )", conn);

    cmd.Parameters.AddWithValue("@UserId", userId);
    cmd.Parameters.AddWithValue("@CourseId", courseId);

    var result = cmd.ExecuteScalar();
    long count = result == null ? 0 : Convert.ToInt64(result);

    return count == 0;
}

public bool HasTestResult(int userId, int testId)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand("SELECT COUNT(*) FROM TestResults WHERE UserId = @UserId AND TestId = @TestId", conn);
    cmd.Parameters.AddWithValue("@UserId", userId);
    cmd.Parameters.AddWithValue("@TestId", testId);

    var result = cmd.ExecuteScalar();
    return Convert.ToInt64(result) > 0;
}

public void SaveTestResult(int userId, int courseId, int testId, double testMark, double maxMark)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand(@"
        INSERT OR REPLACE INTO TestResults (CourseId, TestId, UserId, TestMark, MaxMark, CompletedAt)
        VALUES (@CourseId, @TestId, @UserId, @TestMark, @MaxMark, @CompletedAt)", conn);

    cmd.Parameters.AddWithValue("@CourseId", courseId);
    cmd.Parameters.AddWithValue("@TestId", testId);
    cmd.Parameters.AddWithValue("@UserId", userId);
    cmd.Parameters.AddWithValue("@TestMark", testMark);
    cmd.Parameters.AddWithValue("@MaxMark", maxMark);
    cmd.Parameters.AddWithValue("@CompletedAt", DateTimeOffset.UtcNow.ToString("o"));

    cmd.ExecuteNonQuery();
}

public TestResult? GetTestResult(int userId, int testId)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand(@"
        SELECT ResultId, CourseId, TestId, UserId, TestMark, MaxMark, CompletedAt
        FROM TestResults WHERE UserId = @UserId AND TestId = @TestId", conn);

    cmd.Parameters.AddWithValue("@UserId", userId);
    cmd.Parameters.AddWithValue("@TestId", testId);

    using var reader = cmd.ExecuteReader();

    if (reader.Read())
    {
        return new TestResult
        {
            ResultId = reader.GetInt32(0),
            CourseId = reader.GetInt32(1),
            TestId = reader.GetInt32(2),
            UserId = reader.GetInt32(3),
            TestMark = reader.GetDouble(4),
            MaxMark = reader.GetDouble(5),
            CompletedAt = DateTimeOffset.Parse(reader.GetString(6), CultureInfo.InvariantCulture)
        };
    }
    return null;
}

public List<TestResult> GetTestResultsByCourse(int userId, int courseId)
{
    var results = new List<TestResult>();

    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand(@"
        SELECT ResultId, CourseId, TestId, UserId, TestMark, MaxMark, CompletedAt
        FROM TestResults WHERE UserId = @UserId AND CourseId = @CourseId", conn);

    cmd.Parameters.AddWithValue("@UserId", userId);
    cmd.Parameters.AddWithValue("@CourseId", courseId);

    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        results.Add(new TestResult
        {
            ResultId = reader.GetInt32(0),
            CourseId = reader.GetInt32(1),
            TestId = reader.GetInt32(2),
            UserId = reader.GetInt32(3),
            TestMark = reader.GetDouble(4),
            MaxMark = reader.GetDouble(5),
            CompletedAt = DateTimeOffset.Parse(reader.GetString(6), CultureInfo.InvariantCulture)
        });
    }

    return results;
}

public List<TestDisplay> GetTestDisplaysForUser(int userId, int courseId)
{
    var displays = new List<TestDisplay>();

    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand(@"
        SELECT t.TestId, t.CourseId, t.TestName, t.IsFinalTest, t.AvailableFrom, t.AvailableUntil,
               tr.TestMark, tr.MaxMark
        FROM Tests t
        LEFT JOIN TestResults tr ON t.TestId = tr.TestId AND tr.UserId = @UserId
        WHERE t.CourseId = @CourseId", conn);

    cmd.Parameters.AddWithValue("@UserId", userId);
    cmd.Parameters.AddWithValue("@CourseId", courseId);

    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        displays.Add(new TestDisplay
        {
            TestId = reader.GetInt32(0),
            CourseId = reader.GetInt32(1),
            TestName = reader.GetString(2),
            IsFinalTest = !reader.IsDBNull(3) && reader.GetInt32(3) == 1,
            AvailableFrom = reader.IsDBNull(4) ? null : DateTimeOffset.Parse(reader.GetString(4), CultureInfo.InvariantCulture),
            AvailableUntil = reader.IsDBNull(5) ? null : DateTimeOffset.Parse(reader.GetString(5), CultureInfo.InvariantCulture),
            IsCompleted = !reader.IsDBNull(6),
            Score = reader.IsDBNull(6) ? null : reader.GetDouble(6),
            MaxScore = reader.IsDBNull(7) ? null : reader.GetDouble(7)
        });
    }

    return displays;
}

public Test? GetFinalTestByCourse(int courseId)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand(@"
        SELECT TestId, CourseId, TestName, ContentFilePath, AvailableFrom, AvailableUntil, IsFinalTest, TestMax
        FROM Tests WHERE CourseId = @CourseId AND IsFinalTest = 1", conn);

    cmd.Parameters.AddWithValue("@CourseId", courseId);

    using var reader = cmd.ExecuteReader();

    if (reader.Read())
    {
        return new Test
        {
            TestId = reader.GetInt32(0),
            CourseId = reader.GetInt32(1),
            TestName = reader.GetString(2),
            ContentFilePath = reader.GetString(3),
            AvailableFrom = reader.IsDBNull(4) ? null : DateTimeOffset.Parse(reader.GetString(4), CultureInfo.InvariantCulture),
            AvailableUntil = reader.IsDBNull(5) ? null : DateTimeOffset.Parse(reader.GetString(5), CultureInfo.InvariantCulture),
            IsFinalTest = true,
            TestMax = reader.IsDBNull(7) ? 0 : reader.GetDouble(7)
        };
    }
    return null;
}

public bool SetTestAsFinal(int courseId, int testId)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    using var transaction = conn.BeginTransaction();

    try
    {
        var checkCmd = new SqliteCommand(
            "SELECT COUNT(*) FROM Tests WHERE CourseId = @CourseId AND IsFinalTest = 1 AND TestId != @TestId", 
            conn, transaction);
        checkCmd.Parameters.AddWithValue("@CourseId", courseId);
        checkCmd.Parameters.AddWithValue("@TestId", testId);

        var existingFinal = Convert.ToInt64(checkCmd.ExecuteScalar());
        if (existingFinal > 0)
        {
            transaction.Rollback();
            return false;
        }

        var updateCmd = new SqliteCommand(
            "UPDATE Tests SET IsFinalTest = 1 WHERE TestId = @TestId AND CourseId = @CourseId", 
            conn, transaction);
        updateCmd.Parameters.AddWithValue("@TestId", testId);
        updateCmd.Parameters.AddWithValue("@CourseId", courseId);
        updateCmd.ExecuteNonQuery();

        transaction.Commit();
        return true;
    }
    catch
    {
        transaction.Rollback();
        return false;
    }
}

public void UnsetFinalTest(int courseId, int testId)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand(
        "UPDATE Tests SET IsFinalTest = 0 WHERE TestId = @TestId AND CourseId = @CourseId", conn);
    cmd.Parameters.AddWithValue("@TestId", testId);
    cmd.Parameters.AddWithValue("@CourseId", courseId);
    cmd.ExecuteNonQuery();
}
public List<Lecture> GetLecturesByCourseId(int courseId)
{
    var lectures = new List<Lecture>();

    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand(@"
        SELECT LectureId, Title, ContentFilePath, AvailableFrom, AvailableUntil
        FROM Lectures WHERE CourseId = @CourseId", conn);

    cmd.Parameters.AddWithValue("@CourseId", courseId);

    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        lectures.Add(new Lecture
        {
            LectureId = reader.GetInt32(0),
            CourseId = courseId,
            Title = reader.GetString(1),
            ContentFilePath = reader.GetString(2),
            AvailableFrom = reader.IsDBNull(3) ? null : DateTimeOffset.Parse(reader.GetString(3), CultureInfo.InvariantCulture),
            AvailableUntil = reader.IsDBNull(4) ? null : DateTimeOffset.Parse(reader.GetString(4), CultureInfo.InvariantCulture)
        });
    }

    return lectures;
}

public List<Test> GetTestsByCourseId(int courseId)
{
    var tests = new List<Test>();

    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand(@"
        SELECT TestId, CourseId, TestName, ContentFilePath, AvailableFrom, AvailableUntil, IsFinalTest, TestMax
        FROM Tests WHERE CourseId = @CourseId", conn);

    cmd.Parameters.AddWithValue("@CourseId", courseId);

    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        tests.Add(new Test
        {
            TestId = reader.GetInt32(0),
            CourseId = reader.GetInt32(1),
            TestName = reader.GetString(2),
            ContentFilePath = reader.GetString(3),
            AvailableFrom = reader.IsDBNull(4) ? null : DateTimeOffset.Parse(reader.GetString(4), CultureInfo.InvariantCulture),
            AvailableUntil = reader.IsDBNull(5) ? null : DateTimeOffset.Parse(reader.GetString(5), CultureInfo.InvariantCulture),
            IsFinalTest = !reader.IsDBNull(6) && reader.GetInt32(6) == 1,
            TestMax = reader.IsDBNull(7) ? 0 : reader.GetDouble(7)
        });
    }

    return tests;
}
    public List<Course> GetAllCourses()
    {
        var courses = new List<Course>();

        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        var cmd = new SqliteCommand("SELECT CourseId, CourseName FROM Courses", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            courses.Add(new Course
            {
                CourseId = reader.GetInt32(0),
                CourseName = reader.GetString(1)
            });
        }

        return courses;
    }

    public List<User> GetAllUsers() {
        var users = new List<User>();
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        var cmd = new SqliteCommand("SELECT UserId, Username, Password, UserType FROM Users", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) {
            users.Add(new User {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                UserType = (UserType)reader.GetInt32(3)
            });
        }
        return users;
    }

public void DeleteUser(int userId)
{
    using var connection = new SqliteConnection(ConnectionString);
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = "DELETE FROM Users WHERE UserId = @id";
    command.Parameters.AddWithValue("@id", userId);
    command.ExecuteNonQuery();
}

public void DeleteCourse(int courseId)
{
    using var connection = new SqliteConnection(ConnectionString);
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = "DELETE FROM Courses WHERE CourseId = @id";
    command.Parameters.AddWithValue("@id", courseId);
    command.ExecuteNonQuery();
}

    public double? GetFinalGrade(int userId, int courseId)
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        var cmd = new SqliteCommand(@"
        SELECT FinalGrade FROM Enrollments
        WHERE UserId = @UserId AND CourseId = @CourseId", conn);

        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@CourseId", courseId);

        var result = cmd.ExecuteScalar();

        if (result == null || result == DBNull.Value)
            return null;

        return Convert.ToDouble(result);
    }

    public List<(User student, double? grade)> GetStudentsByCourse(int courseId)
{
    var result = new List<(User, double?)>();

    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand(@"
        SELECT u.UserId, u.Username, e.FinalGrade
        FROM Enrollments e
        JOIN Users u ON u.UserId = e.UserId
        WHERE e.CourseId = @CourseId AND u.UserType = 0", conn); // 0 = Student

    cmd.Parameters.AddWithValue("@CourseId", courseId);

    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        var student = new User
        {
            UserId = reader.GetInt32(0),
            Username = reader.GetString(1)
        };

        double? grade = reader.IsDBNull(2) ? null : reader.GetDouble(2);

        result.Add((student, grade));
    }

    return result;
}

public void DeleteLecture(int lectureId)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();
    var cmd = new SqliteCommand("DELETE FROM Lectures WHERE LectureId = @id", conn);
    cmd.Parameters.AddWithValue("@id", lectureId);
    cmd.ExecuteNonQuery();
}

public void DeleteTest(int testId)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();
    var cmd = new SqliteCommand("DELETE FROM Tests WHERE TestId = @id", conn);
    cmd.Parameters.AddWithValue("@id", testId);
    cmd.ExecuteNonQuery();
}

public void UpdateFinalGrade(int userId, int courseId, double newGrade)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();
    var cmd = new SqliteCommand("UPDATE Enrollments SET FinalGrade = @grade WHERE UserId = @uid AND CourseId = @cid", conn);
    cmd.Parameters.AddWithValue("@grade", newGrade);
    cmd.Parameters.AddWithValue("@uid", userId);
    cmd.Parameters.AddWithValue("@cid", courseId);
    cmd.ExecuteNonQuery();
}

public void DeleteEnrollment(int userId, int courseId)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();
    var cmd = new SqliteCommand("DELETE FROM Enrollments WHERE UserId = @uid AND CourseId = @cid", conn);
    cmd.Parameters.AddWithValue("@uid", userId);
    cmd.Parameters.AddWithValue("@cid", courseId);
    cmd.ExecuteNonQuery();
}

// Метод для додавання/оновлення лекції в БД
public int SaveLectureToDb(int courseId, string title, string filePath, int? lectureId = null)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();
    SqliteCommand cmd;
    if (lectureId.HasValue) {
        cmd = new SqliteCommand("UPDATE Lectures SET Title=@t, ContentFilePath=@p WHERE LectureId=@id", conn);
        cmd.Parameters.AddWithValue("@id", lectureId.Value);
    } else {
        cmd = new SqliteCommand("INSERT INTO Lectures (CourseId, Title, ContentFilePath) VALUES (@cid, @t, @p); SELECT last_insert_rowid();", conn);
        cmd.Parameters.AddWithValue("@cid", courseId);
    }
    cmd.Parameters.AddWithValue("@t", title);
    cmd.Parameters.AddWithValue("@p", filePath);
    
    var result = cmd.ExecuteScalar();
    return lectureId ?? Convert.ToInt32(result);
}

public int SaveTestToDb(int courseId, string name, string filePath, int? testId = null)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();
    SqliteCommand cmd;
    if (testId.HasValue) {
        cmd = new SqliteCommand("UPDATE Tests SET TestName=@n, ContentFilePath=@p WHERE TestId=@id", conn);
        cmd.Parameters.AddWithValue("@id", testId.Value);
    } else {
        cmd = new SqliteCommand("INSERT INTO Tests (CourseId, TestName, ContentFilePath) VALUES (@cid, @n, @p); SELECT last_insert_rowid();", conn);
        cmd.Parameters.AddWithValue("@cid", courseId);
    }
    cmd.Parameters.AddWithValue("@n", name);
    cmd.Parameters.AddWithValue("@p", filePath);
    
    var result = cmd.ExecuteScalar();
    return testId ?? Convert.ToInt32(result);
}
}
