using System.Windows;
using System.Windows.Controls;

namespace Courses
{
    public partial class AdminWindow : Window
    {
        private AdminViewModel viewModel;

        public AdminWindow()
        {
            InitializeComponent();
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
