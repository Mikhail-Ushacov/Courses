using System.Collections.ObjectModel;
using Courses.Models;

public class StudentListViewModel
{
    private readonly DatabaseService _databaseService;

    public string CourseName { get; set; }

    public ObservableCollection<StudentGradeDisplay> Students { get; set; }

    public StudentListViewModel(int courseId)
    {
        _databaseService = new DatabaseService();

        var course = _databaseService.GetCourseById(courseId);
        CourseName = course?.CourseName ?? "Unknown Course";

        var students = _databaseService.GetStudentsByCourse(courseId);

        Students = new ObservableCollection<StudentGradeDisplay>(
            students.Select(s => new StudentGradeDisplay
            {
                Username = s.student.Username,
                Grade = s.grade
            }));
    }
}

public class StudentGradeDisplay
{
    public string Username { get; set; }
    public double? Grade { get; set; }
}