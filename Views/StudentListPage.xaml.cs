using System.Windows.Controls;
using Courses;
using Courses.Services;

namespace Courses.Views
{
    public partial class StudentListPage : Page
    {
        public StudentListPage(int courseId)
        {
            InitializeComponent();
            DataContext = new StudentListViewModel(courseId);
        }
    }
}