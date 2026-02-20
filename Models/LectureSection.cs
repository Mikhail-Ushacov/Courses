using System.Collections.ObjectModel;

namespace Courses.Models
{
    public class LectureSection
    {
        public string Heading { get; set; } = string.Empty;
        public string Paragraph { get; set; } = string.Empty;
        public ObservableCollection<string> ListItems { get; set; } = new();
    }
}
