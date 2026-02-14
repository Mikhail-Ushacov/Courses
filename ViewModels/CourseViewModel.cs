using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
public class CourseViewModel : INotifyPropertyChanged
{
    private Course currentCourse;
    private readonly DatabaseService _databaseService;

    public Course CurrentCourse
    {
        get { return currentCourse; }
        set
        {
            currentCourse = value;
            OnPropertyChanged(nameof(CurrentCourse));
        }
    }

    public CourseViewModel(int courseId)
    {
        _databaseService = new DatabaseService();
        LoadCourse(courseId);
    }

    private void LoadCourse(int courseId)
    {
        var course = _databaseService.GetCourseById(courseId);
        CurrentCourse = course;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
