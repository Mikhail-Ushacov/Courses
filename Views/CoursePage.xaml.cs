using Courses.Models;
using Courses.Services;
using Courses.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

        private void LectureCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Lecture selectedLecture)
            {
                AppNavigationService.Navigate(new LecturePage(selectedLecture.LectureId));
            }
        }

        private void TestCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Test selectedTest)
            {
                var userId = CurrentUser.User?.UserId ?? 0;
                AppNavigationService.Navigate(new TestPage(selectedTest.TestId, userId, CurrentCourse.CourseId));
            }
        }
    }
}
