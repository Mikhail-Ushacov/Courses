using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
public class StudentViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Course> enrolledCourses;
    private readonly DatabaseService _databaseService;

    public ObservableCollection<Course> EnrolledCourses
    {
        get { return enrolledCourses; }
        set
        {
            enrolledCourses = value;
            OnPropertyChanged(nameof(EnrolledCourses));
        }
    }

    public StudentViewModel()
    {
        _databaseService = new DatabaseService();
        LoadEnrolledCourses();
    }

    private void LoadEnrolledCourses()
    {
        // Fetch enrolled courses for student
        var courses = _databaseService.GetEnrolledCourses(1); // Replace with actual student ID
        EnrolledCourses = new ObservableCollection<Course>(courses);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
