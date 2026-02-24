using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Courses.Services;
using Courses.Models;

public class CourseRegistrationViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Course> availableCourses;
    private Course? selectedCourse;
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
        RegisterCommand = new RelayCommand(RegisterForCourse);
        LoadAvailableCourses();
    }

    private void LoadAvailableCourses()
    {
        var courses = _databaseService.GetCourses();
        AvailableCourses = new ObservableCollection<Course>(courses);
    }

    private void RegisterForCourse(object? parameter)
    {
        if (parameter is not Course course) return;

        var userId = CurrentUser.User?.UserId ?? 0;
        var isRegistered = _registrationService.RegisterStudentToCourse(userId, course.CourseId);
        
        if (isRegistered)
        {
            System.Windows.MessageBox.Show(
                $"Ви успішно зареєстровані на курс \"{course.CourseName}\"!",
                "Реєстрація успішна",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
            
            LoadAvailableCourses();
            SelectedCourse = null;
        }
        else
        {
            System.Windows.MessageBox.Show(
                $"Ви вже зареєстровані на курс \"{course.CourseName}\".",
                "Помилка реєстрації",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
