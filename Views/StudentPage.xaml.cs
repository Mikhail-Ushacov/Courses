using System.Windows.Controls;
using System;
using System.IO;
using System.Windows;
namespace Courses
{
    public partial class StudentPage : Page
    {
        public StudentPage()
        {
            InitializeComponent();
            DataContext = new StudentViewModel();
        }
    }
}
