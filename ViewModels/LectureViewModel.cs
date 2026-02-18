using System.IO;
using System.ComponentModel;

public class LectureViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;

    private string lectureContent;
    public string LectureContent
    {
        get => lectureContent;
        set
        {
            lectureContent = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LectureContent)));
        }
    }

    public LectureViewModel(int lectureId)
    {
        _databaseService = new DatabaseService();

        var lecture = _databaseService.GetLectureById(lectureId);

        if (File.Exists(lecture.ContentFilePath))
            LectureContent = File.ReadAllText(lecture.ContentFilePath);
        else
            LectureContent = "Файл лекції не знайдено.";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
