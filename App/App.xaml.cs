using System;
using System.Windows;

namespace EducationalSystem
{
    public partial class App : Application
    {
        public App()
        {
            // Це конструктор, який ініціалізує компоненти програми (завантаження ресурсів, стилів).
            InitializeComponent();
        }

        // Тут можна додати обробку глобальних подій (наприклад, для помилок)
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Ініціалізація головного вікна
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        // Це викликається при закритті програми
        protected override void OnExit(ExitEventArgs e)
        {
            // Тут можна додати код для звільнення ресурсів, збереження даних або логування.
            base.OnExit(e);
        }
    }
}
