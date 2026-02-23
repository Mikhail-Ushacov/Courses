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
            
            this.Loaded += TeacherPage_Loaded;
        }

        private void TeacherPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Courses.ViewModels.TeacherPageViewModel vm)
            {
                vm.LoadData();
            }
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