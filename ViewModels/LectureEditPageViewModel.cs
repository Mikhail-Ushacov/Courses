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
        private DateTime? _availableFrom;
        private DateTime? _availableUntil;
        public string Title { get => _title; set { _title = value; OnPropertyChanged(nameof(Title)); } }
        public string Author { get => _author; set { _author = value; OnPropertyChanged(nameof(Author)); } }
        public ObservableCollection<LectureSection> Sections { get; set; } = new();
        public RelayCommand AddSectionCommand { get; }
        public RelayCommand SaveCommand { get; }
        public DateTime? AvailableFrom { get => _availableFrom; set { _availableFrom = value; OnPropertyChanged(nameof(AvailableFrom)); } }
        public DateTime? AvailableUntil { get => _availableUntil; set { _availableUntil = value; OnPropertyChanged(nameof(AvailableUntil)); } }
        private ObservableCollection<Course> _availableCourses = new();
        private Course? _selectedCourse;

        public LectureEditPageViewModel(int courseId, int? lectureId = null)
        {
            _courseId = courseId;
            _lectureId = lectureId;

            if (CurrentUser.User != null)
            {
                var courses = _dbService.GetCoursesByTeacher(CurrentUser.User.UserId);
                AvailableCourses = new ObservableCollection<Course>(courses);
                SelectedCourse = AvailableCourses.FirstOrDefault(c => c.CourseId == _courseId);
            }

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
                AvailableFrom = lecture.AvailableFrom?.DateTime;
                AvailableUntil = lecture.AvailableUntil?.DateTime;
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
            if (SelectedCourse == null) return;

            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Courses", "Lectures");
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, $"lec_{Guid.NewGuid().ToString().Substring(0, 8)}.xml");

            XDocument doc = new XDocument(new XElement("lecture",
                new XElement("title", Title),
                new XElement("author", Author),
                new XElement("date", DateTime.Now.ToString("yyyy-MM-dd")),
                Sections.Select(s => new XElement("section", new XElement("heading", s.Heading), new XElement("paragraph", s.Paragraph)))
            ));
            doc.Save(filePath);
            
            // Використовуємо SelectedCourse.CourseId замість старого _courseId
            _dbService.SaveLectureToDb(SelectedCourse.CourseId, Title, filePath, AvailableFrom, AvailableUntil, _lectureId);
            AppNavigationService.GoBack();
        }

        public ObservableCollection<Course> AvailableCourses 
        { 
            get => _availableCourses; 
            set { _availableCourses = value; OnPropertyChanged(nameof(AvailableCourses)); } 
        }

        public Course? SelectedCourse 
        { 
            get => _selectedCourse; 
            set { _selectedCourse = value; OnPropertyChanged(nameof(SelectedCourse)); } 
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}