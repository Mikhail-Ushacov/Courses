using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Courses.Services;

namespace Courses.ViewModels
{
    public class TestEditPageViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _dbService = new DatabaseService();
        private int _courseId;
        private int? _testId;
        private string _testName = "Новий тест";

        public string TestName
        {
            get => _testName;
            set { _testName = value; OnPropertyChanged(nameof(TestName)); }
        }

        public ObservableCollection<QuestionEditModel> Questions { get; set; } = new ObservableCollection<QuestionEditModel>();

        public ICommand AddQuestionCommand { get; }
        public ICommand SaveCommand { get; }

        public TestEditPageViewModel(int courseId, int? testId = null)
        {
            _courseId = courseId;
            _testId = testId;

            AddQuestionCommand = new RelayCommand(_ => {
                var q = new QuestionEditModel { QuestionText = "Нове питання" };
                q.Options.Add(new OptionEditModel { Text = "Варіант 1", IsCorrect = true });
                Questions.Add(q);
            });

            SaveCommand = new RelayCommand(_ => SaveTest());

            if (_testId.HasValue) LoadExistingTest(_testId.Value);
        }

        private void LoadExistingTest(int id)
        {
            var test = _dbService.GetTestById(id);
            if (test != null && File.Exists(test.ContentFilePath))
            {
                TestName = test.TestName;
                try
                {
                    var doc = XDocument.Load(test.ContentFilePath);
                    var root = doc.Root;
                    if (root == null) return;

                    foreach (var qEl in root.Elements("question"))
                    {
                        var q = new QuestionEditModel { QuestionText = qEl.Element("text")?.Value ?? "" };
                        var optionsEl = qEl.Element("options");
                        if (optionsEl != null)
                        {
                            foreach (var oEl in optionsEl.Elements("option"))
                            {
                                q.Options.Add(new OptionEditModel
                                {
                                    Text = oEl.Value,
                                    IsCorrect = oEl.Attribute("correct")?.Value == "true"
                                });
                            }
                        }
                        Questions.Add(q);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading test XML: {ex.Message}");
                }
            }
        }

        private void SaveTest()
        {
            if (string.IsNullOrWhiteSpace(TestName)) return;

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Courses", "Tests");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // Генеруємо нове ім'я файлу або використовуємо існуюче
            string filePath;
            if (_testId.HasValue)
            {
                var existing = _dbService.GetTestById(_testId.Value);
                filePath = existing?.ContentFilePath ?? Path.Combine(path, $"test_{Guid.NewGuid().ToString().Substring(0, 8)}.xml");
            }
            else
            {
                filePath = Path.Combine(path, $"test_{Guid.NewGuid().ToString().Substring(0, 8)}.xml");
            }

            XDocument doc = new XDocument(new XElement("test",
                new XElement("title", TestName),
                Questions.Select((q, i) => new XElement("question", new XAttribute("id", i + 1),
                    new XElement("text", q.QuestionText),
                    new XElement("options", q.Options.Select(o => {
                        var el = new XElement("option", o.Text);
                        el.SetAttributeValue("correct", o.IsCorrect ? "true" : "false");
                        return el;
                    }))))
            ));

            doc.Save(filePath);
            _dbService.SaveTestToDb(_courseId, TestName, filePath, _testId);
            AppNavigationService.GoBack();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // --- ДОПОМІЖНІ КЛАСИ, ЯКИХ НЕ ВИСТАЧАЛО ---

    public class QuestionEditModel : INotifyPropertyChanged
    {
        private string _questionText = string.Empty;
        public string QuestionText
        {
            get => _questionText;
            set { _questionText = value; OnPropertyChanged(nameof(QuestionText)); }
        }

        public ObservableCollection<OptionEditModel> Options { get; set; } = new ObservableCollection<OptionEditModel>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    public class OptionEditModel : INotifyPropertyChanged
    {
        private string _text = string.Empty;
        private bool _isCorrect;

        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(nameof(Text)); }
        }

        public bool IsCorrect
        {
            get => _isCorrect;
            set { _isCorrect = value; OnPropertyChanged(nameof(IsCorrect)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}