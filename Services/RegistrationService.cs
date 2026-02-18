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
    using var conn = new SqliteConnection(_databaseService.ConnectionString);
    conn.Open();

    var check = new SqliteCommand(
        "SELECT COUNT(*) FROM Enrollments WHERE UserId = @UserId AND CourseId = @CourseId",
        conn);

    check.Parameters.AddWithValue("@UserId", studentId);
    check.Parameters.AddWithValue("@CourseId", courseId);

    var result = cmd.ExecuteScalar();
    long count = result == null ? 0 : Convert.ToInt64(result);

    if (count > 0)
        return false;

    var command = new SqliteCommand(
        "INSERT INTO Enrollments (UserId, CourseId, EnrollmentDate) VALUES (@UserId, @CourseId, @Date)",
        conn);

    command.Parameters.AddWithValue("@UserId", studentId);
    command.Parameters.AddWithValue("@CourseId", courseId);
    command.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

    return command.ExecuteNonQuery() > 0;
}

}
