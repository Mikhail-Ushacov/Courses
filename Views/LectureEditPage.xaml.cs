using System.Windows;
using System.Windows.Controls;
using Courses.ViewModels;
using Courses.Models;
using Courses.Services;

namespace Courses.Views
{
    public partial class LectureEditPage : Page
    {
        public LectureEditPage(int courseId, int? lectureId = null)
        {
            InitializeComponent();
            DataContext = new LectureEditPageViewModel(courseId, lectureId);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            AppNavigationService.GoBack();
        }

        private void RemoveSection_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is LectureSection section)
            {
                var vm = DataContext as LectureEditPageViewModel;
                vm?.Sections.Remove(section);
            }
        }
    }
}