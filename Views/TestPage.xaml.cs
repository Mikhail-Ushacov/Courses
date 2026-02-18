using System.Windows.Controls;

namespace Courses
{
    public partial class TestPage : Page
    {
        public TestPage(int testId, int userId, int courseId)
        {
            InitializeComponent();
            DataContext = new TestViewModel(testId, userId, courseId);
        }
    }
}
