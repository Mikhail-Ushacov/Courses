using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Courses.Services;

public class MainViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Course> availableCourses;
    private readonly DatabaseService _databaseService;
    private string _welcomeMessage = "Мій Кабінет";

    public ObservableCollection<Course> AvailableCourses
    {
        get { return availableCourses; }
        set
        {
            availableCourses = value;
            OnPropertyChanged(nameof(AvailableCourses));
        }
    }

    public string WelcomeMessage
    {
        get { return _welcomeMessage; }
        set
        {
            _welcomeMessage = value;
            OnPropertyChanged(nameof(WelcomeMessage));
        }
    }

    public ICommand GoToMainPageCommand { get; }
    public ICommand GoToStudentPageCommand { get; }
    public ICommand GoToTeacherPageCommand { get; }
    public ICommand GoToRegistrationCommand { get; }

    public MainViewModel()
    {
        _databaseService = new DatabaseService();

        GoToMainPageCommand = new RelayCommand(_ => GoToMainPage());
        GoToStudentPageCommand = new RelayCommand(_ => GoToStudentPage());
        GoToTeacherPageCommand = new RelayCommand(_ => GoToTeacherPage());
        GoToRegistrationCommand = new RelayCommand(_ => GoToRegistration());

        if (CurrentUser.User != null)
        {
            WelcomeMessage = CurrentUser.IsTeacher 
                ? $"Викладач: {CurrentUser.User.Username}" 
                : $"Студент: {CurrentUser.User.Username}";
        }

        LoadCourses();
    }

    private void GoToMainPage()
    {
        AppNavigationService.Navigate(new Courses.Views.MainPage());
    }

    private void GoToStudentPage()
    {
        AppNavigationService.Navigate(new Courses.StudentPage());
    }

    private void GoToTeacherPage()
    {
        AppNavigationService.Navigate(new Courses.TeacherPage());
    }

    private void GoToRegistration()
    {
        AppNavigationService.Navigate(new Courses.CourseRegistrationPage());
    }

    private void LoadCourses()
    {
        var courses = _databaseService.GetCourses();
        AvailableCourses = new ObservableCollection<Course>(courses);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
