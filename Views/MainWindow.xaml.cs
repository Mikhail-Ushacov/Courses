using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Courses.Services;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Courses
{
    public partial class MainWindow : FluentWindow
    {
        private MainViewModel _viewModel;
    private readonly Stack<object> _navigationHistory = new Stack<object>();
    private readonly Stack<object> _forwardHistory = new Stack<object>();
    private object _currentPage;

        public MainWindow()
        {
            InitializeComponent();
            SystemThemeWatcher.Watch(this);

            AppNavigationService.NavigateAction = view =>
            {
                if (_currentPage != null)
                {
                    _navigationHistory.Push(_currentPage);
                }
                
                _forwardHistory.Clear();
                
                NavigateToPage(view);
            };

            var initialPage = new Views.HomePage();
            NavigateToPage(initialPage);
            _currentPage = initialPage;

            try
            {
                InitializeDatabase();

                _viewModel = new MainViewModel();
                this.DataContext = _viewModel;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Помилка під час запуску програми: {ex.Message}\n\n" +
                                $"Спробуйте перезапустити програму або перевірте права доступу до папки AppData.",
                                "Критична помилка ініціалізації",
                                System.Windows.MessageBoxButton.OK,
                                System.Windows.MessageBoxImage.Error);

                Application.Current.Shutdown();
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
        
        private void NavigateToPage(object page)
        {
            MainFrame.Content = page;
            _currentPage = page;
            
            BackButton.IsEnabled = _navigationHistory.Count > 0;
            ForwardButton.IsEnabled = _forwardHistory.Count > 0;
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_navigationHistory.Count > 0)
            {
                _forwardHistory.Push(_currentPage);
                
                var previousPage = _navigationHistory.Pop();
                NavigateToPage(previousPage);
            }
        }
        
        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_forwardHistory.Count > 0)
            {
                _navigationHistory.Push(_currentPage);
                
                var nextPage = _forwardHistory.Pop();
                NavigateToPage(nextPage);
            }
        }
    }
}
