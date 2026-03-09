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

        public string GradeDisplay
        {
            get
            {
                if (!FinalGrade.HasValue || FinalGrade.Value <= 0)
                    return "Не завершено";
                if (FinalTest != null && FinalTest.TestMax > 0)
                    return $"{FinalGrade.Value:F1}/{FinalTest.TestMax:F0}";
                return $"{FinalGrade.Value:F1}";
            }
        }
    }
}
