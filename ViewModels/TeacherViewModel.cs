using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Courses.Services;
using Courses.Models;
using Courses.Views;

public class TeacherViewModel : INotifyPropertyChanged
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

    public ICommand GoToTeacherPageCommand { get; }
    public ICommand GoToStudentListPageCommand { get; }

    public TeacherViewModel()
    {
        _databaseService = new DatabaseService();

        GoToTeacherPageCommand = new RelayCommand(_ => GoToTeacherPage());
        GoToStudentListPageCommand = new RelayCommand(_ => GoToStudentListPage());

        if (CurrentUser.User != null)
        {
            WelcomeMessage = $"Студент: {CurrentUser.User.Username}";
        }

        LoadCourses();
    }


    private void GoToTeacherPage()
    {
        AppNavigationService.Navigate(new Courses.TeacherPage());
    }

    private void GoToStudentListPage()
    {
        AppNavigationService.Navigate(new Courses.StudentListPage());
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
