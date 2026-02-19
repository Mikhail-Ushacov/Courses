using Courses.Models;
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

        private void CourseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Course selectedCourse)
            {
                NavigationService?.Navigate(new CoursePage(selectedCourse.CourseId));
            }
        }

    }
}
