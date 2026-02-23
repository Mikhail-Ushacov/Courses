using Courses.Models;
using Courses.Services;
using System.Windows;
using System.Windows.Controls;

namespace Courses
{
    public partial class TeacherPage : Page
    {
        public TeacherPage()
        {
            InitializeComponent();
            DataContext = new Courses.ViewModels.TeacherPageViewModel();
        }

        private void CourseCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Course course)
            {
                AppNavigationService.Navigate(new StudentListPage(course.CourseId));
            }
        }
    }
}
