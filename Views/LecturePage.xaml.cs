using System.Windows.Controls;

namespace Courses
{
    public partial class LecturePage : Page
    {
        public LecturePage(int lectureId)
        {
            InitializeComponent();
            DataContext = new LectureViewModel(lectureId);
        }
    }
}
