using Courses.Models;
using Courses.Services;
using System.Collections.ObjectModel;

namespace Courses.ViewModels
{
    internal class HomeViewModel
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<CourseDisplay> AvailableCourses { get; set; }

        public HomeViewModel()
        {
            _databaseService = new DatabaseService();
            AvailableCourses = new ObservableCollection<CourseDisplay>();

            LoadCourses();
        }

        private void LoadCourses()
        {
            if (!CurrentUser.IsAuthenticated || CurrentUser.User == null)
                return;

            var enrolledCourses = _databaseService
                .GetEnrolledCourses(CurrentUser.User.UserId);

            foreach (var course in enrolledCourses)
            {
                var tests = _databaseService.GetTestsByCourseId(course.CourseId);
                var grade = _databaseService
                    .GetFinalGrade(CurrentUser.User.UserId, course.CourseId);

                AvailableCourses.Add(new CourseDisplay
                {
                    CourseId = course.CourseId,
                    CourseName = course.CourseName,
                    FinalGrade = grade,
                    Tests = tests
                });
            }
        }
    }
}
