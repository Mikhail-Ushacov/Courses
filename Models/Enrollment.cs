public class Enrollment
{
    public int EnrollmentId { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public bool IsCompleted { get; set; }
    public decimal FinalGrade { get; set; }
}
