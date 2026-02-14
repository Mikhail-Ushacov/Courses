public class Question
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public List<AnswerOption> AnswerOptions { get; set; }
    public int CorrectAnswerId { get; set; }
}
