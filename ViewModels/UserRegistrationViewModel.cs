using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Courses.Services;
using Wpf.Ui.Controls;

namespace Courses.ViewModels
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private readonly AuthService _authService;
        private readonly FluentWindow _registrationWindow;
        private readonly FluentWindow _loginWindow;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                ClearMessages();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                ClearMessages();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
                ClearMessages();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(HasError));
            }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                _successMessage = value;
                OnPropertyChanged(nameof(SuccessMessage));
                OnPropertyChanged(nameof(HasSuccess));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(_errorMessage);
        public bool HasSuccess => !string.IsNullOrEmpty(_successMessage);

        public ICommand RegisterCommand { get; }
        public ICommand GoBackCommand { get; }

        public RegistrationViewModel(FluentWindow registrationWindow, FluentWindow loginWindow, AuthService authService)
        {
            _registrationWindow = registrationWindow;
            _loginWindow = loginWindow;
            _authService = authService;
            RegisterCommand = new RelayCommand(() => Register(), () => CanRegister());
            GoBackCommand = new RelayCommand(() => GoBack());
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password)
                && !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        private void Register()
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Паролі не співпадають";
                return;
            }

            if (Password.Length < 3)
            {
                ErrorMessage = "Пароль має бути не менше 3 символів";
                return;
            }

            if (_authService.UsernameExists(Username))
            {
                ErrorMessage = "Користувач з таким ім'ям вже існує";
                return;
            }

            bool success = _authService.RegisterStudent(Username, Password);

            if (success)
            {
                SuccessMessage = "Реєстрація успішна! Тепер ви можете увійти.";
                Username = string.Empty;
                Password = string.Empty;
                ConfirmPassword = string.Empty;
            }
            else
            {
                ErrorMessage = "Помилка при реєстрації. Спробуйте ще раз.";
            }
        }

        private void GoBack()
        {
            _loginWindow.Show();
            _registrationWindow.Close();
        }

        private void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
