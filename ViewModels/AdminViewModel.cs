using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Courses.Services;

namespace Courses
{
    // DTO Моделі для відображення та редагування в DataGrid адмін-панелі
    public class AdminUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int UserType { get; set; }
    }

    public class AdminCourse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class AdminLecture
    {
        public int LectureId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string AvailableFrom { get; set; }
        public string AvailableUntil { get; set; }
    }

    public class AdminTest
    {
        public int TestId { get; set; }
        public int CourseId { get; set; }
        public string TestName { get; set; }
        public string AvailableFrom { get; set; }
        public string AvailableUntil { get; set; }
        public int IsFinalTest { get; set; }
        public double TestMax { get; set; }
    }

    public class AdminEnrollment
    {
        public int EnrollmentId { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public string EnrollmentDate { get; set; }
        public double FinalGrade { get; set; }
    }

    public class AdminTestResult
    {
        public int ResultId { get; set; }
        public string StudentName { get; set; }
        public string TestName { get; set; }
        public double TestMark { get; set; }
        public double MaxMark { get; set; }
    }

    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly string connectionString = new DatabaseService().ConnectionString;

        // Колекції для кожної таблиці
        public ObservableCollection<AdminUser> AdminUsers { get; set; } = new();
        public ObservableCollection<AdminUser> Teachers { get; set; } = new();
        public ObservableCollection<AdminCourse> AdminCourses { get; set; } = new();
        public ObservableCollection<AdminLecture> AdminLectures { get; set; } = new();
        public ObservableCollection<AdminTest> AdminTests { get; set; } = new();
        public ObservableCollection<AdminEnrollment> AdminEnrollments { get; set; } = new();
        public ObservableCollection<AdminTestResult> AdminTestResults { get; set; } = new();

        // Вибрані елементи у таблицях
        public AdminUser SelectedUser { get; set; }
        public AdminUser SelectedTeacher { get; set; }
        public AdminCourse SelectedCourse { get; set; }
        public AdminLecture SelectedLecture { get; set; }
        public AdminTest SelectedTest { get; set; }
        public AdminEnrollment SelectedEnrollment { get; set; }
        public AdminTestResult SelectedTestResult { get; set; }

        // Властивості для полів вводу
        private string _newUsername;
        public string NewUsername
        {
            get => _newUsername;
            set { _newUsername = value; OnPropertyChanged(); }
        }

        private int _newUserType;
        public int NewUserType
        {
            get => _newUserType;
            set { _newUserType = value; OnPropertyChanged(); }
        }

        private string _newCourseName;
        public string NewCourseName
        {
            get => _newCourseName;
            set { _newCourseName = value; OnPropertyChanged(); }
        }

        // Команди
        public ICommand UpdateUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public ICommand AddCourseCommand { get; }
        public ICommand UpdateCourseCommand { get; }
        public ICommand DeleteCourseCommand { get; }

        public ICommand UpdateLectureCommand { get; }
        public ICommand DeleteLectureCommand { get; }

        public ICommand UpdateTestCommand { get; }
        public ICommand DeleteTestCommand { get; }

        public ICommand DeleteEnrollmentCommand { get; }

        public ICommand UpdateTestResultCommand { get; }
        public ICommand DeleteTestResultCommand { get; }

        public AdminViewModel()
        {
            UpdateUserCommand = new RelayCommand(UpdateUser);
            DeleteUserCommand = new RelayCommand(DeleteUser);

            AddCourseCommand = new RelayCommand(AddCourse);
            UpdateCourseCommand = new RelayCommand(UpdateCourse);
            DeleteCourseCommand = new RelayCommand(DeleteCourse);

            UpdateLectureCommand = new RelayCommand(UpdateLecture);
            DeleteLectureCommand = new RelayCommand(DeleteLecture);

            UpdateTestCommand = new RelayCommand(UpdateTest);
            DeleteTestCommand = new RelayCommand(DeleteTest);

            DeleteEnrollmentCommand = new RelayCommand(DeleteEnrollment);

            UpdateTestResultCommand = new RelayCommand(UpdateTestResult);
            DeleteTestResultCommand = new RelayCommand(DeleteTestResult);

            LoadAllData();
        }

        private void LoadAllData()
        {
            LoadUsers();
            LoadCourses();
            LoadLectures();
            LoadTests();
            LoadEnrollments();
            LoadTestResults();
        }

        #region CRUD: Користувачі
        public void AddUser(string password)
        {
            if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(password)) return;

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            string salt;
            string hash = AuthService.HashPassword(password, out salt);

            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Username, Password, Salt, UserType) VALUES (@u,@p,@s,@t)";
            cmd.Parameters.AddWithValue("@u", NewUsername);
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.Parameters.AddWithValue("@s", salt);
            cmd.Parameters.AddWithValue("@t", NewUserType);

            cmd.ExecuteNonQuery();
            NewUsername = string.Empty; // Очищаємо поле
            LoadUsers();
        }

        private void UpdateUser()
        {
            if (SelectedUser == null) return;
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Users SET Username=@u, UserType=@t WHERE UserId=@id";
            cmd.Parameters.AddWithValue("@id", SelectedUser.UserId);
            cmd.Parameters.AddWithValue("@u", SelectedUser.Username ?? "");
            cmd.Parameters.AddWithValue("@t", SelectedUser.UserType);
            cmd.ExecuteNonQuery();
            LoadUsers();
            LoadCourses(); // Оновлюємо курси, якщо ім'я вчителя змінилося
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
            LoadAllData(); // Оновлюємо все, бо каскадне видалення могло видалити інші дані
        }

        private void LoadUsers()
        {
            AdminUsers.Clear();
            Teachers.Clear();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT UserId, Username, UserType FROM Users";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var user = new AdminUser
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    UserType = reader.GetInt32(2)
                };

                AdminUsers.Add(user);
                if (user.UserType == 1) // 1 = Викладач
                {
                    Teachers.Add(user);
                }
            }
        }
        #endregion

        #region CRUD: Курси
        private void AddCourse()
        {
            if (SelectedTeacher == null || string.IsNullOrWhiteSpace(NewCourseName)) return;

            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Courses (CourseName, TeacherId) VALUES (@n,@t)";
            cmd.Parameters.AddWithValue("@n", NewCourseName);
            cmd.Parameters.AddWithValue("@t", SelectedTeacher.UserId);
            cmd.ExecuteNonQuery();
            
            NewCourseName = string.Empty;
            LoadCourses();
        }

        private void UpdateCourse()
        {
            if (SelectedCourse == null) return;
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Courses SET CourseName=@n, TeacherId=@t, StartDate=@s, EndDate=@e WHERE CourseId=@id";
            cmd.Parameters.AddWithValue("@id", SelectedCourse.CourseId);
            cmd.Parameters.AddWithValue("@n", SelectedCourse.CourseName ?? "");
            cmd.Parameters.AddWithValue("@t", SelectedCourse.TeacherId);
            cmd.Parameters.AddWithValue("@s", string.IsNullOrWhiteSpace(SelectedCourse.StartDate) ? DBNull.Value : SelectedCourse.StartDate);
            cmd.Parameters.AddWithValue("@e", string.IsNullOrWhiteSpace(SelectedCourse.EndDate) ? DBNull.Value : SelectedCourse.EndDate);
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
            LoadAllData();
        }

        private void LoadCourses()
        {
            AdminCourses.Clear();
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT c.CourseId, c.CourseName, c.TeacherId, u.Username, c.StartDate, c.EndDate 
                FROM Courses c 
                LEFT JOIN Users u ON c.TeacherId = u.UserId";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                AdminCourses.Add(new AdminCourse
                {
                    CourseId = reader.GetInt32(0),
                    CourseName = reader.GetString(1),
                    TeacherId = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    TeacherName = reader.IsDBNull(3) ? "Невідомо" : reader.GetString(3),
                    StartDate = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    EndDate = reader.IsDBNull(5) ? "" : reader.GetString(5)
                });
            }
        }
        #endregion

        #region CRUD: Лекції
        private void UpdateLecture()
        {
            if (SelectedLecture == null) return;
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Lectures SET CourseId=@c, Title=@t, AvailableFrom=@af, AvailableUntil=@au WHERE LectureId=@id";
            cmd.Parameters.AddWithValue("@id", SelectedLecture.LectureId);
            cmd.Parameters.AddWithValue("@c", SelectedLecture.CourseId);
            cmd.Parameters.AddWithValue("@t", SelectedLecture.Title ?? "");
            cmd.Parameters.AddWithValue("@af", string.IsNullOrWhiteSpace(SelectedLecture.AvailableFrom) ? DBNull.Value : SelectedLecture.AvailableFrom);
            cmd.Parameters.AddWithValue("@au", string.IsNullOrWhiteSpace(SelectedLecture.AvailableUntil) ? DBNull.Value : SelectedLecture.AvailableUntil);
            cmd.ExecuteNonQuery();
            LoadLectures();
        }

        private void DeleteLecture()
        {
            if (SelectedLecture == null) return;
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Lectures WHERE LectureId=@id";
            cmd.Parameters.AddWithValue("@id", SelectedLecture.LectureId);
            cmd.ExecuteNonQuery();
            LoadLectures();
        }

        private void LoadLectures()
        {
            AdminLectures.Clear();
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT LectureId, CourseId, Title, AvailableFrom, AvailableUntil FROM Lectures";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                AdminLectures.Add(new AdminLecture
                {
                    LectureId = reader.GetInt32(0),
                    CourseId = reader.GetInt32(1),
                    Title = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    AvailableFrom = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    AvailableUntil = reader.IsDBNull(4) ? "" : reader.GetString(4)
                });
            }
        }
        #endregion

        #region CRUD: Тести
        private void UpdateTest()
        {
            if (SelectedTest == null) return;
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Tests SET CourseId=@c, TestName=@n, AvailableFrom=@af, AvailableUntil=@au, IsFinalTest=@f, TestMax=@m WHERE TestId=@id";
            cmd.Parameters.AddWithValue("@id", SelectedTest.TestId);
            cmd.Parameters.AddWithValue("@c", SelectedTest.CourseId);
            cmd.Parameters.AddWithValue("@n", SelectedTest.TestName ?? "");
            cmd.Parameters.AddWithValue("@af", string.IsNullOrWhiteSpace(SelectedTest.AvailableFrom) ? DBNull.Value : SelectedTest.AvailableFrom);
            cmd.Parameters.AddWithValue("@au", string.IsNullOrWhiteSpace(SelectedTest.AvailableUntil) ? DBNull.Value : SelectedTest.AvailableUntil);
            cmd.Parameters.AddWithValue("@f", SelectedTest.IsFinalTest);
            cmd.Parameters.AddWithValue("@m", SelectedTest.TestMax);
            cmd.ExecuteNonQuery();
            LoadTests();
        }

        private void DeleteTest()
        {
            if (SelectedTest == null) return;
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Tests WHERE TestId=@id";
            cmd.Parameters.AddWithValue("@id", SelectedTest.TestId);
            cmd.ExecuteNonQuery();
            LoadTests();
        }

        private void LoadTests()
        {
            AdminTests.Clear();
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT TestId, CourseId, TestName, AvailableFrom, AvailableUntil, IsFinalTest, TestMax FROM Tests";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                AdminTests.Add(new AdminTest
                {
                    TestId = reader.GetInt32(0),
                    CourseId = reader.GetInt32(1),
                    TestName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    AvailableFrom = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    AvailableUntil = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    IsFinalTest = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    TestMax = reader.IsDBNull(6) ? 0 : reader.GetDouble(6)
                });
            }
        }
        #endregion

        #region CRUD: Підписки (Enrollments)
        private void DeleteEnrollment()
        {
            if (SelectedEnrollment == null) return;
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Enrollments WHERE EnrollmentId=@id";
            cmd.Parameters.AddWithValue("@id", SelectedEnrollment.EnrollmentId);
            cmd.ExecuteNonQuery();
            LoadEnrollments();
        }

        private void LoadEnrollments()
        {
            AdminEnrollments.Clear();
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT e.EnrollmentId, u.Username, c.CourseName, e.EnrollmentDate, e.FinalGrade
                FROM Enrollments e
                JOIN Users u ON e.UserId = u.UserId
                JOIN Courses c ON e.CourseId = c.CourseId";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                AdminEnrollments.Add(new AdminEnrollment
                {
                    EnrollmentId = reader.GetInt32(0),
                    StudentName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    CourseName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    EnrollmentDate = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    FinalGrade = reader.IsDBNull(4) ? 0 : reader.GetDouble(4)
                });
            }
        }
        #endregion

        #region CRUD: Результати Тестів
        private void UpdateTestResult()
        {
            if (SelectedTestResult == null) return;
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE TestResults SET TestMark=@m, MaxMark=@mx WHERE ResultId=@id";
            cmd.Parameters.AddWithValue("@id", SelectedTestResult.ResultId);
            cmd.Parameters.AddWithValue("@m", SelectedTestResult.TestMark);
            cmd.Parameters.AddWithValue("@mx", SelectedTestResult.MaxMark);
            cmd.ExecuteNonQuery();
            LoadTestResults();
        }

        private void DeleteTestResult()
        {
            if (SelectedTestResult == null) return;
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM TestResults WHERE ResultId=@id";
            cmd.Parameters.AddWithValue("@id", SelectedTestResult.ResultId);
            cmd.ExecuteNonQuery();
            LoadTestResults();
        }

        private void LoadTestResults()
        {
            AdminTestResults.Clear();
            using var connection = new SqliteConnection(connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT tr.ResultId, u.Username, t.TestName, tr.TestMark, tr.MaxMark
                FROM TestResults tr
                JOIN Users u ON tr.UserId = u.UserId
                JOIN Tests t ON tr.TestId = t.TestId";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                AdminTestResults.Add(new AdminTestResult
                {
                    ResultId = reader.GetInt32(0),
                    StudentName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    TestName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    TestMark = reader.GetDouble(3),
                    MaxMark = reader.GetDouble(4)
                });
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}