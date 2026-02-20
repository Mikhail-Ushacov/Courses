using Courses.Models;
using Courses.Services;
using Courses.ViewModels;
using Courses.Views;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Courses.Views
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            DataContext = new HomeViewModel();
        }

        private void CourseCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                int? courseId = null;

                if (element.DataContext is Course selectedCourse)
                {
                    courseId = selectedCourse.CourseId;
                }
                else if (element.DataContext is CourseDisplay courseDisplay)
                {
                    courseId = courseDisplay.CourseId;
                }

                if (courseId.HasValue)
                {
                    AppNavigationService.Navigate(new CoursePage(courseId.Value));
                }
            }
        }
    }
}
