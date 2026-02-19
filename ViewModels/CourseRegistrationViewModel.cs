using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Courses.Services;
public class CourseRegistrationViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Course> availableCourses;
    private readonly DatabaseService _databaseService;
    private readonly RegistrationService _registrationService;

    public ObservableCollection<Course> AvailableCourses
    {
        get { return availableCourses; }
        set
        {
            availableCourses = value;
            OnPropertyChanged(nameof(AvailableCourses));
        }
    }

    public ICommand RegisterCommand { get; }

    public CourseRegistrationViewModel()
    {
        _databaseService = new DatabaseService();
        _registrationService = new RegistrationService();
        RegisterCommand = new RelayCommand(RegisterForCourse);
        LoadAvailableCourses();
    }

    private void LoadAvailableCourses()
    {
        var courses = _databaseService.GetCourses();
        AvailableCourses = new ObservableCollection<Course>(courses);
    }

    private void RegisterForCourse(object parameter)
    {
        if (parameter is Course course)
        {
            var isRegistered = _registrationService.RegisterStudentToCourse(CurrentUser.User?.UserId ?? 0, course.CourseId); // Replace with actual student ID
            if (isRegistered)
            {
                // Notify user about successful registration
            }
            else
            {
                // Notify user about failure
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
