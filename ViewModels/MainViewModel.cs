using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

public class MainViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Course> availableCourses;
    private readonly DatabaseService _databaseService;

    public ObservableCollection<Course> AvailableCourses
    {
        get { return availableCourses; }
        set
        {
            availableCourses = value;
            OnPropertyChanged(nameof(AvailableCourses));
        }
    }

    public ICommand GoToCoursesCommand { get; }
    public ICommand GoToLoginCommand { get; }

    public MainViewModel()
    {
        _databaseService = new DatabaseService();
        GoToCoursesCommand = new RelayCommand(GoToCourses);
        GoToLoginCommand = new RelayCommand(GoToLogin);

        LoadCourses();
    }

    private void LoadCourses()
    {
        var courses = _databaseService.GetCourses();
        AvailableCourses = new ObservableCollection<Course>(courses);
    }

    private void GoToCourses()
    {
        // Logic to navigate to course page
    }

    private void GoToLogin()
    {
        // Logic to navigate to login page
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
