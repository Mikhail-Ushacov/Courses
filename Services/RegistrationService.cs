using Microsoft.Data.Sqlite;

public class RegistrationService
{
    private readonly DatabaseService _databaseService;

    public RegistrationService()
    {
        _databaseService = new DatabaseService();
    }

    public bool RegisterStudentToCourse(int studentId, int courseId)
    {
        using (var conn = new SqliteConnection(_databaseService.ConnectionString))
        {
            conn.Open();
            var command = new SqliteCommand("INSERT INTO Enrollments (UserId, CourseId, EnrollmentDate) VALUES (@UserId, @CourseId, @EnrollmentDate)", conn);
            command.Parameters.AddWithValue("@UserId", studentId);
            command.Parameters.AddWithValue("@CourseId", courseId);
            command.Parameters.AddWithValue("@EnrollmentDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}
