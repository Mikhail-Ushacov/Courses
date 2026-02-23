public class TestDisplay
{
    public int TestId { get; set; }
    public int CourseId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public bool IsFinalTest { get; set; }
    public DateTimeOffset? AvailableFrom { get; set; }
    public DateTimeOffset? AvailableUntil { get; set; }
    public bool IsCompleted { get; set; }
    public double? Score { get; set; }
    public double? MaxScore { get; set; }
}
