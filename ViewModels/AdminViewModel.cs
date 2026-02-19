using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Courses.Models;

namespace Courses
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private string connectionString = "Data Source=database.db";

        public ObservableCollection<User> Users { get; set; } = new();
        public ObservableCollection<User> Teachers { get; set; } = new();
        public ObservableCollection<User> Admins { get; set; } = new();
        public ObservableCollection<Course> Courses { get; set; } = new();

        public User SelectedUser { get; set; }
        public Course SelectedCourse { get; set; }
        public User SelectedTeacher { get; set; }

        public string NewUsername { get; set; }
        public int NewUserType { get; set; }
        public string NewCourseName { get; set; }

        public ICommand DeleteUserCommand { get; }
        public ICommand AddCourseCommand { get; }
        public ICommand DeleteCourseCommand { get; }

        public AdminViewModel()
        {
            LoadUsers();
            LoadCourses();

            DeleteUserCommand = new RelayCommand(DeleteUser);
            AddCourseCommand = new RelayCommand(AddCourse);
            DeleteCourseCommand = new RelayCommand(DeleteCourse);
        }

        public void AddUser(string password)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Username, Password, Salt, UserType) VALUES (@u,@p,@s,@t)";
            cmd.Parameters.AddWithValue("@u", NewUsername);
            cmd.Parameters.AddWithValue("@p", password);
            cmd.Parameters.AddWithValue("@s", "salt");
            cmd.Parameters.AddWithValue("@t", NewUserType);

            cmd.ExecuteNonQuery();
            LoadUsers();
        }

        private void DeleteUser()
        {
            if (SelectedUser == null) return;

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Users WHERE UserId=@id";
            cmd.Parameters.AddWithValue("@id", SelectedUser.UserId);
            cmd.ExecuteNonQuery();

            LoadUsers();
        }

        private void AddCourse()
        {
            if (SelectedTeacher == null) return;

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Courses (CourseName, TeacherId) VALUES (@n,@t)";
            cmd.Parameters.AddWithValue("@n", NewCourseName);
            cmd.Parameters.AddWithValue("@t", SelectedTeacher.UserId);

            cmd.ExecuteNonQuery();
            LoadCourses();
        }

        private void DeleteCourse()
        {
            if (SelectedCourse == null) return;

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Courses WHERE CourseId=@id";
            cmd.Parameters.AddWithValue("@id", SelectedCourse.CourseId);
            cmd.ExecuteNonQuery();

            LoadCourses();
        }

        private void LoadUsers()
        {
            Users.Clear();
            Teachers.Clear();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var user = new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    UserType = (UserType)reader.GetInt32(4)
                };

                Users.Add(user);

                if (user.UserType == UserType.Teacher)
                    Teachers.Add(user);

                if (user.UserType == UserType.Admin)
                    Admins.Add(user);
            }
        }

        private void LoadCourses()
        {
            Courses.Clear();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Courses";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Courses.Add(new Course
                {
                    CourseId = reader.GetInt32(0),
                    CourseName = reader.GetString(1)
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
