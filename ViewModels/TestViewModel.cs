using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Xml.Linq;

public class TestViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;
    private readonly int _userId;
    private readonly int _courseId;
    private string testTitle = string.Empty;

    public string TestTitle
    {
        get => testTitle;
        set { testTitle = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TestTitle")); }
    }

    public ObservableCollection<Question> Questions { get; set; }

    public ICommand SubmitTestCommand { get; }
    public ICommand GoToStudentPageCommand { get; }

    public TestViewModel(int testId, int userId, int courseId)
    {
        _databaseService = new DatabaseService();
        _userId = userId;
        _courseId = courseId;

        GoToStudentPageCommand = new RelayCommand(_ => GoToStudentPage());

        try
        {
            var test = _databaseService.GetTestById(testId);

            if (test != null)
            {
                TestTitle = test.TestName;
                
                if (File.Exists(test.ContentFilePath))
                {
                    ParseTestXml(test.ContentFilePath);
                }
                else
                {
                    Questions = new ObservableCollection<Question>();
                }
            }
            else
            {
                Questions = new ObservableCollection<Question>();
            }
        }
        catch (Exception ex)
        {
            Questions = new ObservableCollection<Question>();
        }

        SubmitTestCommand = new RelayCommand(_ => SubmitTest());
    }

    private void GoToStudentPage()
    {
        AppNavigationService.Navigate(new Courses.StudentPage());
    }

    private void ParseTestXml(string filePath)
    {
        try
        {
            var doc = XDocument.Load(filePath);
            var root = doc.Root;

            if (root == null) return;

            var titleEl = root.Element("title");
            if (titleEl != null)
                TestTitle = titleEl.Value;

            var questions = new List<Question>();

            foreach (var q in root.Elements("question"))
            {
                var question = new Question
                {
                    QuestionText = q.Element("text")?.Value ?? "",
                    AnswerOptions = new List<AnswerOption>()
                };

                var options = q.Element("options");
                if (options != null)
                {
                    foreach (var opt in options.Elements("option"))
                    {
                        var isCorrect = opt.Attribute("correct")?.Value == "true";
                        question.AnswerOptions.Add(new SelectableAnswerOption
                        {
                            AnswerId = question.AnswerOptions.Count + 1,
                            AnswerText = opt.Value,
                            Points = isCorrect ? 1 : 0,
                            IsSelected = false
                        });
                    }
                }

                questions.Add(question);
            }

            Questions = new ObservableCollection<Question>(questions);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TestViewModel] Error parsing XML: {ex.Message}");
            Questions = new ObservableCollection<Question>();
        }
    }

    private void SubmitTest()
    {
        double totalPoints = 0;
        int totalCorrect = 0;

        foreach (var question in Questions)
        {
            foreach (var answer in question.AnswerOptions)
            {
                if (answer is SelectableAnswerOption selectable && selectable.IsSelected)
                {
                    totalPoints += answer.Points;
                    if (answer.Points > 0)
                        totalCorrect++;
                }
            }
        }

        _databaseService.SaveFinalGrade(_userId, _courseId, totalPoints);
        GoToStudentPage();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public class QuestionViewModel
    {
        public string Text { get; set; }
        public List<SelectableAnswerOption> Answers { get; set; }

        public QuestionViewModel(Question q)
        {
            Text = q.QuestionText;
            Answers = q.AnswerOptions.Select(a => new SelectableAnswerOption
            {
                AnswerId = a.AnswerId,
                AnswerText = a.AnswerText,
                Points = a.Points
            }).ToList();
        }
    }
}
