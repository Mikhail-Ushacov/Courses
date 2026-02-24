using Courses.Models;
using Courses.Services;
using Courses.Views;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Courses
{
    public partial class StudentPage : Page
    {
        public StudentPage()
        {
            InitializeComponent();
            DataContext = new StudentViewModel();
        }

        private void CourseCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Course selectedCourse)
            {
                AppNavigationService.Navigate(new CoursePage(selectedCourse.CourseId));
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            AppNavigationService.Navigate(new CourseRegistrationPage());
        }
    }
}
