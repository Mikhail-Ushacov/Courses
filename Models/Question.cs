using System.Collections.Generic;

public class Question
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<AnswerOption> AnswerOptions { get; set; } = new();
}