using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Courses.Services;
using Courses.Views;

namespace Courses.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private readonly AuthService _authService;
        private readonly Window _loginWindow;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                ErrorMessage = string.Empty;
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                ErrorMessage = string.Empty;
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

        public bool HasError => !string.IsNullOrEmpty(_errorMessage);

        public ICommand LoginCommand { get; }
        public ICommand GoToRegistrationCommand { get; }
        public ICommand GoToTeacherWindowCommand { get; }

        public LoginViewModel(Window loginWindow, string connectionString)
        {
            _loginWindow = loginWindow;
            _authService = new AuthService(connectionString);
            LoginCommand = new RelayCommand(() => Login(), () => CanLogin());
            GoToRegistrationCommand = new RelayCommand(() => GoToRegistration());
            GoToTeacherWindowCommand = new RelayCommand(() => GoToTeacherWindow());
        }

        private void GoToTeacherWindow()
        {
            var teacherWindow = new Courses.TeacherWindow();
            teacherWindow.Show();
            _loginWindow.Close();
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private void Login()
        {
            var user = _authService.Authenticate(Username, Password);

            if (user != null)
            {
                CurrentUser.User = user;
                ErrorMessage = string.Empty;

                if (CurrentUser.IsTeacher)
                {
                    GoToTeacherWindow();
                }
                else if (CurrentUser.IsStudent)
                {
                    GoToMainWindow();
                }

                _loginWindow.Close();
            }
            else
            {
                ErrorMessage = "Невірний логін або пароль";
            }
        }

        private void GoToMainWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void GoToRegistration()
        {
            var registrationWindow = new Views.RegistrationPage(_loginWindow, _authService);
            registrationWindow.Show();
            _loginWindow.Hide();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
