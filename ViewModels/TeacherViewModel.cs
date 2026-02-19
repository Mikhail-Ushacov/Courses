using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Courses.Services;

public class TeacherViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Course> managedCourses;
    private readonly DatabaseService _databaseService;

    public ObservableCollection<Course> ManagedCourses
    {
        get { return managedCourses; }
        set
        {
            managedCourses = value;
            OnPropertyChanged(nameof(ManagedCourses));
        }
    }

    public TeacherViewModel()
    {
        _databaseService = new DatabaseService();
        LoadManagedCourses();
    }

    private void LoadManagedCourses()
    {
        var userId = CurrentUser.User?.UserId ?? 0;
        var courses = _databaseService.GetCoursesByTeacher(userId);
        ManagedCourses = new ObservableCollection<Course>(courses);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
