using Courses.Models;
using Courses.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Courses.Views;

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
                StudentsPerformanceView = CollectionViewSource.GetDefaultView(_studentsPerformance);
                OnPropertyChanged(nameof(StudentsPerformance));
            }
        }

        public ICollectionView StudentsPerformanceView
        {
            get => _performanceView;
            private set { _performanceView = value; OnPropertyChanged(nameof(StudentsPerformanceView)); }
        }

        // Команди
        public ICommand NavigateToAddLectureCommand { get; }
        public ICommand NavigateToEditLectureCommand { get; }
        public ICommand DeleteLectureCommand { get; }
        public ICommand NavigateToAddTestCommand { get; }
        public ICommand NavigateToEditTestCommand { get; }
        public ICommand DeleteTestCommand { get; }
        public ICommand ToggleFinalTestCommand { get; }
        public ICommand EditGradeCommand { get; }
        public ICommand SortCommand { get; }

        public TeacherPageViewModel()
        {
            _databaseService = new DatabaseService();

            // Навігація для Лекцій
            NavigateToAddLectureCommand = new RelayCommand(_ => {
                var course = ManagedCourses.FirstOrDefault();
                if (course != null) AppNavigationService.Navigate(new LectureEditPage(course.CourseId));
                else MessageBox.Show("У вас немає курсів для додавання лекцій.");
            });

            NavigateToEditLectureCommand = new RelayCommand(param => {
                if (param is LectureDisplay lecture) 
                    AppNavigationService.Navigate(new LectureEditPage(lecture.CourseId, lecture.LectureId));
            });

            // Навігація для Тестів
            NavigateToAddTestCommand = new RelayCommand(_ => {
                var course = ManagedCourses.FirstOrDefault();
                if (course != null) AppNavigationService.Navigate(new TestEditPage(course.CourseId));
                else MessageBox.Show("У вас немає курсів для додавання тестів.");
            });

            NavigateToEditTestCommand = new RelayCommand(param => {
                if (param is CourseTestDisplay test)
                    AppNavigationService.Navigate(new TestEditPage(test.CourseId, test.TestId));
            });

            // Видалення
            DeleteLectureCommand = new RelayCommand(param => {
                if (param is LectureDisplay lecture) {
                    _databaseService.DeleteLecture(lecture.LectureId);
                    LoadData();
                }
            });

            DeleteTestCommand = new RelayCommand(param => {
                if (param is CourseTestDisplay test) {
                    _databaseService.DeleteTest(test.TestId);
                    LoadData();
                }
            });

            ToggleFinalTestCommand = new RelayCommand(ToggleFinalTest);
            EditGradeCommand = new RelayCommand(EditGrade);
            SortCommand = new RelayCommand(SortPerformance);

            LoadData();
        }

        public void LoadData()
        {
            if (CurrentUser.User == null) return;
            int teacherId = CurrentUser.User.UserId;

            var courses = _databaseService.GetCoursesByTeacher(teacherId);
            ManagedCourses = new ObservableCollection<Course>(courses);

            var lecturesList = new List<LectureDisplay>();
            var testsList = new List<CourseTestDisplay>();
            var perfList = new List<StudentPerformanceDisplay>();

            foreach (var course in courses)
            {
                var lectures = _databaseService.GetLecturesByCourseId(course.CourseId);
                foreach (var l in lectures) lecturesList.Add(new LectureDisplay { 
                    LectureId = l.LectureId, CourseId = l.CourseId, Title = l.Title, CourseName = course.CourseName 
                });

                var tests = _databaseService.GetTestsByCourseId(course.CourseId);
                foreach (var t in tests) testsList.Add(new CourseTestDisplay { 
                    TestId = t.TestId, CourseId = t.CourseId, TestName = t.TestName, CourseName = course.CourseName, IsFinalTest = t.IsFinalTest 
                });

                var students = _databaseService.GetStudentsByCourse(course.CourseId);
                foreach (var s in students) perfList.Add(new StudentPerformanceDisplay {
                    UserId = s.student.UserId, CourseId = course.CourseId, StudentName = s.student.Username, CourseName = course.CourseName, Grade = s.grade ?? 0
                });
            }

            AllLectures = new ObservableCollection<LectureDisplay>(lecturesList);
            CourseTests = new ObservableCollection<CourseTestDisplay>(testsList);
            StudentsPerformance = new ObservableCollection<StudentPerformanceDisplay>(perfList);
        }

        private void ToggleFinalTest(object? parameter)
        {
            if (parameter is CourseTestDisplay test)
            {
                if (test.IsFinalTest)
                {
                    if (!_databaseService.SetTestAsFinal(test.CourseId, test.TestId))
                    {
                        test.IsFinalTest = false;
                        MessageBox.Show("Цей курс вже має підсумковий тест.");
                    }
                }
                else _databaseService.UnsetFinalTest(test.CourseId, test.TestId);
            }
        }

        private void EditGrade(object? parameter)
        {
            if (parameter is StudentPerformanceDisplay perf)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox($"Нова оцінка для {perf.StudentName}:", "Редагування", perf.Grade.ToString());
                if (double.TryParse(input, out double val))
                {
                    _databaseService.UpdateFinalGrade(perf.UserId, perf.CourseId, val);
                    LoadData();
                }
            }
        }

        private void SortPerformance(object? parameter)
        {
            if (parameter is string column && StudentsPerformanceView != null)
            {
                var sd = StudentsPerformanceView.SortDescriptions;
                var dir = sd.Count > 0 && sd[0].PropertyName == column && sd[0].Direction == ListSortDirection.Ascending 
                    ? ListSortDirection.Descending : ListSortDirection.Ascending;
                sd.Clear();
                sd.Add(new SortDescription(column, dir));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}