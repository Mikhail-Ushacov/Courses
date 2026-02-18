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
}
