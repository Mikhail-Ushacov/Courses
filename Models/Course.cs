using System.Collections.Generic;

namespace Courses.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public Teacher Instructor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Lecture> Lectures { get; set; }
        public Test FinalTest { get; set; }
        public double? FinalGrade { get; set; }
    }
}