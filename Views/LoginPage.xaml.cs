using Courses.Services;
using Courses.ViewModels;
using System.IO;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Courses.Views
{
    public partial class LoginPage : FluentWindow
    {
        private LoginViewModel _viewModel;

        public LoginPage()
        {
            InitializeComponent();
            SystemThemeWatcher.Watch(this);
            string connectionString = GetConnectionString();
            _viewModel = new LoginViewModel(this, connectionString);
            DataContext = _viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }

        private string GetConnectionString()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string projectFolder = Path.Combine(appDataPath, "Courses");
            string dbPath = Path.Combine(projectFolder, "courses.db");
            return $"Data Source={dbPath};";
        }
    }
}
