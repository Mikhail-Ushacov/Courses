using System.Windows;
using System.Windows.Controls;
using Courses.Services;


namespace Courses
{
    public partial class CourseRegistrationPage : Page
    {
        public CourseRegistrationPage()
        {
            InitializeComponent();
            DataContext = new CourseRegistrationViewModel();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AppNavigationService.GoBack();
        }
    }
}
