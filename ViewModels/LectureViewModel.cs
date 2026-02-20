using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using Courses.Models;

public class LectureViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;

    private string lectureTitle = string.Empty;
    private string lectureAuthor = string.Empty;
    private string lectureDate = string.Empty;
    public ObservableCollection<LectureSection> Sections { get; } = new();

    public string LectureTitle
    {
        get => lectureTitle;
        set { lectureTitle = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LectureTitle))); }
    }

    public string LectureAuthor
    {
        get => lectureAuthor;
        set { lectureAuthor = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LectureAuthor))); }
    }

    public string LectureDate
    {
        get => lectureDate;
        set { lectureDate = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LectureDate))); }
    }

    public LectureViewModel(int lectureId)
    {
        _databaseService = new DatabaseService();

        var lecture = _databaseService.GetLectureById(lectureId);

        if (lecture == null)
        {
            Sections.Add(new LectureSection { Paragraph = "Лекцію не знайдено." });
            return;
        }

        LectureTitle = lecture.Title;

        if (File.Exists(lecture.ContentFilePath))
        {
            ParseLectureXml(lecture.ContentFilePath);
        }
        else
        {
            Sections.Add(new LectureSection { Paragraph = "Файл лекції не знайдено." });
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

            foreach (var section in root.Elements("section"))
            {
                var lectureSection = new LectureSection();

                var heading = section.Element("heading");
                if (heading != null)
                    lectureSection.Heading = heading.Value;

                var para = section.Element("paragraph");
                if (para != null)
                    lectureSection.Paragraph = para.Value;

                var list = section.Element("list");
                if (list != null)
                {
                    foreach (var item in list.Elements("item"))
                    {
                        lectureSection.ListItems.Add(item.Value);
                    }
                }

                Sections.Add(lectureSection);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LectureViewModel] Error parsing XML: {ex.Message}");
            Sections.Add(new LectureSection { Paragraph = "Помилка при читанні лекції." });
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
