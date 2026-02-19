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
    }
}
