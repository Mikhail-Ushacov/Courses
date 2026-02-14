using System.IO;
using System.Windows;

namespace EducationalSystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Initialize database on startup
            InitializeDatabase();
            
            DataContext = new MainViewModel();
        }

        private void InitializeDatabase()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Courses",
                "courses.db");
            
            var connectionString = $"Data Source={dbPath};";
            var initializer = new DatabaseInitializer(connectionString);
            initializer.Initialize();
        }
    }
}
