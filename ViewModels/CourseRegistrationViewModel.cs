using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Courses.Services;
using Courses.Models;

public class CourseRegistrationViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Course> availableCourses;
    private Course? selectedCourse;
    private readonly DatabaseService _databaseService;
    private readonly RegistrationService _registrationService;
    private readonly IMessageBoxService _messageBoxService;

    public ObservableCollection<Course> AvailableCourses
    {
        get { return availableCourses; }
        set
        {
            availableCourses = value;
            OnPropertyChanged(nameof(AvailableCourses));
        }
    }

    public Course? SelectedCourse
    {
        get { return selectedCourse; }
        set
        {
            selectedCourse = value;
            OnPropertyChanged(nameof(SelectedCourse));
        }
    }

    public ICommand RegisterCommand { get; }

    public CourseRegistrationViewModel()
    {
        _databaseService = new DatabaseService();
        _registrationService = new RegistrationService();
        _messageBoxService = new MessageBoxService();
        RegisterCommand = new RelayCommand(RegisterForCourse);
        LoadAvailableCourses();
    }

    private void LoadAvailableCourses()
    {
        var userId = CurrentUser.User?.UserId ?? 0;
        var allCourses = _databaseService.GetCourses();
        var enrolledCourses = _databaseService.GetEnrolledCourses(userId);
        
        var enrolledCourseIds = new HashSet<int>(enrolledCourses.Select(c => c.CourseId));
        var availableCoursesList = allCourses.Where(c => !enrolledCourseIds.Contains(c.CourseId)).ToList();
        
        AvailableCourses = new ObservableCollection<Course>(availableCoursesList);
    }

    private async void RegisterForCourse(object? parameter)
    {
        if (parameter is not Course course) return;

        var userId = CurrentUser.User?.UserId ?? 0;
        var isRegistered = _registrationService.RegisterStudentToCourse(userId, course.CourseId);
        
        if (isRegistered)
        {
            await _messageBoxService.ShowMessageAsync(
                "Реєстрація успішна",
                $"Ви успішно зареєстровані на курс \"{course.CourseName}\"!");
            
            LoadAvailableCourses();
            SelectedCourse = null;
        }
        else
        {
            await _messageBoxService.ShowMessageAsync(
                "Помилка реєстрації",
                $"Ви вже зареєстровані на курс \"{course.CourseName}\".");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
