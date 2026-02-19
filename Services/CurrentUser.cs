using Courses.Models;

namespace Courses.Services
{
    public static class CurrentUser
    {
        private static User? _user;

        public static User? User
        {
            get => _user;
            set => _user = value;
        }

        public static bool IsAuthenticated => _user != null;

        public static bool IsStudent => _user?.UserType == UserType.Student;

        public static bool IsTeacher => _user?.UserType == UserType.Teacher;

        public static bool IsAdmin => _user?.UserType == UserType.Admin;

        public static void Logout()
        {
            _user = null;
        }
    }
}
