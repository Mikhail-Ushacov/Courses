public class AnswerOption
{
    public int AnswerId { get; set; }
    public int QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public int Points { get; set; } // 1 для правильної, 0 для неправильної
    public bool IsCorrect => Points > 0;
}