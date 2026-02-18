using System;

public class Lecture
{
    public int LectureId { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentFilePath { get; set; } = string.Empty;

    public DateTime? AvailableFrom { get; set; }
    public DateTime? AvailableUntil { get; set; }
}
