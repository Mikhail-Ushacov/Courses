using System.Windows.Controls;


namespace Courses
{
    public partial class CourseRegistrationPage : Page
    {
        public CourseRegistrationPage()
        {
            InitializeComponent();
            DataContext = new CourseRegistrationViewModel();
        }
    }
}
