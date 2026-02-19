using System.Collections.Generic;

namespace Courses.Models
{
    public class CourseDisplay
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public double? FinalGrade { get; set; }
        public List<Test> Tests { get; set; }
    }
}
