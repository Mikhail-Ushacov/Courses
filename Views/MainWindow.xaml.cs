using System;
using System.IO;
using System.Windows;
using Courses.Services;

namespace Courses.Views
{
    /// <summary>
    /// Логіка взаємодії для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Ініціалізація компонентів XAML
            InitializeComponent();

            AppNavigationService.NavigateAction = view =>
            {
                MainFrame.Content = view;
            };

            try
            {
                // 1. Налаштування бази даних перед запуском інтерфейсу
                InitializeDatabase();

                // 2. Встановлення контексту даних (ViewModel)
                // MainViewModel керуватиме логікою перемикання сторінок та даними
                this.DataContext = new MainViewModel();
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

        /// <summary>
        /// Створює необхідні папки та ініціалізує базу даних SQLite
        /// </summary>
        private void InitializeDatabase()
        {
            // Визначаємо шлях до папки в LocalApplicationData (зазвичай C:\Users\User\AppData\Local)
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string projectFolder = Path.Combine(appDataPath, "Courses");

            // Перевіряємо, чи існує папка, якщо ні — створюємо
            if (!Directory.Exists(projectFolder))
            {
                Directory.CreateDirectory(projectFolder);
            }

            // Формуємо шлях до файлу БД
            string dbPath = Path.Combine(projectFolder, "courses.db");
            string connectionString = $"Data Source={dbPath};";

            // Виклик сервісу ініціалізації
            // DatabaseInitializer має містити SQL-скрипти створення таблиць (Users, Courses, Lectures, etc.)
            var initializer = new DatabaseInitializer(connectionString);
            initializer.Initialize();

            // (Опціонально) Початкове заповнення даними, якщо база порожня
            var contentInitializer = new ContentInitializer(connectionString);
            contentInitializer.SeedInitialData();
        }
    }
}