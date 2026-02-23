using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Courses.Services;
using Courses.Models;
using Courses.Views;

public class CourseTestDisplay : INotifyPropertyChanged
{
    private bool _isFinalTest;
    
    public int TestId { get; set; }
    public int CourseId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    
    public bool IsFinalTest
    {
        get => _isFinalTest;
        set
        {
            _isFinalTest = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFinalTest)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public class TeacherViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Course> availableCourses;
    private ObservableCollection<CourseTestDisplay> courseTests;
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

    public ObservableCollection<CourseTestDisplay> CourseTests
    {
        get { return courseTests; }
        set
        {
            courseTests = value;
            OnPropertyChanged(nameof(CourseTests));
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
    public ICommand ToggleFinalTestCommand { get; }

    public TeacherViewModel()
    {
        _databaseService = new DatabaseService();

        GoToTeacherPageCommand = new RelayCommand(_ => GoToTeacherPage());
        GoToStudentListPageCommand = new RelayCommand(_ => GoToStudentListPage());
        ToggleFinalTestCommand = new RelayCommand(ToggleFinalTest);

        if (CurrentUser.User != null)
        {
            WelcomeMessage = $"Викладач: {CurrentUser.User.Username}";
        }

        LoadCourses();
        LoadTests();
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

    private void LoadTests()
    {
        var tests = new List<CourseTestDisplay>();
        var courses = _databaseService.GetCourses();

        foreach (var course in courses)
        {
            var courseTests = _databaseService.GetTestsByCourseId(course.CourseId);
            foreach (var test in courseTests)
            {
                tests.Add(new CourseTestDisplay
                {
                    TestId = test.TestId,
                    CourseId = test.CourseId,
                    TestName = test.TestName,
                    CourseName = course.CourseName,
                    IsFinalTest = test.IsFinalTest
                });
            }
        }

        CourseTests = new ObservableCollection<CourseTestDisplay>(tests);
    }

    private void ToggleFinalTest(object? parameter)
    {
        if (parameter is CourseTestDisplay test)
        {
            if (test.IsFinalTest)
            {
                var success = _databaseService.SetTestAsFinal(test.CourseId, test.TestId);
                if (!success)
                {
                    test.IsFinalTest = false;
                    System.Windows.MessageBox.Show("Цей курс вже має підсумковий тест. Спочатку зніміть позначку з іншого тесту.", 
                        "Попередження", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                }
            }
            else
            {
                _databaseService.UnsetFinalTest(test.CourseId, test.TestId);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
