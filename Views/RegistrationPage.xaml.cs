using System.Windows;
using Courses.Services;
using Courses.ViewModels;
using Wpf.Ui.Controls;

namespace Courses.Views
{
    public partial class RegistrationPage : FluentWindow
    {
        private readonly RegistrationViewModel _viewModel;

        public RegistrationPage(FluentWindow loginWindow, AuthService authService)
        {
            InitializeComponent();
            _viewModel = new RegistrationViewModel(this, loginWindow, authService);
            DataContext = _viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
        }
    }
}
