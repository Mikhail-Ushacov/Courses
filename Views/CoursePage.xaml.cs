using Courses.Models;
using Courses.Views;
using Courses.Services;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Courses
{
    public partial class CoursePage : Page
    {
        private readonly DatabaseService _databaseService;
        private Course _course;

        public CoursePage(int courseId)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();

            _course = _databaseService.GetCourseById(courseId);

            DataContext = _course;
        }
    }
}
