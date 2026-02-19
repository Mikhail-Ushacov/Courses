using System.Windows.Controls;

namespace Courses
{
    public partial class TeacherPage : Page
    {
        public TeacherPage()
        {
            InitializeComponent();
            DataContext = new TeacherViewModel();
        }
    }
}
