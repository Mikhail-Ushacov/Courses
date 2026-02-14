using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
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
        var courses = _databaseService.GetCoursesByTeacher(1); // Replace with actual teacher ID
        ManagedCourses = new ObservableCollection<Course>(courses);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
