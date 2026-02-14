using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

public class LectureViewModel : INotifyPropertyChanged
{
    private Lecture currentLecture;
    private readonly DatabaseService _databaseService;
    private readonly FileService _fileService;

    public Lecture CurrentLecture
    {
        get { return currentLecture; }
        set
        {
            currentLecture = value;
            OnPropertyChanged(nameof(CurrentLecture));
        }
    }

    public string LectureContent { get; set; }

    public LectureViewModel(int lectureId)
    {
        _databaseService = new DatabaseService();
        _fileService = new FileService();
        LoadLecture(lectureId);
    }

    private void LoadLecture(int lectureId)
    {
        var lecture = _databaseService.GetLectureById(lectureId);
        CurrentLecture = lecture;

        LectureContent = _fileService.ReadLectureContent(lecture.ContentFilePath);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
