using System.Collections.ObjectModel;
using System.Windows.Input;
using Wpf.Ui.Controls;
using Courses;

public class CourseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly AccessControlService _accessService;
    private readonly int _userId;
    private readonly int _courseId;

    public ObservableCollection<Lecture> Lectures { get; set; }
    public ObservableCollection<TestDisplay> Tests { get; set; }

    public ICommand OpenLectureCommand { get; }
    public ICommand OpenTestCommand { get; }

    public CourseViewModel(int courseId, int userId)
    {
        _databaseService = new DatabaseService();
        _accessService = new AccessControlService();
        _userId = userId;
        _courseId = courseId;

        Lectures = new ObservableCollection<Lecture>(
            _databaseService.GetLecturesByCourseId(courseId));

        Tests = new ObservableCollection<TestDisplay>(
            _databaseService.GetTestDisplaysForUser(userId, courseId));

        OpenLectureCommand = new RelayCommand(OpenLecture);
        OpenTestCommand = new RelayCommand(OpenTest);
    }

    private async void OpenLecture(object parameter)
    {
        if (parameter is Lecture lecture)
        {
            if (!_accessService.IsAvailableNow(lecture.AvailableFrom, lecture.AvailableUntil))
            {
                await ShowMessageAsync("Недоступно", "Лекція недоступна у цей час");
                return;
            }

            AppNavigationService.Navigate(new LecturePage(lecture.LectureId));
        }
    }

    private async void OpenTest(object parameter)
    {
        if (parameter is TestDisplay test)
        {
            if (test.IsCompleted)
            {
                await ShowMessageAsync("Завершено", "Тест вже пройдено");
                return;
            }

            if (!_accessService.IsAvailableNow(test.AvailableFrom, test.AvailableUntil))
            {
                await ShowMessageAsync("Недоступно", "Тест недоступний у цей час");
                return;
            }

            if (test.IsFinalTest && !_databaseService.AllCourseTestsCompleted(_userId, test.CourseId))
            {
                await ShowMessageAsync("Заблоковано", "Спочатку пройдіть усі тести курсу");
                return;
            }

            AppNavigationService.Navigate(new TestPage(test.TestId, _userId, test.CourseId));
        }
    }

    private static async System.Threading.Tasks.Task ShowMessageAsync(string title, string message)
    {
        var messageBox = new MessageBox
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK"
        };
        await messageBox.ShowDialogAsync();
    }
}
