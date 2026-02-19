using System;
using System.IO;
using System.Windows;
using Courses.Services;

namespace Courses
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            AppNavigationService.NavigateAction = view =>
            {
                MainFrame.Content = view;
            };

            try
            {
                InitializeDatabase();

                _viewModel = new MainViewModel();
                this.DataContext = _viewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час запуску програми: {ex.Message}\n\n" +
                                $"Спробуйте перезапустити програму або перевірте права доступу до папки AppData.",
                                "Критична помилка ініціалізації",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                Application.Current.Shutdown();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.IsTeacher)
            {
                TeacherButton.Visibility = Visibility.Visible;
                StudentButton.Visibility = Visibility.Visible;
            }
            else if (CurrentUser.IsStudent)
            {
                TeacherButton.Visibility = Visibility.Collapsed;
                StudentButton.Visibility = Visibility.Visible;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Logout();
            var loginPage = new Views.LoginPage();
            loginPage.Show();
            this.Close();
        }

        private void InitializeDatabase()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string projectFolder = Path.Combine(appDataPath, "Courses");

            if (!Directory.Exists(projectFolder))
            {
                Directory.CreateDirectory(projectFolder);
            }

            string dbPath = Path.Combine(projectFolder, "courses.db");
            string connectionString = $"Data Source={dbPath};";

            var initializer = new DatabaseInitializer(connectionString);
            initializer.Initialize();

            var contentInitializer = new ContentInitializer(connectionString);
            contentInitializer.SeedInitialData();
        }
    }
}
