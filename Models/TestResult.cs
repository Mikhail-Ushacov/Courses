public class TestResult
{
    public int ResultId { get; set; }
    public int CourseId { get; set; }
    public int TestId { get; set; }
    public int UserId { get; set; }
    public double TestMark { get; set; }
    public double MaxMark { get; set; }
    public DateTimeOffset CompletedAt { get; set; }
}
