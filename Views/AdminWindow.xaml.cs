using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Courses
{
    public partial class AdminWindow : FluentWindow
    {
        private AdminViewModel viewModel;

        public AdminWindow()
        {
            InitializeComponent();
            SystemThemeWatcher.Watch(this);
            viewModel = new AdminViewModel();
            DataContext = viewModel;
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddUser(PasswordBox.Password);
            PasswordBox.Clear();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
