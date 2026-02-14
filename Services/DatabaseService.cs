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
}
