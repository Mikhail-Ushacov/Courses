using System.Windows.Controls;
using Courses;
using Courses.Services;
using Courses.Views;
using Courses.Models;

namespace Courses
{
    
    public partial class StudentListPage : Page
    {
        public StudentListPage()
        {
            InitializeComponent();
            DataContext = new StudentListViewModel(courseId);
        }
    }
}