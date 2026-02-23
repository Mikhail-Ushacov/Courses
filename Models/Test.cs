public class Test
{
    public int TestId { get; set; }
    public int CourseId { get; set; }

    public string TestName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string ContentFilePath { get; set; } = string.Empty;

    public DateTimeOffset? AvailableFrom { get; set; }
    public DateTimeOffset? AvailableUntil { get; set; }

    public bool IsFinalTest { get; set; }
    public double TestMax { get; set; }
}
