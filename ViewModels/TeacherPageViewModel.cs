using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Courses.Models;
using Courses.Services;

namespace Courses.ViewModels
{
    public class TeacherPageViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Course> _managedCourses;
        private ObservableCollection<LectureDisplay> _allLectures;
        private ObservableCollection<CourseTestDisplay> _courseTests;
        private ObservableCollection<StudentPerformanceDisplay> _studentsPerformance;
        private ICollectionView _performanceView;

        public ObservableCollection<Course> ManagedCourses
        {
            get => _managedCourses;
            set { _managedCourses = value; OnPropertyChanged(nameof(ManagedCourses)); }
        }

        public ObservableCollection<LectureDisplay> AllLectures
        {
            get => _allLectures;
            set { _allLectures = value; OnPropertyChanged(nameof(AllLectures)); }
        }

        public ObservableCollection<CourseTestDisplay> CourseTests
        {
            get => _courseTests;
            set { _courseTests = value; OnPropertyChanged(nameof(CourseTests)); }
        }

        public ObservableCollection<StudentPerformanceDisplay> StudentsPerformance
        {
            get => _studentsPerformance;
            set
            {
                _studentsPerformance = value;
                _performanceView = CollectionViewSource.GetDefaultView(_studentsPerformance);
                OnPropertyChanged(nameof(StudentsPerformance));
            }
        }

        public ICommand SortCommand { get; }
        public ICommand ToggleFinalTestCommand { get; }

        public TeacherPageViewModel()
        {
            _databaseService = new DatabaseService();
            SortCommand = new RelayCommand(SortPerformance);
            ToggleFinalTestCommand = new RelayCommand(ToggleFinalTest);
            LoadData();
        }

        private void LoadData()
        {
            if (CurrentUser.User == null) return;

            int teacherId = CurrentUser.User.UserId;

            // 1. Завантажуємо курси саме цього викладача
            var courses = _databaseService.GetCoursesByTeacher(teacherId);
            ManagedCourses = new ObservableCollection<Course>(courses);

            var lecturesList = new List<LectureDisplay>();
            var testsList = new List<CourseTestDisplay>();
            var performanceList = new List<StudentPerformanceDisplay>();

            foreach (var course in courses)
            {
                // 2. Завантажуємо Лекції
                var lectures = _databaseService.GetLecturesByCourseId(course.CourseId);
                foreach (var l in lectures)
                {
                    lecturesList.Add(new LectureDisplay
                    {
                        Title = l.Title,
                        CourseName = course.CourseName,
                        FilePath = l.ContentFilePath
                    });
                }

                // 3. Завантажуємо Тести
                var tests = _databaseService.GetTestsByCourseId(course.CourseId);
                foreach (var t in tests)
                {
                    testsList.Add(new CourseTestDisplay
                    {
                        TestId = t.TestId,
                        CourseId = t.CourseId,
                        TestName = t.TestName,
                        CourseName = course.CourseName,
                        IsFinalTest = t.IsFinalTest
                    });
                }

                // 4. Завантажуємо Успішність (тільки завершені)
                var studentGrades = _databaseService.GetStudentsByCourse(course.CourseId);
                foreach (var sg in studentGrades)
                {
                    if (sg.grade.HasValue)
                    {
                        performanceList.Add(new StudentPerformanceDisplay
                        {
                            StudentName = sg.student.Username,
                            CourseName = course.CourseName,
                            Grade = sg.grade.Value
                        });
                    }
                }
            }

            AllLectures = new ObservableCollection<LectureDisplay>(lecturesList);
            CourseTests = new ObservableCollection<CourseTestDisplay>(testsList);
            StudentsPerformance = new ObservableCollection<StudentPerformanceDisplay>(performanceList);
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

        private void SortPerformance(object parameter)
        {
            string column = parameter as string;
            if (string.IsNullOrEmpty(column) || _performanceView == null) return;

            _performanceView.SortDescriptions.Clear();
            _performanceView.SortDescriptions.Add(new SortDescription(column, ListSortDirection.Ascending));
            _performanceView.Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
