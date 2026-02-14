public class DatabaseService
{
    private string connectionString = "Host=myserver;Username=myuser;Password=mypass;Database=mydatabase";

    public List<Course> GetCourses()
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            var command = new NpgsqlCommand("SELECT * FROM Courses", conn);
            var reader = command.ExecuteReader();

            List<Course> courses = new List<Course>();
            while (reader.Read())
            {
                courses.Add(new Course { CourseId = reader.GetInt32(0), CourseName = reader.GetString(1) });
            }
            return courses;
        }
    }
}
