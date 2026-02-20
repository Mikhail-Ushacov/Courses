using Courses.Models;
using Courses.Views;
using Courses.Services;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Courses.Services;

namespace Courses
{
    public partial class CoursePage : Page
    {
        private readonly DatabaseService _databaseService;
        public Course CurrentCourse { get; set; }
        public List<Lecture> Lectures { get; set; }
        public List<Test> Tests { get; set; }

        public CoursePage(int courseId)
        {
            InitializeComponent();

            _databaseService = new DatabaseService();

            CurrentCourse = _databaseService.GetCourseById(courseId);

            Lectures = _databaseService.GetLecturesByCourseId(courseId);
            Tests = _databaseService.GetTestsByCourseId(courseId);

            DataContext = this;
        }

        private void LectureButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Lecture selectedLecture)
            {
                NavigationService?.Navigate(new LecturePage(selectedLecture.LectureId));
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Test selectedTest)
            {
                var userId = CurrentUser.User?.UserId ?? 0;
                NavigationService?.Navigate(new TestPage(selectedTest.TestId, userId, CurrentCourse.CourseId));
            }
        }
    }
}
