using System.Windows.Controls;
using Courses.Services;

namespace Courses
{
    public partial class CoursePage : Page
    {
        public CoursePage(int courseId)
        {
            InitializeComponent();
            
            var userId = CurrentUser.User?.UserId ?? 0;
            DataContext = new CourseViewModel(courseId, userId);
        }
    }
}
