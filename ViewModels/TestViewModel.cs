using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

public class TestViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;
    private readonly int _userId;
    private readonly int _courseId;

    public ObservableCollection<Question> Questions { get; set; }

    public ICommand SubmitTestCommand { get; }

    public TestViewModel(int testId, int userId, int courseId)
    {
        _databaseService = new DatabaseService();
        _userId = userId;
        _courseId = courseId;

        Questions = new ObservableCollection<Question>(
            _databaseService.GetQuestionsByTestId(testId));

        SubmitTestCommand = new RelayCommand(_ => SubmitTest());
    }

    private void SubmitTest()
    {
        double totalPoints = 0;

        foreach (var question in Questions)
        {
            foreach (var answer in question.AnswerOptions)
            {
                if (answer is SelectableAnswerOption selectable && selectable.IsSelected)
                {
                    totalPoints += answer.Points;
                }
            }
        }

        _databaseService.SaveFinalGrade(_userId, _courseId, totalPoints);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
