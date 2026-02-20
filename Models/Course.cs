using System.Collections.Generic;

namespace Courses.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public Teacher Instructor { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public List<Lecture> Lectures { get; set; }
        public Test FinalTest { get; set; }
        public double? FinalGrade { get; set; }
    }
}
