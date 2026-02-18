using System.Collections.ObjectModel;
using System.Windows.Input;

public class CourseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly AccessControlService _accessService;

    private readonly int _userId;

    public ObservableCollection<Lecture> Lectures { get; set; }
    public ObservableCollection<Test> Tests { get; set; }

    public ICommand OpenLectureCommand { get; }
    public ICommand OpenTestCommand { get; }

    public CourseViewModel(int courseId, int userId)
    {
        _databaseService = new DatabaseService();
        _accessService = new AccessControlService();
        _userId = userId;

        Lectures = new ObservableCollection<Lecture>(
            _databaseService.GetLecturesByCourseId(courseId));

        Tests = new ObservableCollection<Test>(
            _databaseService.GetTestsByCourseId(courseId));

        OpenLectureCommand = new RelayCommand(OpenLecture);
        OpenTestCommand = new RelayCommand(OpenTest);
    }

    private void OpenLecture(object parameter)
    {
        if (parameter is Lecture lecture)
        {
            if (!_accessService.IsAvailableNow(lecture.AvailableFrom, lecture.AvailableUntil))
                return;

            AppNavigationService.Navigate(new LecturePage(lecture.LectureId));        }
    }

    private void OpenTest(object parameter)
    {
        if (parameter is Test test)
        {
            if (!_accessService.IsAvailableNow(test.AvailableFrom, test.AvailableUntil))
                return;

            if (test.IsFinalTest && !_databaseService.AllCourseTestsCompleted(_userId, test.CourseId))
                return;

            AppNavigationService.Navigate(new TestPage(test.TestId, _userId, test.CourseId));
        }
    }
}
