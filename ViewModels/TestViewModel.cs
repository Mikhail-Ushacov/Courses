using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
public class TestViewModel : INotifyPropertyChanged
{
    private Test currentTest;
    private readonly DatabaseService _databaseService;
    private readonly FileService _fileService;

    public Test CurrentTest
    {
        get { return currentTest; }
        set
        {
            currentTest = value;
            OnPropertyChanged(nameof(CurrentTest));
        }
    }

    public string TestQuestions { get; set; }

    public TestViewModel(int testId)
    {
        _databaseService = new DatabaseService();
        _fileService = new FileService();
        LoadTest(testId);
    }

    private void LoadTest(int testId)
    {
        // Fetch test details from database
        var test = _databaseService.GetTestById(testId);
        CurrentTest = test;

        // Load test questions from file
        TestQuestions = _fileService.ReadTestQuestions(test.ContentFilePath);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
