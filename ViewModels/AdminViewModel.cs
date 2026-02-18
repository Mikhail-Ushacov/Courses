using System.Collections.ObjectModel;
using System.Windows.Input;

public class AdminViewModel
{
    private readonly DatabaseService _databaseService;

    public ObservableCollection<User> Users { get; set; }
    public ObservableCollection<Course> Courses { get; set; }

    public ICommand DeleteUserCommand { get; }
    public ICommand DeleteCourseCommand { get; }

    public AdminViewModel()
    {
        _databaseService = new DatabaseService();

        Users = new ObservableCollection<User>(_databaseService.GetAllUsers());
        Courses = new ObservableCollection<Course>(_databaseService.GetAllCourses());

        DeleteUserCommand = new RelayCommand(DeleteUser);
        DeleteCourseCommand = new RelayCommand(DeleteCourse);
    }

    private void DeleteUser(object parameter)
    {
        if (parameter is User user)
        {
            _databaseService.DeleteUser(user.UserId);
            Users.Remove(user);
        }
    }

    private void DeleteCourse(object parameter)
    {
        if (parameter is Course course)
        {
            _databaseService.DeleteCourse(course.CourseId);
            Courses.Remove(course);
        }
    }

    public List<User> GetAllUsers()
{
    var users = new List<User>();

    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand("SELECT UserId, Username, Password, UserType FROM Users", conn);
    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        users.Add(new User
        {
            UserId = reader.GetInt32(0),
            Username = reader.GetString(1),
            Password = reader.GetString(2),
            UserType = reader.GetInt32(3)
        });
    }

    return users;
}

public void DeleteUser(int userId)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand("DELETE FROM Users WHERE UserId = @Id", conn);
    cmd.Parameters.AddWithValue("@Id", userId);
    cmd.ExecuteNonQuery();
}

public void DeleteCourse(int courseId)
{
    using var conn = new SqliteConnection(ConnectionString);
    conn.Open();

    var cmd = new SqliteCommand("DELETE FROM Courses WHERE CourseId = @Id", conn);
    cmd.Parameters.AddWithValue("@Id", courseId);
    cmd.ExecuteNonQuery();
}

}
