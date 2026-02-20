using System.IO;
using System.Xml.Linq;
using System.Text;
using System.ComponentModel;

public class LectureViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;

    private string lectureTitle = string.Empty;
    private string lectureAuthor = string.Empty;
    private string lectureDate = string.Empty;
    private string lectureContent = string.Empty;

    public string LectureTitle
    {
        get => lectureTitle;
        set { lectureTitle = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LectureTitle")); }
    }

    public string LectureAuthor
    {
        get => lectureAuthor;
        set { lectureAuthor = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LectureAuthor")); }
    }

    public string LectureDate
    {
        get => lectureDate;
        set { lectureDate = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LectureDate")); }
    }

    public string LectureContent
    {
        get => lectureContent;
        set
        {
            lectureContent = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LectureContent"));
        }
    }

    public LectureViewModel(int lectureId)
    {
        _databaseService = new DatabaseService();

        var lecture = _databaseService.GetLectureById(lectureId);

        if (lecture == null)
        {
            LectureContent = "Лекцію не знайдено.";
            return;
        }

        LectureTitle = lecture.Title;

        if (File.Exists(lecture.ContentFilePath))
        {
            ParseLectureXml(lecture.ContentFilePath);
        }
        else
        {
            LectureContent = "Файл лекції не знайдено.";
        }
    }

    private void ParseLectureXml(string filePath)
    {
        try
        {
            var doc = XDocument.Load(filePath);
            var root = doc.Root;

            if (root == null) return;

            var titleEl = root.Element("title");
            if (titleEl != null && string.IsNullOrEmpty(LectureTitle))
                LectureTitle = titleEl.Value;

            var authorEl = root.Element("author");
            if (authorEl != null)
                LectureAuthor = authorEl.Value;

            var dateEl = root.Element("date");
            if (dateEl != null)
                LectureDate = dateEl.Value;

            var sb = new StringBuilder();

            foreach (var section in root.Elements("section"))
            {
                var heading = section.Element("heading");
                if (heading != null)
                {
                    sb.AppendLine("=== " + heading.Value + " ===");
                    sb.AppendLine();
                }

                foreach (var para in section.Elements("paragraph"))
                {
                    sb.AppendLine(para.Value);
                    sb.AppendLine();
                }

                var list = section.Element("list");
                if (list != null)
                {
                    foreach (var item in list.Elements("item"))
                    {
                        sb.AppendLine("• " + item.Value);
                    }
                    sb.AppendLine();
                }
            }

            LectureContent = sb.ToString();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LectureViewModel] Error parsing XML: {ex.Message}");
            LectureContent = "Помилка при читанні лекції.";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
