using Microsoft.Data.Sqlite;
using System.IO;

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
                "SELECT c.CourseId, c.CourseName FROM Courses c INNER JOIN Enrollments e ON c.CourseId = e.CourseId WHERE e.UserId = @UserId", conn);
            command.Parameters.AddWithValue("@UserId", studentId);
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
            var command = new SqliteCommand("SELECT TestId, TestName, Title, ContentFilePath FROM Tests WHERE TestId = @TestId", conn);
            command.Parameters.AddWithValue("@TestId", testId);
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Test 
                { 
                    TestId = reader.GetInt32(0), 
                    TestName = reader.GetString(1),
                    Title = reader.GetString(2),
                    ContentFilePath = reader.GetString(3)
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
        WHERE CourseId = @CourseId 
        AND TestId NOT IN (
            SELECT CourseId FROM Enrollments 
            WHERE UserId = @UserId AND IsCompleted = 1
        )", conn);

    cmd.Parameters.AddWithValue("@UserId", userId);
    cmd.Parameters.AddWithValue("@CourseId", courseId);

    var result = cmd.ExecuteScalar();
    long count = result == null ? 0 : Convert.ToInt64(result);

    return count == 0;
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
            AvailableFrom = reader.IsDBNull(3) ? null : DateTime.Parse(reader.GetString(3)),
            AvailableUntil = reader.IsDBNull(4) ? null : DateTime.Parse(reader.GetString(4))
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
        SELECT TestId, TestName, Title, ContentFilePath, AvailableFrom, AvailableUntil
        FROM Tests WHERE CourseId = @CourseId", conn);

    cmd.Parameters.AddWithValue("@CourseId", courseId);

    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        tests.Add(new Test
        {
            TestId = reader.GetInt32(0),
            CourseId = courseId,
            TestName = reader.GetString(1),
            Title = reader.GetString(2),
            ContentFilePath = reader.GetString(3),
            AvailableFrom = reader.IsDBNull(4) ? null : DateTime.Parse(reader.GetString(4)),
            AvailableUntil = reader.IsDBNull(5) ? null : DateTime.Parse(reader.GetString(5))
        });
    }

    return tests;
}

public List<User> GetAllUsers()
{
    var users = new List<User>();

    using var connection = new SqliteConnection(ConnectionString);
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = "SELECT Id, Username, Role FROM Users";

    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        users.Add(new User
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            Role = reader.GetString(2)
        });
    }

    return users;
}

public List<Course> GetAllCourses()
{
    var courses = new List<Course>();

    using var connection = new SqliteConnection(ConnectionString);
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = "SELECT Id, Title FROM Courses";

    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        courses.Add(new Course
        {
            Id = reader.GetInt32(0),
            Title = reader.GetString(1)
        });
    }

    return courses;
}

public void DeleteUser(int userId)
{
    using var connection = new SqliteConnection(ConnectionString);
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = "DELETE FROM Users WHERE Id = @id";
    command.Parameters.AddWithValue("@id", userId);
    command.ExecuteNonQuery();
}

public void DeleteCourse(int courseId)
{
    using var connection = new SqliteConnection(ConnectionString);
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = "DELETE FROM Courses WHERE Id = @id";
    command.Parameters.AddWithValue("@id", courseId);
    command.ExecuteNonQuery();
}
}
