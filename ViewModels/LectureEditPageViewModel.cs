using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Courses.Models;
using Courses.Services;

namespace Courses.ViewModels
{
    public class LectureEditPageViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _dbService = new DatabaseService();
        private int _courseId;
        private int? _lectureId;
        private string _title = "Нова лекція";
        private string _author = "Кафедра";

        public string Title { get => _title; set { _title = value; OnPropertyChanged(nameof(Title)); } }
        public string Author { get => _author; set { _author = value; OnPropertyChanged(nameof(Author)); } }
        public ObservableCollection<LectureSection> Sections { get; set; } = new();

        public RelayCommand AddSectionCommand { get; }
        public RelayCommand SaveCommand { get; }

        public LectureEditPageViewModel(int courseId, int? lectureId = null)
        {
            _courseId = courseId;
            _lectureId = lectureId;

            AddSectionCommand = new RelayCommand(_ => Sections.Add(new LectureSection { Heading = "Новий розділ" }));
            SaveCommand = new RelayCommand(_ => SaveLecture());

            if (_lectureId.HasValue) LoadLecture(_lectureId.Value);
            else Sections.Add(new LectureSection { Heading = "Вступ", Paragraph = "Текст..." });
        }

        private void LoadLecture(int id)
        {
            var lecture = _dbService.GetLectureById(id);
            if (lecture != null && File.Exists(lecture.ContentFilePath))
            {
                Title = lecture.Title;
                try {
                    var doc = XDocument.Load(lecture.ContentFilePath);
                    Author = doc.Root?.Element("author")?.Value ?? Author;
                    foreach (var s in doc.Root.Elements("section"))
                        Sections.Add(new LectureSection { Heading = s.Element("heading")?.Value ?? "", Paragraph = s.Element("paragraph")?.Value ?? "" });
                } catch { }
            }
        }

        private void SaveLecture()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Courses", "Lectures");
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, $"lec_{Guid.NewGuid().ToString().Substring(0,8)}.xml");

            XDocument doc = new XDocument(new XElement("lecture",
                new XElement("title", Title),
                new XElement("author", Author),
                new XElement("date", DateTime.Now.ToString("yyyy-MM-dd")),
                Sections.Select(s => new XElement("section", new XElement("heading", s.Heading), new XElement("paragraph", s.Paragraph)))
            ));
            doc.Save(filePath);
            _dbService.SaveLectureToDb(_courseId, Title, filePath, _lectureId);
            AppNavigationService.GoBack();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}