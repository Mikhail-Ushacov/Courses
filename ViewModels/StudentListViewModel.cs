using System.Collections.ObjectModel;
using Courses.Models;
using System.Linq;
using Courses.Services;

public class StudentListViewModel
{
    private readonly DatabaseService _databaseService;

    public string CourseName { get; set; }

    public ObservableCollection<StudentEnrollmentDisplay> Students { get; set; }

    public StudentListViewModel(int courseId)
    {
        _databaseService = new DatabaseService();

        int teacherId = CurrentUser.User?.UserId ?? 0;
        var courses = _databaseService.GetCoursesByTeacher(teacherId);

        if (courseId > 0)
        {
            courses = courses.Where(c => c.CourseId == courseId).ToList();
            CourseName = courses.FirstOrDefault()?.CourseName ?? "Невідомий курс";
        }
        else
        {
            CourseName = "Всі курси";
        }

        Students = new ObservableCollection<StudentEnrollmentDisplay>();

        foreach (var course in courses)
        {
            var studentsInCourse = _databaseService.GetStudentsByCourse(course.CourseId);
            foreach (var s in studentsInCourse)
            {
                Students.Add(new StudentEnrollmentDisplay
                {
                    Username = s.student.Username,
                    CourseName = course.CourseName,
                    Confirmed = "✔   ❌"
                });
            }
        }
    }
}

public class StudentEnrollmentDisplay
{
    public string Username { get; set; }
    public string CourseName { get; set; }
    public string Confirmed { get; set; }
}