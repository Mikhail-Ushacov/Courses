using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Courses.Services;
using Courses.Models;

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
        var userId = CurrentUser.User?.UserId ?? 0;
        var courses = _databaseService.GetEnrolledCourses(userId);
        EnrolledCourses = new ObservableCollection<Course>(courses);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
